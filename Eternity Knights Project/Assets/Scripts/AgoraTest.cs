using UnityEngine;
using System.Collections;

public class AgoraTest : MonoBehaviour 
{
  public AgoraMerchant test;	
  public AgoraMerchant test2;

  protected void Awake()
  {
    AgoraMerchant toAdd=(GameManager.instance.InstantiatePrefabAsChild(gameObject,test.gameObject,transform.position) as GameObject).GetComponent<AgoraMerchant>();
    GetComponent<Agora>().AddMerchant(toAdd);
    
    AgoraMerchant toAdd2=(GameManager.instance.InstantiatePrefabAsChild(gameObject,test2.gameObject,transform.position) as GameObject).GetComponent<AgoraMerchant>();
    GetComponent<Agora>().AddMerchant(toAdd2);
  }
}
