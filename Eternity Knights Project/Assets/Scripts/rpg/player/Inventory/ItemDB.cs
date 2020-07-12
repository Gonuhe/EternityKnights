using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDB
{

  public static ItemDB instance = new ItemDB();


  private int _itemCount;
  private Dictionary<int,Item> _items = new Dictionary<int, Item>();

  private static Sprite[] _allSpritesIcons;
  private const string _iconsFolder = "/Test/Inventory/";



  /**
   * Ajoute l'élément à la DB.
   * Renvoie False si l'élément n'a pas pu etre ajouté (par exemple : un élément existait déjà avec cet id).
   **/
  public bool addItem(Item item)
  {
    Item i = new Item();
    if(!_items.TryGetValue(item.itemId, out i))
    {
      _items.Add(item.itemId,item);
      return true;
    }
    return false;
  }

  public Item GetItem(int id)
  {
    return _items[id];
  }

  public static Sprite getIcon(int itemId)
  {
    if(_allSpritesIcons == null)
      _allSpritesIcons = Resources.LoadAll<Sprite>("Test/Inventory/icons"); // pour pouvoir créer des items dans awake. et rendre cette fonction static
    return _allSpritesIcons[itemId];
  }

}
