using UnityEngine;
using System.Collections;

/**
* Classe définissant les attributs communs à tous les bâtiments.
**/
[RequireComponent(typeof(BoxCollider2D))]
public class PlacementData : MonoBehaviour 
{
  public int unitsHeight;
  public int unitsWidth;
  public int cost;


  private bool _destroying = false;

  /**
   * Renvoie true si des sprites sont actuellement en train de marcher sur les routes à l'intérieur de la maison.
   **/
  public bool HasPeopleOnView()
  {
    GameObject roads = GetRoads();
    if(roads != null)
    {
      RoadData[] roadsRD = roads.GetComponentsInChildren<RoadData>();
      foreach(RoadData road in roadsRD)
      {
        if(!road.roadLock.IsFree())
          return true;
      }
    }
    return false;
  }

  private GameObject GetRoads()
  {
    Transform[] children = GetComponentsInChildren<Transform>();
    foreach(Transform child in children)
    {
      if(child.name == "Roads")
        return child.gameObject;
    }
    return null;
  }

  public void SetDestroying(bool b)
  {
    SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
    _destroying = b;
    if(sprites != null)
    {
      if(_destroying)
      {
        foreach(SpriteRenderer sprite in sprites)
          sprite.color = Color.red ;
        StartCoroutine(SafeBuildingDestroy());
      }
      else
      {
        foreach(SpriteRenderer sprite in sprites)
          sprite.color = Color.white ;
      }
    }
  }

  /**
   * Coroutine qui va essayer de supprimer un bâtiment et qui réessayera jusqu'à y parvenir
   **/
  public IEnumerator SafeBuildingDestroy()
  {
    while(true)
    {
      if(!HasPeopleOnView())
      {
        GameManager.instance.DestroyGameObject(gameObject);
        break;
      }
      else
        yield return new WaitForSeconds(0.5F);
    }
  }


}
