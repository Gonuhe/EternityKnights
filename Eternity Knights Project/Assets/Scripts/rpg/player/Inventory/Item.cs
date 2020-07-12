using UnityEngine;
using System.Collections;

public class Item {


  public string itemName;
  public int itemId;
  public string itemDesc;
  public ItemType itemType;
  public Sprite itemIcon;

  public enum ItemType
  {
    Weapon,
    Potion,
    Head,
    Chest,
    Accessory,
    Artefact,
  }


  public Item()
  {

  }

  /**
   * TODO : ne pas utiliser, mieux vaut les faire via des préfabs pour pouvoir éditer facilement les caractéristiques.
   * à supprimer ?
   **/
  public Item(string name, int id, string desc, ItemType type)
  {
    itemName = name;
    itemId = id;
    itemDesc = desc;
    itemType = type;

    itemIcon = ItemDB.getIcon(id);//Doit aller chercher la bonne icone sur la sprite sheet, si on utilise une sprtiesheet !
  }
}
