using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public static class Utils
{
  public static List<GameObject> GetNearObjects(GameObject origin, string tag, int distance)
  {
    /**
     * Choix des tags car les autres solutions ont aussi leur limite ou sont plus complexes à coder :
     * - Conserver toutes les maisons dans une variable (cityBuilderData) en plus de consommer de la mémoire, devrait alors potentiellement etre fait pour n'importe quel batiment
     * car on aura sans doute besoin d'accéder à tous les batiment d'une catégorie, du coup le code de cityBuilderData serait très répétitif (accesseur pour chaque type de batiment)
     * Solution ? : Réflection sur les accesseurs;
     * - Layer : meme limites que les tags mais tag plus approprié ici.
     * - Chercher tous les gameObjects avec un component "home" ou "work", la fonction d'unity est dite non efficace (FindObjectOfType) et je pourrais la coder à la main et
     * itérer sur tous les gameObject mais ça me semble si simple que la fonction de unity en question fait d'office ça (et certainement mieux !)
     * - Créer un gameObject par catégorie de batiment et en faire le parent faire de chaque batiment de ce type ? => Peut-etre le mieux, nous permet une hiérarchie des
     * batiment qui pourra etre la meme que dans les scripts.
     **/
    GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
    List<GameObject>  nearObjects= new List<GameObject>();
    foreach(GameObject nearObject in objectsWithTag)
    {
      if(Vector2.Distance(nearObject.transform.position, origin.transform.position) < distance)
      {
        nearObjects.Add(nearObject);
      }
    }
    return nearObjects;
  }

  public static List<GameObject> GetNearObjects(GameObject origin, int layer, int distance)
  {
    Collider2D[] objects = Physics2D.OverlapCircleAll(origin.transform.position, (float)distance, 1 << layer);
    List<GameObject> objectsList = new List<GameObject>();
    for(int i = 0; i < objects.Length; i++)
      if(!objectsList.Contains(objects[i].gameObject))
        objectsList.Add(objects[i].gameObject);
    return objectsList;
  }

  public static GameObject GetNearestObject(GameObject origin, string tag)
  {
    return GetNearestObject(origin, tag, new HashSet<GameObject>());
  }

  public static GameObject GetNearestObject(GameObject origin, string tag, HashSet<GameObject> exclude)
  {
    GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
    float minDistance = Mathf.Infinity;
    GameObject nearest = null;
    foreach(GameObject nearObject in objectsWithTag)
    {
      float distance = Vector2.Distance(nearObject.transform.position, origin.transform.position);
      if((exclude == null || (exclude != null && !exclude.Contains(nearObject))) && distance < minDistance)
      {
        minDistance = distance;
        nearest = nearObject;
      }
    }
    return nearest;
  }


  //TODO faire une version avec une co-routine plus efficace (un pathfinding/frame)
  public static GameObject GetNearestObjectWithPath(RoadData origin, string tag, HashSet<GameObject> exclude)
  {
    GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
    float minDistance = Mathf.Infinity;
    GameObject nearest = null;
    foreach(GameObject nearObject in objectsWithTag)
    {      
      RoadData destinationRoadData = nearObject.GetComponentInChildren<FreightAreaIn>()==null ? null : nearObject.GetComponentInChildren<FreightAreaIn>().road;
      if(destinationRoadData != null  && (exclude == null || (exclude != null && !exclude.Contains(nearObject))))
      {
        float distance = RoadsPathfinding.RealDistanceBetween(destinationRoadData, origin);

        if(distance != -1.0f && distance < minDistance)
        {
          minDistance = distance;
          nearest = nearObject;
        }
      }
    }
    return nearest;
  }

  public static Collider2D[] GetFrontObjects(MoveManager _moveManager, float lookUpZoneSide)
  {
    Vector2 playerPosition=_moveManager.transform.position;

    Vector2 diagTop=Vector2.zero;
    Vector2 diagDown=Vector2.zero;

    switch(_moveManager.orientation)
    {
    case Orientation.NORTH:
      diagTop=new Vector2(playerPosition.x-lookUpZoneSide/2.0f,playerPosition.y+lookUpZoneSide);
      diagDown=new Vector2(playerPosition.x+lookUpZoneSide/2.0f,playerPosition.y);
      break;
    case Orientation.SOUTH:
      diagTop=new Vector2(playerPosition.x-lookUpZoneSide/2.0f,playerPosition.y);
      diagDown=new Vector2(playerPosition.x+lookUpZoneSide/2.0f,playerPosition.y-lookUpZoneSide);
      break;
    case Orientation.WEST:
      diagTop=new Vector2(playerPosition.x-lookUpZoneSide,playerPosition.y+lookUpZoneSide/2.0f);
      diagDown=new Vector2(playerPosition.x,playerPosition.y-lookUpZoneSide/2.0f);
      break;
    case Orientation.EAST:
      diagTop=new Vector2(playerPosition.x,playerPosition.y+lookUpZoneSide/2.0f);
      diagDown=new Vector2(playerPosition.x+lookUpZoneSide,playerPosition.y-lookUpZoneSide/2.0f);
      break;
    default://c'est qu'on est orienté en diagonale
      int yMultiplier= _moveManager.orientation.Contains(Orientation.NORTH) ? 1 : -1;
      int xMultiplier= _moveManager.orientation.Contains(Orientation.EAST) ? 1 : -1;
      diagTop=new Vector2(playerPosition.x+lookUpZoneSide*xMultiplier,playerPosition.y+lookUpZoneSide*yMultiplier);
      diagDown=playerPosition;
      break;
    }

    return Physics2D.OverlapAreaAll(diagTop,diagDown);
  }

  /**
  * Permet de comparer deux floats à une certaine valeur près, évitant ainsi
  * les erreurs d'arrondis.
  **/
  public static bool FloatComparison(float a,float b,float tolerance)
  {
    return Mathf.Abs(a-b)<tolerance;
  }
  
  public static void ExecuteNotAttachedScriptFunction(string scriptName, string functionName)
  {//TODO sécuriser un peu tout ça, envoyer des exceptions si la méthode n'existe pas
    Type t = Type.GetType(scriptName);
    MethodInfo function = t.GetMethod(functionName);
    object o = Activator.CreateInstance(t);
    function.Invoke(o, null);
  }

  public static GameObject FindGameObjectWithName(Component[] gameObjects, string name)
  {
    for(int i = 0; i < gameObjects.Length; i++)
    {
      if(gameObjects[i].gameObject.name == name)
        return gameObjects[i].gameObject;
    }
    return null;
  }
}
