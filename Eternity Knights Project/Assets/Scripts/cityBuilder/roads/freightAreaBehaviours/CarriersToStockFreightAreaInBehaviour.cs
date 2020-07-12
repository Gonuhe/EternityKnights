using UnityEngine;

/**
* Script à lier à un bâtiment pour que sa FreightAreaIn gère les ResourceCarriers entrant 
* - en ajoutant leur contenu à son stock si possible, et les renvoyant sinon
* - en acceptant tout chargement qui reviendrait à son point d'origine et détruisant
* le transporteur associé.
**/
public class CarriersToStockFreightAreaInBehaviour : FreightAreaInBehaviour
{
  protected override void OnFreightAreaEntered(Collider2D other)
  {
  	ResourceCarrier carrier=other.GetComponent<ResourceCarrier>();

    if(carrier!=null && !carrier.collectingResources && carrier.destination==freightAreaData.parentStock)
    {      
      if(carrier.origin!=freightAreaData.parentStock)
      {
      	if(freightAreaData.parentStock.stockLock.AvailableFor(carrier.shipment))
      	  freightAreaData.AddShipmentToStockAndSendBack(carrier);
        else
          carrier.OnShipmentRefused();
      }
      else
        freightAreaData.AddShipmentToStockAndDestroy(carrier);
    } 
  }
}