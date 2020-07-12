using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour {

  private List<Slot> _slots = new List<Slot>();
  public GameObject slotPrefab;
  int x = -150;
  int y = 175;
  

  protected void Awake()
  {
    GameModes.INVENTORY_MODE.uIPanel = this;
  }

	// Use this for initialization
  protected void Start()
  {
    for(int i = 0; i<8;i++)
    {
      for(int j = 0; j<7;j++)
      {
        GameObject slot = (GameObject)Instantiate(slotPrefab);
        _slots.Add(slot.GetComponent<Slot>());
        slot.name = "Slot"+i+"."+j;
        slot.transform.SetParent(gameObject.transform);
        slot.GetComponent<RectTransform>().localPosition = new Vector3(x,y,0);
        x += 50;
      }
      x = -150;
      y -=50;
    }


    gameObject.SetActive(false);
	}

  public void Refresh()
  {
    Dictionary<Item, int> items =  GameManager.instance.RPGData.inventory.GetItems();
    int i = 0;
    foreach(KeyValuePair<Item,int> item in items)
    {
      _slots[i].setItem(item.Key);
      i++;
    }
  }

  //TODO Pas utilisé pour l'instant, mais mieux pour plus tard !
  public void AddItem(Item item)
  {
    Slot firstEmptySlot = findFirstEmptySlot();
    firstEmptySlot.setItem(item);
  }

  Slot findFirstEmptySlot()
  {
    for(int i = 0;i<_slots.Count;i++)
    {
      if(_slots[i].isEmpty())
        return _slots[i];
    }
    return null;
  }

}
