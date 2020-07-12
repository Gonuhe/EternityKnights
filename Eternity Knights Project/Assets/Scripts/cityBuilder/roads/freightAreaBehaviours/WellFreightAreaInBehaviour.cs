using UnityEngine;
using System.Collections;

public class WellFreightAreaInBehaviour : FreightAreaInBehaviour
{
  public float waterDeliveryTime;
	
  protected override void OnFreightAreaEntered(Collider2D other)
  {
    ResourceCarrier carrier=other.GetComponent<ResourceCarrier>();
  	
  	if(carrier!=null && carrier.collectingResources && carrier.destination==freightAreaData.parentStock)
  	{
  	  StartCoroutine(DeliveryCoroutine(carrier));
  	} 
  }
  
  private IEnumerator DeliveryCoroutine(ResourceCarrier carrier)
  {
    KeepAsideOrderManager keepAsideOrderManager=freightAreaData.parentStock.GetComponent<KeepAsideOrderManager>();
  	
    yield return new WaitForSeconds(waterDeliveryTime);
    
  	ResourceShipment order=keepAsideOrderManager.DeliverKeptAside(carrier);
  	
  	carrier.shipment=order;
  	carrier.destination=carrier.origin;
  	carrier.GetComponent<RoadRouteManager>().MoveTo(carrier.destination.freightAreaData.freightAreaIn.road);
  }
}