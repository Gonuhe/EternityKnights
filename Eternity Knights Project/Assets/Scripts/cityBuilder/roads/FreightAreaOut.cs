using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreightAreaOut : MonoBehaviour
{   
  //RoadData de la route servant de zone de frêt
  [HideInInspector]
  public RoadData road;
    
  private Queue<ResourceCarrier> _waitingCarriers=new Queue<ResourceCarrier>();
  
  private HashSet<Collider2D> _freightAreaDataTreatedColliders;
  
  protected void Awake()
  {
    road=GetComponent<RoadData>();
    _freightAreaDataTreatedColliders=GetComponentInParent<FreightAreaData>().treatedColliders;
  }
  
  protected void Start()
  {
  	StartCoroutine(SendWaitingCarriers());  
  }
  
  void OnTriggerExit2D(Collider2D other)
  {
    _freightAreaDataTreatedColliders.Remove(other);  
  }
  
  public void EnqueueCarrierToSend(ResourceCarrier carrier)//pré= le carrier ne doit PAS être en cours de mouvement.
  {
  	MoveManager moveManager=carrier.GetComponent<MoveManager>();
  	
  	RoadData occupiedRoad=carrier.GetComponent<RoadRouteManager>().occupiedRoad;
  	if(occupiedRoad!=null) //Elle vaudra null si c'est un nouveau carrier qui démarre seulement
  	  occupiedRoad.roadLock.UnlockFor(moveManager.orientation,carrier.gameObject);
  
  	if(carrier.destination!=null)//Si la destination vaut null, c'est qu'elle a été détruite
  	{
  	  carrier.GetComponent<Collider2D>().enabled=false;
      _waitingCarriers.Enqueue(carrier);
    }
    else
      GameManager.instance.DestroyGameObject(carrier.gameObject);
  }
  
  private IEnumerator SendWaitingCarriers()
  {
    while(true)
    {
      if(_waitingCarriers.Count>0 && road.roadLock.IsFree(Orientation.SOUTH))//TODO orientation
      {
        ResourceCarrier candidate=_waitingCarriers.Dequeue();
        ResourceCarrier firstCandidate=candidate;
        bool firstOk=true;

        while(RoadsPathfinding.RouteStar(road,candidate.destination.freightAreaData.freightAreaIn.road,10,Orientation.SOUTH)==null && firstCandidate!=candidate)//TODO orientation (et attention avec in/out :/)
        {
          firstOk=false;
          _waitingCarriers.Enqueue(candidate);
          candidate=_waitingCarriers.Dequeue();
          
          yield return new WaitForSeconds(0.1f);
        }//TODO ATTENTION: la boucle ci-dessus veut dire que si le joueur fait n'importe quoi et coupe des chemins partout, la taille de la file va augmenter alors que des transporteurs y restent coincés!!!! Du coup, au moins mettre un genre de message d'erreur pourrait être sympa.==> TODO attention aussi au cas des destruction: que deviennent les carriers qui devaient aller à un bâtiment qui n'existe plus ? Comparer avec null pour vérifier que pas détruits ??? OK sur les children ?
        
        if(firstOk || firstCandidate!=candidate)//Donc, on a trouvé un transporteur à envoyer
        {
          candidate.transform.position=new Vector3(road.transform.position.x,road.transform.position.y,candidate.transform.position.z);
          MoveManager candidateMoveManager=candidate.GetComponent<MoveManager>();
          candidateMoveManager.orientation=Orientation.SOUTH;//TODO orientation
          candidate.GetComponent<RoadRouteManager>().occupiedRoad=road;
          road.roadLock.LockFor(candidateMoveManager.orientation,candidate.gameObject);
          candidate.GetComponent<Collider2D>().enabled=true;
          candidate.GetComponent<RoadRouteManager>().MoveTo(candidate.destination.freightAreaData.freightAreaIn.road);
        }
      }
    
      yield return new WaitForSeconds(0.5f);	
    }
  }
}