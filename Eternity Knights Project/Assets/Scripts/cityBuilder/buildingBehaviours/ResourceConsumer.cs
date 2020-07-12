using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

/**
* Attribut à attacher à tout bâtiment consommant des ressources.
**/
[Serializable]
[RequireComponent(typeof(WorkPlace))]
public class ResourceConsumer : MonoBehaviour
{
  //Contient le stock du bâtiment auquel ce script est attaché
  protected BuildingStock _currentStock;
  
  //Événement déclenché après chaque consommation de ressources 
  public UnityEvent consumptionEvent;  
  
  //Fréquence, en secondes, de la consommation de ressources
  public float consumptionRate;
  
  //Entraîne une consommation quand tombe à 0
  private float _consumptionCountDown;  
  
  //Indique si le bâtiment est actuellement en cours de consommation (et a donc déjà consommé ses prérequis)
  private bool _consuming=false;
  
  /*
  Nombre de fois que les requirements de consommation peuvent être contenus 
  dans le stock (la limite de stock est modifiée en conséquence dans Awake, pas 
  besoin de faire plus)
  */
  public int requirementsBufferSize;
  
  //Liste des prérequis devant être remplis pour que le bâtiment ne les consomme et n'appelle l'évènement de consommation.
  public ResourceShipment[] requirements;
  
  //Ressources à commander dans un entrepôt dès que possible pour remplir le stock de requirements en buffer
  private Dictionary<string,int> _resourcesToOrder=new Dictionary<string,int>();
  
  protected virtual void Awake()
  {
    _currentStock=GetComponent<BuildingStock>();   
    _consumptionCountDown=consumptionRate;
    
    foreach(ResourceShipment requirement in requirements)
    {
      int bufferSubSum=requirement.amount*requirementsBufferSize;
      _resourcesToOrder[requirement.resourceName]=bufferSubSum;
      _currentStock.stockLock.totalStockLimit+=bufferSubSum;
      _currentStock.stockLock.SetLimitFor(requirement.resourceName,bufferSubSum);
    }
  }
  
  protected virtual void Start()
  {
  	StartCoroutine(ResourceOrdererCoroutine());
    StartCoroutine(Consume());
  }

  /**
  * pré: RequirementsMet()==true
  *
  * Retire du stock du bâtiment tous les prérequis à à consommer en un coup.
  **/
  private void RetrieveRequirements()
  {
    foreach(ResourceShipment requirement in requirements)
    {
      _currentStock.RemoveFromStock(requirement.resourceName,requirement.amount);  
      _resourcesToOrder[requirement.resourceName]+=requirement.amount;
    }
  }
  
  /**
  * Vérifie que les prérequis de consommation sont remplis.
  **/
  protected virtual bool RequirementsMet()
  {
    foreach(ResourceShipment requirement in requirements)
    {
      if(_currentStock.StockFor(requirement.resourceName)<requirement.amount)
        return false;
    }

    return true;
  }
  
  /**
  * Coroutine de consommation de ressources.
  **/
  private IEnumerator Consume()
  {
  	WorkPlace workPlace=GetComponent<WorkPlace>();
  	
  	float lastWorkersRatio=workPlace.WorkersRatio();
    while(true)
    {
      if(!_consuming) 
      {
      	if(RequirementsMet())
      	{
          RetrieveRequirements();
          _consuming=true;
        }
        else 
          yield return new WaitForSeconds(1.0f);//Il attend une seconde avant de revérifier
      }
      else 
      {
        if(Utils.FloatComparison(_consumptionCountDown,0.0f,0.0001f) || _consumptionCountDown<0.0f) //Donc en somme si _consumptionCountDown est <=0, avec une petite marge d'erreur pour l'imprécision des floats
        {
          consumptionEvent.Invoke();
          _consumptionCountDown=consumptionRate;
          _consuming=false;
        }
        else//Il n'est pas encore temps d'appliquer les effets de la consommation, on "digère" toujours !
        {
          yield return new WaitForSeconds(consumptionRate);
          _consumptionCountDown-= lastWorkersRatio*consumptionRate;//On utilise chaque fois le ratio précédent pour éviter les changements trop soudains de rythme
          lastWorkersRatio=workPlace.WorkersRatio();
          //Cette façon de faire permet de s'adapter relativement dynamiquement aux changements de ratio ;)
        }
      }
    }	
  }
  
  /**
  * Ajoute au stock du bâtiment les chargements de ressources dans la liste.
  *
  * ATTENTION: il faut au préalable vérifier qu'ajouter ces chargements ne dépasse
  * pas la capacité du stock, définie dans son StockLock.
  **/
  public void ReceiveShipment(List<ResourceShipment> shipments)
  {
    foreach(ResourceShipment shipment in shipments)
    {
      _currentStock.AddToStock(shipment.resourceName,shipment.amount);
    }  
  } 
  
  /**
  * Coroutine de commande des ressources auprès du système de stockage.
  * Cette coroutine se charge en continu de commander auprès des entrepôts à 
  * proximité les ressources nécessaires pour remplir le stock alloué au buffer
  * de consommation.
  **/
  private IEnumerator ResourceOrdererCoroutine()
  {
  	FreightAreaIn freightIn=_currentStock.freightAreaData.freightAreaIn;
  	
    while(requirements.Length>0)
    {
      Cell<PriorityQueue<float,OrderManager>> rslt=new Cell<PriorityQueue<float,OrderManager>>();
      yield return StartCoroutine(freightIn.road.SortOrderManagersByDistance(rslt));
      PriorityQueue<float,OrderManager> sortedOrderManagers=rslt.value;
    	
      foreach(ResourceShipment requirement in requirements)
      {      	  
        foreach(OrderManager orderManager in sortedOrderManagers)
        {
          string resourceName=requirement.resourceName;
          if(_resourcesToOrder[resourceName]==0) break;
          
          int orderedAmount=orderManager.Order(resourceName,_resourcesToOrder[resourceName],_currentStock);	
          _resourcesToOrder[resourceName]-=orderedAmount;
          
          yield return null;
        }
      }
    }
  }
  
  /**
  * Annule une commande passée (car le transporteur s'est trouvé bloqué trop 
  * longtemps ou que le bâtiment a perdu son lien au réseau routier avant 
  * expédition de sa commande) en réindiquant le chargement comme "à commander".
  **/
  public void CancelResourceOrder(ResourceShipment order)
  {
    _resourcesToOrder[order.resourceName]+=order.amount;
  }
}