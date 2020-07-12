using UnityEngine;

public class HomeCarriersFreightAreaInBehaviour : FreightAreaInBehaviour
{  
  protected override void OnFreightAreaEntered(Collider2D other)
  {
    ResourceCarrier carrier=other.GetComponent<ResourceCarrier>();
    
    if(carrier!=null && carrier.destination==freightAreaData.parentStock)
    {
      if(carrier.collectingResources)
      {
      	if(carrier.shipment!=null)
      	  freightAreaData.GetComponent<Home>().CancelOrderToPick(carrier);
      
        freightAreaData.AddShipmentToStockAndDestroy(carrier);
      }
      else
      {
        freightAreaData.treatedColliders.Remove(other);
        freightAreaIn.road.roadLock.UnlockFor(carrier.GetComponent<MoveManager>().orientation,carrier.gameObject);
        GameManager.instance.DestroyGameObject(other.gameObject);
      }
    }
  }
}