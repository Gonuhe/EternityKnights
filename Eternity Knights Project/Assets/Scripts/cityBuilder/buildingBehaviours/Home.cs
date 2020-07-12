using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Classe représentant une maison, ses principales données et les actions possibles dessus.
 **/
[RequireComponent(typeof(EvolutionManager))]
[RequireComponent(typeof(HomeCarriersFreightAreaInBehaviour))]
public class Home : Building 
{
  private GameManager _gameManager;

  public int inhabitantsCount = 0;

  public int capacity = 4;

  //Enregistre combien de travailleurs de cette maison travaillent à quel endroit.
  private Dictionary<WorkPlace, int> _workPlaces;

  private int _availableForWork = 0;
  
  /*
  Contient pour chaque ressource consommée par les habitants de la maison la 
  fréquence à laquelle cette ressource est consommée (en secondes).
  La consommation effective suivant les infos contenues dans ce dictionnaires
  est gérée par une coroutine pour chaque ressource concernée.
  */
  private Dictionary<string,float> _homeConsumptionData=new Dictionary<string,float>();
  
  /*
  Contient les ressources qui n'ont pas pu être consommées par la maison au terme
  du timer de la coroutine de consommation car il n'y en a plus. C'est à ce champ
  que le HomeEvolutionData doit se fier pour savoir comment faire déévoluer la maison
  (en l'appelant via la méthode CouldNotConsume).
  
  Le contenu de ce HashSet est intégralement géré par les coroutines de consommation
  de ressources (fonction HomeConsumptionCoroutine).
  */
  private HashSet<string> _resourcesUnableToBeConsumed=new HashSet<string>();
  
  private Dictionary<ResourceCarrier,ResourceShipment> _ordersToPick=new Dictionary<ResourceCarrier,ResourceShipment>();

  new void Start () 
  {
    base.Start();
    _gameManager = GameManager.instance;
    _gameManager.cityBuilderData.homeAvailable += capacity;
    _workPlaces = new Dictionary<WorkPlace, int>(capacity);
    StartCoroutine(AgoraReservationCoroutine());
  }
	
  new void Update () 
  {
    base.Update();
  }

  public int GetAvailableForWork()
  {
    return _availableForWork;
  }

  public bool HaveFreeRooms()
  {
    return inhabitantsCount < capacity;
  }

  public bool MoveIn()
  {
    if(inhabitantsCount < capacity)
    {
      _availableForWork++;
      inhabitantsCount++;
      _gameManager.cityBuilderData.homeAvailable--;
      _gameManager.cityBuilderData.homeless--;
      _gameManager.cityBuilderData.population++;
      return true;
    }
    else
    {
      return false;
    }
  }

  public int GetNearAvailableWork(int distance)
  {
    return Utils.GetNearObjects(gameObject, "WorkPlace", distance).Count;
  }

  public void Hire(WorkPlace workPlace, int quantity)
  {
    _availableForWork -= quantity;

    if(_workPlaces.ContainsKey(workPlace))
    {
      int previousWorkersAtThatPlace;
      _workPlaces.TryGetValue(workPlace, out previousWorkersAtThatPlace);
      _workPlaces.Remove(workPlace);
      _workPlaces.Add(workPlace, quantity);
    }
    else
    {
      _workPlaces.Add(workPlace, quantity);
    }
  }

  void OnDestroy()
  {
    _gameManager.cityBuilderData.homeAvailable-= (capacity - inhabitantsCount);
    _gameManager.cityBuilderData.homeless += inhabitantsCount;
    _gameManager.cityBuilderData.population -= inhabitantsCount;

    foreach(KeyValuePair<WorkPlace, int> workPlaceWorking in _workPlaces)
    {
      workPlaceWorking.Key.Quit(this, workPlaceWorking.Value);
    }

    //TODO ne marche pas car au moment la coroutine est créée l'objet n'existe plus (inactif) ! StartCoroutine(GenerateHumansRoutine(inhabitantsCount));
    // Comment faire popper des gens ? sachant qu'ils ne peuvent pas tous popper en même temps à cause des lock sur les routes.
    // La question devient encore plus compliquée quand on aura des bâtiments avec 32+ habitants...
  }

  /**
   * Pour virer 'number' habitants de cette maison de leur travail à 'workPlace'
   * Pré : 'number' <= nombre de personne de cette maison qui travaillent à workPlace
   **/
  public void Fire(WorkPlace workPlace, int number)
  {
    int currentNumber;
    if(_workPlaces.TryGetValue(workPlace, out currentNumber)) // il y avait des gens dans cette maison, sinon, envoyer une exception !
    {
      _workPlaces.Remove(workPlace);
      if(currentNumber > number)
      {
        _workPlaces.Add(workPlace, currentNumber - number);
      }
      //ELSE ERROR, ne peut se produire, pré pas respectée
    } 
  }

