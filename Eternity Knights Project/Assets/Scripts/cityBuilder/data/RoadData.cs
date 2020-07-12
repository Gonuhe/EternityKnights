using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
* Classe définissant les attributs d'une route.
**/
[RequireComponent(typeof(RoadLock))]
[RequireComponent(typeof(RoadSpriteModifier))]
public class RoadData : MonoBehaviour
{
  //Routes voisines
  public RoadData eastNeighboringRoad;
  public RoadData westNeighboringRoad;
  public RoadData northNeighboringRoad;
  public RoadData southNeighboringRoad;
  
  //Gère l'accessibilité de cette route
  public RoadLock roadLock=new RoadLock();
  
  private RoadSpriteModifier _spriteModifier;
  
  protected void Awake()
  {
    _spriteModifier=GetComponent<RoadSpriteModifier>();
  }
  
  protected void Start()
  {
  	//Important pour les réseaux routiers dans des prefabs
    if(eastNeighboringRoad!=null) eastNeighborDetected(eastNeighboringRoad.gameObject);
    if(westNeighboringRoad!=null) westNeighborDetected(westNeighboringRoad.gameObject);
    if(northNeighboringRoad!=null) northNeighborDetected(northNeighboringRoad.gameObject);
    if(southNeighboringRoad!=null) southNeighborDetected(southNeighboringRoad.gameObject);  
    
    AdaptToNeighborhood();
  }
  
  protected void OnDestroy()
  {
    if(eastNeighboringRoad!=null) DetachEastNeighbor();
    if(westNeighboringRoad!=null) DetachWestNeighbor();
    if(northNeighboringRoad!=null) DetachNorthNeighbor();
    if(southNeighboringRoad!=null) DetachSouthNeighbor();
  }
  
  public void AdaptToNeighborhood() //Lié via l'inspecteur au PlacementListener, sur placementDone
  {
    _spriteModifier.CheckNeighborhood();  
  }
  
  public void DetachWestNeighbor()
  {
  	RoadData detached=westNeighboringRoad;
    westNeighboringRoad=null;
    
    if(detached.eastNeighboringRoad!=null)
    {
      detached.DetachEastNeighbor();
      detached.AdaptToNeighborhood();
    }
  }
  
  public void DetachEastNeighbor()
  {
  	RoadData detached=eastNeighboringRoad;
    eastNeighboringRoad=null;
    
    if(detached.westNeighboringRoad!=null)
    {
      detached.DetachWestNeighbor();
      detached.AdaptToNeighborhood();
    }
  }
  
  public void DetachSouthNeighbor()
  {
  	RoadData detached=southNeighboringRoad;
    southNeighboringRoad=null;
    
    if(detached.northNeighboringRoad!=null)
    {
      detached.DetachNorthNeighbor();
      detached.AdaptToNeighborhood();
    }
  }
  
  public void DetachNorthNeighbor()
  {
  	RoadData detached=northNeighboringRoad;
    northNeighboringRoad=null;
    
    if(detached.southNeighboringRoad!=null)
    {
      detached.DetachSouthNeighbor();
      detached.AdaptToNeighborhood();
    }
  }
  
  /**
  * Cette fonction est appelée par le PlacementListener associé à la route
  * si celui-ci lui découvre un voisin à l'est.
  **/
  //TODO: maj aux noms de fonctions!!
  public void eastNeighborDetected(GameObject neighbor) //Lié via l'inspecteur au PlacementListener
  {
  	RoadData neighborData=neighbor.GetComponent<RoadData>();
  	if(neighborData!=null)
    {
      eastNeighboringRoad=neighborData;
      
      if(eastNeighboringRoad.westNeighboringRoad==null)
      {
        eastNeighboringRoad.westNeighborDetected(gameObject);
        eastNeighboringRoad.AdaptToNeighborhood();
      }
    }
  }
  
