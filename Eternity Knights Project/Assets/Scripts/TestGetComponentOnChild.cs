using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResourceConsumer))]
public class TestGetComponentOnChild : MonoBehaviour 
{
  protected void Start() 
  {
	ResourceConsumer test=GetComponent<ResourceConsumer>();
	Debug.Log("TEST "+(test==null)); //==> Donc, il trouve bien le ResourceProducer (classe fille) attaché, c'est super :)
  }
	
  void Update() 
  {
  }
}
