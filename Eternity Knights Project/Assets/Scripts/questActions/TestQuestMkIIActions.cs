using UnityEngine;
using System.Collections;

public class TestQuestMkIIActions
{
  public void Cadoc()
  {
    Debug.Log("Non, pas les lapins ; les lapins c'est gentil...");  
  }
  
  public void CreateToto()
  {
  	GameManager.instance.InstantiatePrefab("Prefabs/Tests/Toto","Toto");
    Debug.Log("Toto créé");  
  }
  
  public void DestroyToto()
  {
    GameManager.instance.DestroyGameObject(GameObject.Find("Toto"));
    Debug.Log("Toto détruit");
  }
  
  public void OnTotoTalked()
  {
    GameManager.instance.RPGData.questManager.TryToIncrement("testMk2","test1","characterTalked",1);  
  }
  
  public void OnDuduTalked()
  {
  	GameManager.instance.RPGData.questManager.SetLocalNarrativeProperty("testMk2","DuduTalked",true);
    GameManager.instance.RPGData.questManager.TryToIncrement("testMk2","test2","talk",1);  
  }
  
  public void OnTontonTalked()
  {
  	GameManager.instance.RPGData.questManager.SetLocalNarrativeProperty("testMk2","TontonTalked",true);
    GameManager.instance.RPGData.questManager.TryToIncrement("testMk2","test2","talk",1);  
  }
  
  public void BLC()
  {
    Debug.Log("Papa a vu le fifi de lolo");
  }
  
  public void Marsupilami()
  {
    Debug.Log("Houba!");
  }
  
  public void Test3PreD()
  {
    GameManager.instance.StartCoroutine(WaitAndIncrement(10.0f,"test3_Dudu","counter"));
  }
  
  public void Test3PreT()
  {
    GameManager.instance.StartCoroutine(WaitAndIncrement(30.0f,"test3_Tonton","counter"));
  }
  
  private IEnumerator WaitAndIncrement(float duration,string step,string subStep)
  {
    yield return new WaitForSeconds(duration);
    GameManager.instance.RPGData.questManager.TryToIncrement("testMk2",step,subStep,1);  
  }
  
  public void Test3PostD()
  {
    Debug.Log("C'est pas faux!");  
  }
  
  public void Test3PostT()
  {
  	Debug.Log("Si on se faisait un petit Cul de Chouette ?");  
  }
}
