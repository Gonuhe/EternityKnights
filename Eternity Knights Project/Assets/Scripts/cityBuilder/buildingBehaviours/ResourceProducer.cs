using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Attribut à attacher à tout bâtiment produisant des ressources.
**/
[RequireComponent(typeof(OrderManager))]
public class ResourceProducer : ResourceConsumer
{
  /*
  Nombre de fois que les requirements de production peuvent être contenus 
  dans le stock (la limite de stock est modifiée en conséquence dans Awake, pas 
  besoin de faire plus)
  */
  public int productionBufferSize;
  
  [HideInInspector]
  public OrderManager orderManager;
  
  //Liste de tout ce qui est produit en un seul coup par le consumptionEvent dont hérite ce ResourceProducer
  public ResourceShipment[] production;
 
  private Queue<ResourceOrder> _warehouseOrders=new Queue<ResourceOrder>();
  private const int WAREHOUSE_ORDERS_LIMIT=20;
  
  protected override void Awake()
  {
  	base.Awake();  	
    consumptionEvent.AddListener(GenerateResources);
    orderManager=GetComponent<OrderManager>();
    
    foreach(ResourceShipment requirement in requirements)
    {
      orderManager.unOrderableResources.Add(requirement.resourceName);//Pour éviter qu'on puisse commander au producteur les ressources dont il a besoin	
    }
  }
  
  protected override void Start()
  {
  	base.Start();
  	
  	foreach(ResourceShipment productionShipment in production)
    {
     _currentStock.stockLock.totalStockLimit+=productionShipment.amount*productionBufferSize;
    }
  	
    StartCoroutine(ManageProductionStorage());
    StartCoroutine(ManageWarehouseDispatching());
  }
  
  protected override bool RequirementsMet()
  {
  	if(!base.RequirementsMet()) return false;
  	  
  	foreach(ResourceShipment shipment in production)
  	{
  	  if(!_currentStock.stockLock.AvailableFor(shipment)) return false;
  	}
  	
  	FreightAreaData freightData=_currentStock.freightAreaData;
  	int carrierCapacity=freightData.carrierCapacity;
  	
  	return  (_currentStock.totalStock
  				+ (freightData.carriersNber)*carrierCapacity
  				- (freightData.availableCarriers)*carrierCapacity ) 
  			<= _currentStock.stockLock.totalStockLimit;
  }
  
  /**
  * Génère et ajoute au stock du bâtiment un exemplaire complet de toute sa production.
  * ATTENTION: Cette fonction ne vérifie pas la capacité du stock. Il faut d'abord 
  * la vérifier via son StockLock.
  **/
  private void GenerateResources()
  {
  	foreach(ResourceShipment produced in production)
  	{
      _currentStock.AddToStock(produced.resourceName,produced.amount);
    }
  }
  
  /**
  * Coroutine se chargeant d'envoyer tout ce qui a été produit par le bâtiment
  * et se trouve contenu dans son stock à l'entrepôt le plus proche capable 
  * de l'accueillir ou à un bâtiment l'ayant commandé. Tout cela se voit bien 
  * entendu diviser entre les transporteurs
  * disponibles pour le bâtiment.
  **/
  private IEnumerator ManageProductionStorage()
  {
  	FreightAreaData freightData=orderManager.freightAreaData;
  	
    while(true)
    {
      if(!orderManager.SendOrderIfPossible() && !orderManager.OrdersToTreat() && _warehouseOrders.Count>0)//L'appel à la méthode envoie une commande si elle retourne true, d'où l'aspect un peu étrange de la condition
      {
        ResourceOrder warehouseOrder=_warehouseOrders.Dequeue();
        ResourceShipment orderShipment=warehouseOrder.shipment;
        
        FreightAreaIn destinationIn=warehouseOrder.deliveryPlace.freightAreaData.freightAreaIn;
        if(orderManager.stock.StockFor(orderShipment.resourceName)>=orderShipment.amount && RoadsPathfinding.RouteStar(destinationIn.road,freightData.freightAreaOut.road,10,Orientation.SOUTH)!=null)//TODO orientation
        {
          _currentStock.RemoveFromStock(orderShipment.resourceName,orderShipment.amount);
          freightData.SendCarrier(warehouseOrder.deliveryPlace,Orientation.SOUTH,orderShipment);//TODO orientation
        }
      }
      
      yield return new WaitForSeconds(0.5f); 
    }
  }
    	
  private IEnumerator ManageWarehouseDispatching()
  {
  	FreightAreaData freightData=_currentStock.freightAreaData; 
  	RoadLock freightOutLock=freightData.freightAreaOut.road.roadLock;
  	
    while(true)
    {
      if(!orderManager.SendOrderIfPossible() && _warehouseOrders.Count<=WAREHOUSE_ORDERS_LIMIT)//L'appel à la méthode envoie une commande si elle retourne true, d'où l'aspect un peu étrange de la condition
      {	  
        foreach(ResourceShipment productionShipment in production) //TODO: si on produit plusieurs ressources et qu'on veut définir l'ordre dans lequel elles partent, c'est ici
        {
          int resourceStock=_currentStock.StockFor(productionShipment.resourceName);
        
          for(int i=0;i<freightData.availableCarriers && resourceStock>0;i++)
          {
            int amountToShip=Math.Min(freightData.carrierPrefab.capacity,resourceStock);
            ResourceShipment toShip=new ResourceShipment(productionShipment.resourceName,amountToShip);
            Cell<FreightAreaIn> nestedCoroutineReturnHelper=new Cell<FreightAreaIn>();
      
            yield return StartCoroutine(freightData.freightAreaOut.road.FindNearestFreeWarehouseFor(toShip,nestedCoroutineReturnHelper,new HashSet<FreightAreaIn>()));
          
            FreightAreaIn nearestFreeWarehouse=nestedCoroutineReturnHelper.value;
            
            if(nearestFreeWarehouse!=null)
            {
              while(!freightOutLock.IsFree(Orientation.SOUTH))//TODO orientation
              {
                yield return new WaitForSeconds(0.2f);
              }
                    	
              _warehouseOrders.Enqueue(new ResourceOrder(new ResourceShipment(productionShipment.resourceName,amountToShip),nearestFreeWarehouse.GetComponentInParent<BuildingStock>()));              
            }
            
            resourceStock=_currentStock.StockFor(productionShipment.resourceName);
          }
        }
      }
      
      yield return new WaitForSeconds(0.5f);
    }
  }
}