  /**
   * Fonction qui gère le départ de 'number' habitants de la maisons, en les virants des différents aspects du citybuilder où ils ont une influence (virer de leur travail,...).
   * C'est aussi cette fonction qui doit gérer les priorités entre les différents travails.
   **/
  public void LeaveHouse(int number)
  {
    int yetToLeave = number;

    //On vire d'abord les chômeurs
    if(_availableForWork > 0)
    {
      _availableForWork -= yetToLeave;
      if(_availableForWork < 0)
      {
        yetToLeave = -_availableForWork;
        _availableForWork = 0;
      }
      else //on a viré assez de gens
      {
        MakeHumansPop(number);
        return;
      }
    }

    //On vire ensuite les travailleurs
    foreach(KeyValuePair<WorkPlace, int> wp in _workPlaces)
    {
      int workerAfter = wp.Value;
      workerAfter -= yetToLeave;
      if(workerAfter < 0) // Il faut virer tout le monde de cette workplace
      {
        yetToLeave = -workerAfter;
        _workPlaces.Remove(wp.Key);
        wp.Key.Quit(this, wp.Value);
      }
      else //On vire partiellement, on aura viré assez de gens
      {
        _workPlaces.Remove(wp.Key);
        if(workerAfter != 0)
          _workPlaces.Add(wp.Key, workerAfter);
        wp.Key.Quit(this, wp.Value-workerAfter);
        MakeHumansPop(number);
        return;
      }
    }
  }

  private void MakeHumansPop(int number)
  {
    GameManager.instance.cityBuilderData.homeless += number;//TODO à changer si on souhaite utiliser cette fonction pour faire popper autre chose que des homeless !
    StartCoroutine(GenerateHumansRoutine(number));
  }

  /**
  * Retourne la ressource la plus nécessaire pour la maison, soit parce qu'elle
  * va régresser si elle n'en reçoit pas rapidement, soit parce qu'elle en a
  * besoin pour évoluer.
  *
  * Si la maison ne consomme pas de ressources (c'est le cas aux niveaux 0 et 1),
  * cette méthode retourne null.
  **/
  private string HighestResourceNeed(HashSet<string> excludeSet)
  { 
    Dictionary<string,int> allOrdersToPick=new Dictionary<string,int>();
    
    string highestNeed=null;
    float lowestScore=0.0f;
    foreach(KeyValuePair<string,float> need in _homeConsumptionData)
    {	
      int orderToPick= allOrdersToPick.ContainsKey(need.Key) ? allOrdersToPick[need.Key] : 0;
      float needScore= (buildingStock.StockFor(need.Key)+orderToPick) * need.Value;  	
      
      if(!excludeSet.Contains(need.Key) && (needScore<lowestScore || highestNeed==null))
      {
        lowestScore=needScore;
        highestNeed=need.Key;
      }
    }
    
    return highestNeed;
  }
  
  private Dictionary<string,int> AllOrdersToPick()
  {
  	Dictionary<string,int> rslt=new Dictionary<string,int>();
    foreach(ResourceShipment shipment in _ordersToPick.Values)
    {
      string resource=shipment.resourceName;
      
      if(rslt.ContainsKey(resource))
        rslt[resource]+=shipment.amount;
      else 
      	rslt[resource]=shipment.amount;
    }
    
    return rslt;
  }
  
  private int TotalOrderToPick(string resource)
  {
  	int rslt=0;
    foreach(ResourceShipment shipment in _ordersToPick.Values)
    {
      if(shipment.resourceName==resource) rslt+=shipment.amount;
    }
    
    return rslt;
  }
  
  private void RegisterOrderToPick(ResourceCarrier carrier,string resource,int amount)
  {
    _ordersToPick[carrier]=new ResourceShipment(resource,amount);
  }
  
  public void CancelOrderToPick(ResourceCarrier deliveringCarrier)
  {
    _ordersToPick.Remove(deliveringCarrier);
  }
  
  private IEnumerator AgoraReservationCoroutine()
  {	 
    while(true)
    {
      if(inhabitantsCount>0)
      { 
        Cell<PriorityQueue<float,KeepAsideOrderManager>> cell=new Cell<PriorityQueue<float,KeepAsideOrderManager>>();
        yield return StartCoroutine(SortAvailableKeepAsideOrderManagersByDistance(cell));
                     
        HashSet<string> needExcludeSet=new HashSet<string>();
        while(freightAreaData.availableCarriers > 0) 
        {
          string toOrder=HighestResourceNeed(needExcludeSet);
          if(toOrder==null)
            break; //Fait avec un break pour alléger un peu le code et éviter d'avoir encore des acollades partout !
          
          int amountToOrder=Math.Min(freightAreaData.carrierPrefab.capacity,buildingStock.FreeSpaceFor(toOrder)+TotalOrderToPick(toOrder));
          
          bool ordered=false;
          
          if(amountToOrder>0)
          {
            foreach(KeepAsideOrderManager keepAsideManager in cell.value)
            {
      	      if(amountToOrder>0 && keepAsideManager!=null && keepAsideManager.CanMakeKeepAsideOrder(toOrder))
      	      {
      	  	    ResourceCarrier carrier=freightAreaData.InstantiateCarrierWithoutCollider();
      	  	    carrier.collectingResources=true;
      	  	    int orderedAmount=keepAsideManager.MakeKeepAsideOrder(toOrder,amountToOrder,carrier);
      	  	    RegisterOrderToPick(carrier,toOrder,orderedAmount);
      	  	    freightAreaData.SendCarrier(carrier,keepAsideManager.freightAreaData.parentStock,Orientation.SOUTH,null);//TODO orientation
      	  	    ordered=true;
      	  	    break;
              }
            }
          }
          
          if(!ordered) //S'il n'a pas été commandé, c'est que personne ne peut le fournir en ce moment ou que le stock est plein. On se concentre donc sur les autres besoins
            needExcludeSet.Add(toOrder);
          
          yield return new WaitForSeconds(1.0f); 
        }
      }
    	
      yield return new WaitForSeconds(1.0f);//TODO du random et un délais plus grand ici? Dépend de la déévolution et de sa vitesse !
    }
  }
  
