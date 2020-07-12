using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
* Attribut à attacher à un prefab de PNJ pour en faire un transporteur, à
* savoir un livreur équipé d'une charette, chargé de livrer de la marchandise 
* d'un point à un autre de la ville.
**/
public class ResourceCarrier : MonoBehaviour
{
  //Chargement actuellement véhiculé par le transporteur
  //[HideInInspector] TODO DEBUG
  public ResourceShipment shipment;
  
  //Origine (bâtiment qui a généré le transporteur)
  [HideInInspector]
  public BuildingStock origin;
  
  //Emplacement où le transporteur doit se rendre
  //[HideInInspector] TODO DEBUG
  public BuildingStock destination;
  
  //Quantité maximale de ressources pouvant être contenue dans le transporteur
  public int capacity;
  
  //True ssi le transporteur provient d'un client et va chercher des ressources à une source quelconque (une agora étant l'exemple type)
  public bool collectingResources=false; 

  
  void Start()
  {
  }
  
  /**
  * Appelé quand le lieu que livre un transporteur refuse ce que celui-ci lui 
  * apporte ou que le transporteur se retrouve coincé trop longtemps sans 
  * pouvoir livrer sa marchandise (voir ResourceCarrierStuck)
  **/
  public void OnShipmentRefused(HashSet<FreightAreaIn> toIgnore)
  {
    StartCoroutine(RedirectAfterShipmentRefusal(toIgnore));
  }
  
  public void OnShipmentRefused()
  {
    StartCoroutine(RedirectAfterShipmentRefusal(new HashSet<FreightAreaIn>()));
  }
  
  private IEnumerator RedirectAfterShipmentRefusal(HashSet<FreightAreaIn> toIgnore)
  {
  	FreightAreaIn bestWarehouse=null;
  	  
  	for(int i=0;i<3 && bestWarehouse==null;i++) //TODO possibilité d'un peu de random sur ce 3, arbitraire
  	{
  	  if(i>0) yield return new WaitForSeconds(Random.Range(0.5f,3.0f-i));
      Cell<FreightAreaIn> coroutineRslt=new Cell<FreightAreaIn>();
      RoadData occupiedRoad=GetComponent<RoadRouteManager>().occupiedRoad;
    
      yield return StartCoroutine(occupiedRoad.FindNearestFreeWarehouseFor(shipment,coroutineRslt,toIgnore));    
      
      bestWarehouse=coroutineRslt.value;
    }
    
    RoadRouteManager routeManager=GetComponent<RoadRouteManager>();
    if(bestWarehouse!=null) 
    {
      routeManager.MoveTo(bestWarehouse.road);
      destination=bestWarehouse.GetComponentInParent<BuildingStock>();
    }
    else 
    {
      destination=origin;
      routeManager.MoveTo(destination.freightAreaData.freightAreaIn.road);
    }
    
    yield return null;
  }
  
  /**
  * Fonctiona appelée par le onStuckEvent du RoadRouteManager de ce ResourceCarrier.
  **/
  public void ResourceCarrierStuck(RetryData retryData)//ATTENTION: RetryData peut valoir null (si on essaie de se rendre à qqch de détruit)
  {
    if(collectingResources)
      CollectingResourceCarrierStuck(retryData);
    else 
      DeliveringResourceCarrierStuck(retryData);
  }

  public void CollectingResourceCarrierStuck(RetryData retryData)//ATTENTION: RetryData peut valoir null (si on essaie de se rendre à qqch de détruit)
  {
  	RoadRouteManager routeManager=GetComponent<RoadRouteManager>();
  	  
    if(destination==null && origin==null) //bâtiment de destination détruit, et l'origine est détruite
    {
      collectingResources=false;//Pour être sûr de ne pas refiler les ressources à l'autre maison et faire ainsi confusion avec son/ses transporteur(s)
      StartCoroutine(GoHome());//On part se faire éliminer dans la maison la plus proche.
    }
    else//Dans tous les autre cas, on retourne à l'origine (soit on s'obstine, soit on fait demi-tour)
    {
      if(destination!=origin && destination!=null) destination.GetComponent<KeepAsideOrderManager>().CancelKeptAside(this);
    	
      destination=origin;
      routeManager.MoveTo(destination.freightAreaData.freightAreaIn.road);	
    }
  }
  
  public void DeliveringResourceCarrierStuck(RetryData retryData)//ATTENTION: RetryData peut valoir null (si on essaie de se rendre à qqch de détruit)
  {
    RoadRouteManager roadRouteManager=GetComponent<RoadRouteManager>();
    
    if(destination!=null)//destination peut valoir null si le bâtiment a été détruit
    {
      ResourceConsumer consumer=destination.freightAreaData.parentStock.GetComponent<ResourceConsumer>();
    
      if(shipment!=null && consumer!=null && origin.freightAreaData.freightAreaIn!=destination)//Donc: si le transporteur est en livraison vers un ResourceConsumer et qu'il n'est pas déjà en cours de mouvement d'annulation de livraison
        consumer.CancelResourceOrder(shipment);
    }
        
    if(origin!=null)
    {
      destination=origin;
      roadRouteManager.MoveTo(destination.freightAreaData.freightAreaIn.road,Random.Range(0.5f,3.0f),null);
    }
    else
      StartCoroutine(GoHome());
  }
  
  /**
  * Déclenche (via une coroutine) le mouvement du transporteur vers la maison la 
  * plus proche (sur le réseau routier), où il sera détruit (utile pour se 
  * débarrasser des transporteurs dont le bâtiment principal est détruit). 
  * Le stock sera alors perdu. Si aucune maison n'est disponible, le 
  * transporteur ne bouge pas, et la coroutine va continuer d'attendre une 
  * maison vers laquelle se rendre.
  **/
  private IEnumerator GoHome()  
  {
  	RoadRouteManager roadRouteManager=GetComponent<RoadRouteManager>();
    while(true)
    {
      //TODO utiliser un findNearestObjectWithPath (Utils) pour être plus générique.
      GameObject[] homesArray=GameObject.FindGameObjectsWithTag("Home");
      
      float minDistance=0.0f;
      FreightAreaIn nearestHomeIn=null;
      foreach(GameObject home in homesArray)
      {
        FreightAreaIn homeIn=home.GetComponent<BuildingStock>().freightAreaData.freightAreaIn;
        
        float distance=RoadsPathfinding.RealDistanceBetween(homeIn.road,roadRouteManager.occupiedRoad);
        if(distance>0.0f && (nearestHomeIn==null || minDistance>distance))
        {
          minDistance=distance;
          nearestHomeIn=homeIn;
        }
        yield return null;
      }
      
      if(nearestHomeIn!=null)//Sinon, c'est qu'il n'y a AUCUNE maison satisfaisante, et la boucle principale se charge de relancer la recherche
      {
        destination=nearestHomeIn.GetComponentInParent<BuildingStock>();
        roadRouteManager.MoveTo(destination.freightAreaData.freightAreaIn.road,Random.Range(0.5f,3.0f),null);
        yield break;
      }
   	  
      yield return null;
    }
  }
  
}