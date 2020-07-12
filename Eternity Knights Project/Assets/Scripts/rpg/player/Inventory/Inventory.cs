using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory 
{

  private Dictionary<Item, int> _items = new Dictionary<Item,int>();

  public void AddItem(int id, int quantity)
  {
    Item item = ItemDB.instance.GetItem(id);


    int previousQuantity;
    if(_items.TryGetValue(item, out previousQuantity))
    {
      _items.Remove(item);
      _items.Add(item, quantity+previousQuantity);
    }
    else
      _items.Add(item, quantity);
    
  }

  public bool RemoveItem(int id, int quantity)
  {
    Item item = ItemDB.instance.GetItem(id);


    int previousQuantity;
    if(_items.TryGetValue(item, out previousQuantity))
    {
      if(quantity<=previousQuantity)
      {
        _items.Remove(item);
        _items.Add(item, previousQuantity-quantity);
        return true;
      }
      else return false;
    }
    else
      return false;
  }

  public int GetQuantity(int id)
  {
    int previousQuantity;
    if(_items.TryGetValue(ItemDB.instance.GetItem(id), out previousQuantity))
      return previousQuantity;
    else
      return -1;
  }

  public void AddItem(int id)
  {
    AddItem(id,1);
  }

  //TODO pas safe !
  public Dictionary<Item,int> GetItems()
  {
    return _items;
  }

  public bool HaveItem(int id)
  {
    Item item = ItemDB.instance.GetItem(id);

    return _items.ContainsKey(item);
  }
}
