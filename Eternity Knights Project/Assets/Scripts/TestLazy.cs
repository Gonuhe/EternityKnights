using UnityEngine;
using System.Collections;

public class TestLazy : MonoBehaviour 
{
  // Use this for initialization
  void Start () 
  {
    IEnumerator test=Test();
    Debug.Log("fzhoezhfozeji");
    test.MoveNext();
  }
  
  public IEnumerator Test()
  {
  	Debug.Log("tournicoti");
  	bool toto=false;
  	while(toto)
  	{
      Debug.Log("toto"); //===> DONC: pour créer l'énumérateur, on n'exécute même pas le début de la fonction!! Tout se fait au premier MoveNext!!
    
      yield return null;
    }
  }
}