  /**
  * Cette fonction est appelée par le PlacementListener associé à la route
  * si celui-ci lui découvre un voisin à l'ouest.
  **/
  public void westNeighborDetected(GameObject neighbor) //Lié via l'inspecteur au PlacementListener
  {
    RoadData neighborData=neighbor.GetComponent<RoadData>();
  	if(neighborData!=null)
    {
      westNeighboringRoad=neighborData;
      
      if(westNeighboringRoad.eastNeighboringRoad==null)
      {
        westNeighboringRoad.eastNeighborDetected(gameObject);
        westNeighboringRoad.AdaptToNeighborhood();
      }
    }
  }
  
  /**
  * Cette fonction est appelée par le PlacementListener associé à la route
  * si celui-ci lui découvre un voisin au nord.
  **/
  public void northNeighborDetected(GameObject neighbor) //Lié via l'inspecteur au PlacementListener
  {
    RoadData neighborData=neighbor.GetComponent<RoadData>();
  	if(neighborData!=null)
    {
      northNeighboringRoad=neighborData;
      
      if(northNeighboringRoad.southNeighboringRoad==null)
      {
        northNeighboringRoad.southNeighborDetected(gameObject);
        northNeighboringRoad.AdaptToNeighborhood();
      }
    }
  }
  
  /**
  * Cette fonction est appelée par le PlacementListener associé à la route
  * si celui-ci lui découvre un voisin au sud.
  **/
  public void southNeighborDetected(GameObject neighbor) //Lié via l'inspecteur au PlacementListener
  {
    RoadData neighborData=neighbor.GetComponent<RoadData>();
  	if(neighborData!=null)
    {
      southNeighboringRoad=neighborData;
      
      if(southNeighboringRoad.northNeighboringRoad==null)
      {
        southNeighboringRoad.northNeighborDetected(gameObject);
        southNeighboringRoad.AdaptToNeighborhood();
      }
    }
  }
  
  /**
  * Retourne une liste contenant toutes les routes voisines à cette route.
  **/
  public List<RoadData> Neighbors(bool includeNeverAccessible=true)
  {
    List<RoadData> rslt=new List<RoadData>();
    
    if(northNeighboringRoad!=null && (includeNeverAccessible || !northNeighboringRoad.roadLock.neverAccessible)) rslt.Add(northNeighboringRoad);
    if(southNeighboringRoad!=null && (includeNeverAccessible || !southNeighboringRoad.roadLock.neverAccessible)) rslt.Add(southNeighboringRoad);
    if(eastNeighboringRoad!=null && (includeNeverAccessible || !eastNeighboringRoad.roadLock.neverAccessible)) rslt.Add(eastNeighboringRoad);
    if(westNeighboringRoad!=null && (includeNeverAccessible || !westNeighboringRoad.roadLock.neverAccessible)) rslt.Add(westNeighboringRoad);
   
    return rslt;
  }
  
