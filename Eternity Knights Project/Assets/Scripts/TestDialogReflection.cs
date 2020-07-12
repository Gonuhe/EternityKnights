using UnityEngine;
using System.Collections;

public class TestDialogReflection:DialogPostAction
{

  public int advancedInt;

  public void kikoo()
  {
    Debug.Log("it works");
  }

  public void advancedKikoo()
  {
    Debug.Log("it works again "+advancedInt);
  }

  public void anotherTest()
  {
    Debug.Log("là");
  }

}
