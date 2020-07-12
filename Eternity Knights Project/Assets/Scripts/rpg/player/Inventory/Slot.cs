using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerDownHandler
{


  private Item _item;
  private Image _itemImage;

  void Awake()
  {
    _itemImage = gameObject.transform.GetChild(0).GetComponent<Image>();
  }

	// Use this for initialization
	void Start ()
  {
	}
	
	// Update is called once per frame
	void Update ()
  {

	}

  public void setItem(Item item)
  {
    if(item != null)
    {
      _item = item;
      _itemImage.sprite = item.itemIcon;
    }
  }

  public void removeItem()
  {
    _item = null;
  }

  public bool isEmpty()
  {
    return _item == null;
  }

  /**
   * TODO
   */
  public void OnPointerDown(PointerEventData data)
  {
    if(_item != null)
      Debug.Log(_item.itemName);
  }
}