  /**
  * Retourne, en s'aidant du RoadLock associé, la liste des routes voisines à cette
  * route pour un objet mouvant (équipé d'un RoadRouteManager, donc) s'y trouvant 
  * dans l'orientation passée en paramètre. 
  **/
  public List<RoadData> AccessibleFreeNeighbors(string orientation)//orientation est donc l'orientation dans laquelle on est, qu'on verrouille déjà
  {
    List<RoadData> rslt=new List<RoadData>();
    
    //ICI========> TODO ====> C'est ça le problème: remettre des true dans ce qui suit et s'arranger pour ne PAS relock s'il y a déjà qqch dans l'orientation concernée sur la case ?? (dans le RoadRouteManager ????)
    //TODO DEBUG: j'ai tout passé à true ci-dessous. Ce morceau de code sera à revoir une fois les shifts intégralement débuggés
    bool canGoNorth= true;//(roadLock.split==RoadLock.VERTICAL_SPLIT && orientation==Orientation.NORTH) || roadLock.locksNber==1 || (roadLock.split==RoadLock.HORIZONTAL_SPLIT && orientation==Orientation.WEST);
    bool canGoSouth= true;//(roadLock.split==RoadLock.VERTICAL_SPLIT && orientation==Orientation.SOUTH) || roadLock.locksNber==1 || (roadLock.split==RoadLock.HORIZONTAL_SPLIT && orientation==Orientation.EAST);
    bool canGoEast= true;//(roadLock.split==RoadLock.HORIZONTAL_SPLIT && orientation==Orientation.EAST) || roadLock.locksNber==1 || (roadLock.split==RoadLock.VERTICAL_SPLIT && orientation==Orientation.NORTH);
    bool canGoWest= true;//(roadLock.split==RoadLock.HORIZONTAL_SPLIT && orientation==Orientation.WEST) || roadLock.locksNber==1 || (roadLock.split==RoadLock.VERTICAL_SPLIT && orientation==Orientation.SOUTH);
    
    bool northNeighborOk= northNeighboringRoad!=null && northNeighboringRoad.roadLock.IsFree(Orientation.NORTH);
    bool southNeighborOk= southNeighboringRoad!=null && southNeighboringRoad.roadLock.IsFree(Orientation.SOUTH);
    bool eastNeighborOk= eastNeighboringRoad!=null && eastNeighboringRoad.roadLock.IsFree(Orientation.EAST);
    bool westNeighborOk= westNeighboringRoad!=null && westNeighboringRoad.roadLock.IsFree(Orientation.WEST);
    
    if(northNeighborOk && canGoNorth) rslt.Add(northNeighboringRoad);
    if(southNeighborOk && canGoSouth) rslt.Add(southNeighboringRoad);
    if(eastNeighborOk && canGoEast) rslt.Add(eastNeighboringRoad);
    if(westNeighborOk && canGoWest) rslt.Add(westNeighboringRoad);
    
    return rslt;
  }
  
  /**
  * Coroutine à lancer pour découvrir la FreightArea de l'entrepôt le plus proche 
  * capable d'accueillir le chargement passé en paramètre.
  **/
  public IEnumerator FindNearestFreeWarehouseFor(ResourceShipment shipment,Cell<FreightAreaIn> rslt,HashSet<FreightAreaIn> toIgnore) 
  { 	
    Cell<Warehouse> nearestFree=new Cell<Warehouse>();
    Func<Warehouse,bool> acceptanceFunction= (Warehouse toTest)=>toTest.orderManager.stock.stockLock.AvailableFor(shipment);
    yield return StartCoroutine(StockManager.FindNearestWarehouseSatisfyingCondition(this,acceptanceFunction,nearestFree,toIgnore));
    
    if(nearestFree.value!=null)
      rslt.value=nearestFree.value.orderManager.freightAreaData.freightAreaIn;
  }
  
  /**
  * Coroutine à lancer pour trier selon leur distance (sur le réseau
  * routier) à cette route les bâtiments capable de recevoir des commandes.
  **/
  public IEnumerator SortOrderManagersByDistance(Cell<PriorityQueue<float,OrderManager>> rslt)
  {
  	PriorityQueue<float,OrderManager> rsltQueue=new PriorityQueue<float,OrderManager>();
  	StockManager stockManager=GameManager.instance.cityBuilderData.stockManager;
    IEnumerator<OrderManager> allOrderManagers=stockManager.AllOrderManagers();    
    while(allOrderManagers.MoveNext())
    {
      OrderManager orderManager=allOrderManagers.Current;
      
      if(orderManager!=null)
      {
        float realDistanceToOrderManager=RoadsPathfinding.RealDistanceBetween(this,orderManager.freightAreaData.freightAreaOut.road);
        if(realDistanceToOrderManager>0.0f)
          rsltQueue.Push(realDistanceToOrderManager,orderManager);
      }
      
      yield return null;
    }
    
    rslt.value=rsltQueue;
    
    yield return null;
  }
}