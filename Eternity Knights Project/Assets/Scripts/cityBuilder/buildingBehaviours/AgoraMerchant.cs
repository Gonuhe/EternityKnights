using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(OrderManager))]
public class AgoraMerchant : MonoBehaviour
{
  [HideInInspector]
  public OrderManager orderManager;
	
  protected void Awake()
  {
    orderManager=GetComponent<OrderManager>();
    Agora parentAgora=GetComponentInParent<Agora>();
    ConnectFreightTo(parentAgora);
  }
  
  public void ConnectFreightTo(Agora agora)
  {
  	FreightAreaData agoraFreight=agora.GetComponent<FreightAreaData>();
  	FreightAreaIn agoraAreaIn=agoraFreight.freightAreaIn;
  	FreightAreaOut agoraAreaOut=agoraFreight.freightAreaOut;
  	HashSet<Collider2D> agoraTreatedColliders=agoraFreight.treatedColliders;
  	
    FreightAreaData freightData=GetComponent<FreightAreaData>();    
    freightData.freightAreaIn=agoraAreaIn;
    freightData.freightAreaOut=agoraAreaOut;
    freightData.treatedColliders=agoraTreatedColliders;
  }
}