using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Agora))]
public class AgoraOrderManager : KeepAsideOrderManager
{
  private Agora _agora;
	
  protected new void Awake()
  {
    base.Awake();
    _agora=GetComponent<Agora>();
  }
  
  public override int Order(string resourceName,int orderedAmount,BuildingStock deliveryPlace)
  {
  	int leftToOrder=orderedAmount;
  	IEnumerator<AgoraMerchant> merchantsEnumerator=_agora.MerchantsEnumerator();
    while(merchantsEnumerator.MoveNext())
    {
      AgoraMerchant merchant=merchantsEnumerator.Current;
      leftToOrder-=merchant.orderManager.Order(resourceName,leftToOrder,deliveryPlace);
      
      if(leftToOrder==0) return orderedAmount;
    }
    
    return orderedAmount-leftToOrder;
  }
  
  public override int OrderedAmountFor(string resourceName)
  {
  	int count=0;
  	IEnumerator<AgoraMerchant> merchantsEnumerator=_agora.MerchantsEnumerator();
    while(merchantsEnumerator.MoveNext())
    {
      AgoraMerchant merchant=merchantsEnumerator.Current;
      count+=merchant.orderManager.OrderedAmountFor(resourceName);
    }	
    
    return count;
  }
  
  public override int MakeKeepAsideOrder(string resourceName,int orderedAmount,ResourceCarrier recipient)
  {
  	int leftToOrder=orderedAmount;
  	IEnumerator<AgoraMerchant> merchantsEnumerator=_agora.MerchantsEnumerator();
    while(merchantsEnumerator.MoveNext())
    {
      AgoraMerchant merchant=merchantsEnumerator.Current;
      KeepAsideOrderManager merchantKeepAside=merchant.GetComponent<KeepAsideOrderManager>();
      if(merchantKeepAside!=null)
        leftToOrder-=merchantKeepAside.MakeKeepAsideOrder(resourceName,leftToOrder,recipient);
      
      if(leftToOrder==0) return orderedAmount;
    }
    
    return orderedAmount-leftToOrder;
  }
  
  public void DeliverKeptAsideOrderTo(ResourceCarrier recipient)
  {
  	ResourceShipment orderedShipment=null;
  	IEnumerator<AgoraMerchant> merchantsEnumerator=_agora.MerchantsEnumerator();
    while(merchantsEnumerator.MoveNext())
    {
      AgoraMerchant merchant=merchantsEnumerator.Current;
      KeepAsideOrderManager merchantOrderManager=merchant.GetComponent<KeepAsideOrderManager>();
    
      if(merchantOrderManager!=null)
      {
        if(orderedShipment!=null)
          orderedShipment.AddShipment(merchantOrderManager.DeliverKeptAside(recipient));
        else
          orderedShipment=merchantOrderManager.DeliverKeptAside(recipient);
      }
    }
    
    recipient.shipment=orderedShipment;
  }  
  
  public override bool CanMakeKeepAsideOrder(string resourceName)
  {
  	IEnumerator<AgoraMerchant> merchantsEnumerator=_agora.MerchantsEnumerator();
    while(merchantsEnumerator.MoveNext())
    {
      AgoraMerchant merchant=merchantsEnumerator.Current;
      KeepAsideOrderManager merchantKeepAside=merchant.GetComponent<KeepAsideOrderManager>();
      
      if(merchantKeepAside!=null && merchantKeepAside.CanMakeKeepAsideOrder(resourceName)) 
        return true;	
    }
    
    return false;
  }
  
  public override void CancelKeptAside(ResourceCarrier carrier)
  {
  	IEnumerator<AgoraMerchant> merchantsEnumerator=_agora.MerchantsEnumerator();
    while(merchantsEnumerator.MoveNext())
    {
      AgoraMerchant merchant=merchantsEnumerator.Current;
      KeepAsideOrderManager merchantKeepAside=merchant.GetComponent<KeepAsideOrderManager>();
      
      if(merchantKeepAside!=null) 
        merchantKeepAside.CancelKeptAside(carrier);	
    }
  }
}