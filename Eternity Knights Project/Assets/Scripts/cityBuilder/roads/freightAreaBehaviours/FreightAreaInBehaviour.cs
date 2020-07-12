using UnityEngine;

[RequireComponent(typeof(FreightAreaData))]
public abstract class FreightAreaInBehaviour : MonoBehaviour
{
  protected FreightAreaIn freightAreaIn;
  protected FreightAreaData freightAreaData;
	
  protected void Awake()
  {
  	freightAreaData=GetComponent<FreightAreaData>();  
  }
  
  protected void Start()
  {
    freightAreaIn=freightAreaData.freightAreaIn;
    freightAreaIn.inEvent.AddListener(OnFreightAreaEntered);  
  }
  
  protected abstract void OnFreightAreaEntered(Collider2D other);
}