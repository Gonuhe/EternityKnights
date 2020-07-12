using UnityEngine;
using System.Collections;

public class InventoryTrigger : MonoBehaviour
{
  private bool visible = false;

	// Use this for initialization
	void Start ()
  {
	}
	
	// Update is called once per frame
	void Update ()
  {
	  if(Input.GetButtonDown("Inventory"))
    {
      showInventoryUIPanel(!visible);
    }
	}

  void showInventoryUIPanel(bool b)
  {
    if(b)
    {
      GameManager.instance.StackGameMode(GameModes.INVENTORY_MODE);
    }
    else
    {
      GameManager.instance.UnstackGameMode(GameModes.INVENTORY_MODE);
    }
    visible = b;
  }


}
