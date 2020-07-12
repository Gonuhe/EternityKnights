using System;
using System.Collections.Generic;

public class KeepAsideOrderManager : OrderManager
{ 
  public float availabilityRange;
  	
  private Dictionary<ResourceCarrier,ResourceOrder> _keptAsideOrders=new Dictionary<ResourceCarrier,ResourceOrder>();

  protected new void Start()
  {
  	base.Start();
  	
    if(!ignoreInStockManager) 
      GameManager.instance.cityBuilderData.stockManager.AddToKeepAsideOrderManagers(this);
  }
  
  public override int OrderedAmountFor(string resourceName)
  {
  	int count=base.OrderedAmountFor(resourceName);
    
  	foreach(ResourceOrder keptAside in _keptAsideOrders.Values)
  	{
  	  ResourceShipment shipment=keptAside.shipment;
  	  if(shipment.resourceName==resourceName) 
  	    count+=shipment.amount;	
  	}
  		
  	return count;
  }
  
  public virtual int MakeKeepAsideOrder(string resourceName,int orderedAmount,ResourceCarrier recipient)
  {
    int availableStock= stock.StockFor(resourceName)-OrderedAmountFor(resourceName);
    int ordered= Math.Min(availableStock,orderedAmount);
    
    if(ordered>0)
      _keptAsideOrders[recipient]=new ResourceOrder(new ResourceShipment(resourceName,ordered),recipient.origin);
    
    return ordered;
  }
  
  public virtual bool CanMakeKeepAsideOrder(string resourceName)
  {
    return stock.StockFor(resourceName)-OrderedAmountFor(resourceName) > 0;  
  }
  
  public ResourceShipment DeliverKeptAside(ResourceCarrier carrier)
  {
    ResourceOrder order=null;
    if(_keptAsideOrders.TryGetValue(carrier,out order))
    {
      _keptAsideOrders.Remove(carrier);
      return order.shipment;
    }
    
    return null;
  }
  
  public virtual void CancelKeptAside(ResourceCarrier carrier)
  {
    if(_keptAsideOrders.ContainsKey(carrier)) _keptAsideOrders.Remove(carrier);
  }
}