using UnityEngine;

/**
ATTENTION:
Pour les agoras, à la différence des autres sous-classes de FreightAreaInBeghaviour,
cette classe doit être attachée DIRECTEMENT à l'agora, et pas aux marcands!!
En effet, les commandes laissées de côté à un agora peuvent être étalées entre
ses marchands, et donc entre plusieurs stocks. La seule chose qui a donc du sens
est d'utiliser le stock de l'agora comme destination des transporteurs, d'où
le fait que ce script s'attache directement à l'agora.
**/
public class KeepAsideManagementFreightAreaInBehaviour : FreightAreaInBehaviour
{  
  protected override void OnFreightAreaEntered(Collider2D other)
  {
  	ResourceCarrier carrier=other.GetComponent<ResourceCarrier>();
  	
  	if(carrier!=null && carrier.collectingResources && carrier.destination==freightAreaData.parentStock)
  	{
  	  KeepAsideOrderManager keepAsideOrderManager=freightAreaData.parentStock.GetComponent<KeepAsideOrderManager>();
  	  
  	  carrier.shipment=keepAsideOrderManager.DeliverKeptAside(carrier);
  	  freightAreaData.SendBack(carrier);
  	}
  }
}