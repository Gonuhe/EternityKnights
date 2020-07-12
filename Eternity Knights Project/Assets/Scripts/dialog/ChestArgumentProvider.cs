using UnityEngine;
using System.Collections;

public class ChestArgumentProvider : ArgumentProvider
{

  private Chest _chest;

  void Start()
  {
    _chest = GetComponent<Chest>();
  }

  public override string GetArgument(int i)
  {
    if(i == 1)
      return   ItemDB.instance.GetItem(_chest.rewardId).itemName;
    if(i == 2)
      return ""+_chest.quantity;
    return "";
  }
}
