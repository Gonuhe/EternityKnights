using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BuildingStock))]
public class OrderManager : MonoBehaviour
{
  /*
  Définit la quantité maximum de ressource qui peut être commandée à ce bâtiment 
  (au-delà, il refusera les commandes jusqu'à ce qu'il en ait livré une).
  */
  public int maxOrderedAmount;
  
  //stock du bâtiment auquel ce script est associé
  [HideInInspector]
  public BuildingStock stock;
  
  //FreightArea du bâtiment auquel ce script est associé 
  [HideInInspector]
  public FreightAreaData freightAreaData;
  
  //Permet de ne pas ajouter l'OrderManager au StockManager (notamment pour les marchands d'agora)
  public bool ignoreInStockManager=false;
  
  //Ressources qui ne peuvent pas être commandées à cet OrderManager, même si elles sont en stock
  public HashSet<string> unOrderableResources=new HashSet<string>();
  
  //File contenant, dans l'ordre, les différentes commandes à livrer par les livreurs de ce bâtiment
  private Queue<ResourceOrder> _orders=new Queue<ResourceOrder>();
  
  //Contient la quantité totale de ressources commandées dans _orders
  private int _totalOrderedAmount=0;
  
  protected void Awake()
  {
    stock=GetComponent<BuildingStock>();
    freightAreaData=GetComponent<FreightAreaData>();
  }
  
  protected void Start()
  {
    if(!ignoreInStockManager)
      GameManager.instance.cityBuilderData.stockManager.AddToOrderManagers(this);  //TODO: quand on détruit le bâtiment, l'en supprimer!!  
  }
  
  /**
  * Effectue une tentative de commande dans ce bâtiment pour la ressource resourceName,
  * dans la quantité orderedAmount, à livrer à deliveryPlace.
  *
  * Cette méthode retoune la quantité qui a pu être commandée au bâtiment, qui 
  * vaudra donc 0 si rien n'a été commandé. 
  * On a donc 0 <= orderedAmmount <= valeur de retour.
  **/
  public virtual int Order(string resourceName,int orderedAmount,BuildingStock deliveryPlace)
  {  
    int ordered=0;
    int availableStock=stock.StockFor(resourceName)-OrderedAmountFor(resourceName);
    int carrierCapacity=freightAreaData.carrierCapacity;
    while(!unOrderableResources.Contains(resourceName) && maxOrderedAmount>=_totalOrderedAmount && availableStock>0 && ordered!=orderedAmount)
    {
      int newOrder=Math.Min(Math.Min(availableStock,carrierCapacity),orderedAmount-ordered);
      ordered+=newOrder;
      availableStock-=newOrder;
      _totalOrderedAmount+=newOrder;
      _orders.Enqueue(new ResourceOrder(new ResourceShipment(resourceName,newOrder),deliveryPlace));
    }
    
    return ordered;
  }
  
  /**
  * Retourne la quantité de la ressource resourceName actuellement en commande
  * dans ce bâtiment.
  **/
  public virtual int OrderedAmountFor(string resourceName)
  {
  	int rslt=0;
    foreach(ResourceOrder order in _orders)
    {
      if(order.shipment.resourceName==resourceName)
        rslt+=order.shipment.amount;
    }
    
    return rslt;
  }
  
  /**
  * Retourne true ssi ce bâtiment a reçu des commandes qu'il n'a pas encore traitées.
  **/
  public bool OrdersToTreat()
  {
    return _orders.Count>0;  
  }
  
  /**
  * Retourne la prochaine commande à traiter, ou null, s'il n'y en a aucune.
  **/
  private ResourceOrder NextOrder()
  {
    if(!OrdersToTreat()) return null;
    
    return _orders.Dequeue();
  }
  
  private void MarkAsTreated(ResourceOrder order)
  {
    stock.RemoveFromStock(order.shipment);
    _totalOrderedAmount-=order.shipment.amount; 
  }
  
  public bool SendOrderIfPossible()
  {
  	FreightAreaOut freightOut=freightAreaData.freightAreaOut;
  	if(freightAreaData.availableCarriers>0 && OrdersToTreat() && freightOut.road.roadLock.IsFree(Orientation.SOUTH))//TODO orientation (opposée à celle du bâtiment)
    {
      ResourceOrder toTreat=NextOrder();
        
      FreightAreaIn destinationIn=toTreat.deliveryPlace.freightAreaData.freightAreaIn;
      if(RoadsPathfinding.RouteStar(destinationIn.road,freightOut.road,10,Orientation.SOUTH)!=null)//TODO orientation
      {	
        MarkAsTreated(toTreat);
        freightAreaData.SendCarrier(toTreat.deliveryPlace,Orientation.SOUTH,toTreat.shipment);//TODO orientation (opposée à celle du bâtiment)
        
        return true;
      }
      else //Si la destination n'est pas raccordée au réseau routier, on annule sa commande et on l'en notifie
        toTreat.deliveryPlace.freightAreaData.parentStock.GetComponent<ResourceConsumer>().CancelResourceOrder(toTreat.shipment);  
    }
    
    return false;
  }
}