  private IEnumerator SortAvailableKeepAsideOrderManagersByDistance(Cell<PriorityQueue<float,KeepAsideOrderManager>> rslt)
  {
  	StockManager stockManager=GameManager.instance.cityBuilderData.stockManager;

  	PriorityQueue<float,KeepAsideOrderManager> rsltQueue=new PriorityQueue<float,KeepAsideOrderManager>();
  	IEnumerator<KeepAsideOrderManager> keepAsideEnumerator=stockManager.AllKeepAsideOrderManagers();
    while(keepAsideEnumerator.MoveNext())
    {
      KeepAsideOrderManager keepAsideManager=keepAsideEnumerator.Current;
      
      if(keepAsideManager!=null)//On vérifie s'il n'a pas été détruit
      {
        float distance=Vector2.Distance(transform.position,keepAsideManager.transform.position);
      
        if(distance<=keepAsideManager.availabilityRange)//Pour éviter les calculs inutiles ; si ce n'est pas le cas, il est de toute façon trop loin
        {
          FreightAreaIn inArea=keepAsideManager.freightAreaData.freightAreaIn;
          FreightAreaOut outHome=freightAreaData.freightAreaOut;
        
          float roadDistance=RoadsPathfinding.RealDistanceBetween(inArea.road,outHome.road);
        
          if(roadDistance>0.0f && roadDistance<=keepAsideManager.availabilityRange)
            rsltQueue.Push(roadDistance,keepAsideManager);
        }
      }
    	
      yield return null;
    }
    
    rslt.value=rsltQueue;
  }

  private IEnumerator GenerateHumansRoutine(int toGenerate)
  {
    RoadData door = GetComponent<FreightAreaData>().freightAreaIn.road;
    for(int i = 0; i<toGenerate; i++)
    {
      if(door.roadLock.IsFree(Orientation.SOUTH))
      {
        GameObject newHuman = (GameObject)Instantiate(Resources.Load("Prefabs/HumanTest"), door.transform.position, Quaternion.identity);

        newHuman.GetComponent<RoadRouteManager>().occupiedRoad = door;
        door.roadLock.LockFor(Orientation.SOUTH,newHuman);

        newHuman.GetComponent<Human>().SearchHome();
      }
      else
        i--;
      yield return new WaitForSeconds(1 + UnityEngine.Random.Range(0.25f,1.5F));
    }
  }
  
  public void AddToHomeConsumption(string resource,float frequency)
  {
    bool contained=_homeConsumptionData.ContainsKey(resource);
    
    _homeConsumptionData[resource]=frequency;
    
    if(!contained) //Si on ne consommait pas déjà cette ressource auparavant, une nouvelle coroutine est lancée pour la gérer
      StartCoroutine(HomeConsumptionCoroutine(resource));
  }
  
  public void RemoveFromHomeConsumption(string resource)
  {
    _homeConsumptionData.Remove(resource);
  }
  
  private IEnumerator HomeConsumptionCoroutine(string resource)
  {
    while(_homeConsumptionData.ContainsKey(resource))
    {
      if(buildingStock.StockFor(resource)>0)
      {
      	//Puisqu'on peut le consommer, on l'en retire s'il s'y trouvait (ce qui peut donc arriver avant que l'EvolutionData ne s'en apperçoive)
      	_resourcesUnableToBeConsumed.Remove(resource);
      	
        buildingStock.RemoveFromStock(resource,1);
      	  
        yield return new WaitForSeconds(_homeConsumptionData[resource]);
      }
      else //Cette ressource n'est plus en stock, mais on doit la consommer !
      {
        _resourcesUnableToBeConsumed.Add(resource);
        yield return null; //On a BESOIN de cette ressource ! On réessaie donc à la frame suivante !
      }
    }
  }
  
  /**
  * Retourne true ssi pour l'une des ressources passées en paramètre (c'est donc 
  * un OR), il n'y avait plus de stock au moment de la consommer.
  * Cette méthode est particulièrement utile pour gérer la régression des maison.
  **/
  public bool CouldNotConsume(params string[] resources)
  {
  	foreach(string resource in resources)
  	{
      if(_resourcesUnableToBeConsumed.Contains(resource)) return true;
    }
    
    return false;
  }
}
