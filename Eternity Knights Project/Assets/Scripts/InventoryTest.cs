using UnityEngine;
using System.Collections;

public class InventoryTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
    Item t = new Item("sword", 1, "a fine weapon", Item.ItemType.Weapon);
    ItemDB.instance.addItem(t);
    ItemDB.instance.addItem(new Item("shield", 2, "a fine shield", Item.ItemType.Accessory));
    GameManager.instance.RPGData.inventory.AddItem(1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
