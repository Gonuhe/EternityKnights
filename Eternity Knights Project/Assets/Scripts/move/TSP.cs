using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TSP 
{


  //TODO pour l'instant, pas de TSP, c'est un algo qui établi un itinéraire en prenant à chaque fois le gameObject le plus proche à vol d'oiseau du précédent.
  /**
   * Pré : origin doit avoir un RoadData
   * */
  public static PriorityQueue<int, GameObject> GetItinary(GameObject origin, List<GameObject> toVisit)
  {
    PriorityQueue<int, GameObject> rslt = new PriorityQueue<int, GameObject>();


    GameObject previous = origin;
    int i = 0;
    while(toVisit.Count > 0)
    {
      GameObject nearest = GetNearestObject(previous, toVisit);

      if(previous.GetComponentInChildren<RoadData>() != null
        && nearest.GetComponentInChildren<RoadData>() != null
        && RoadsPathfinding.RealDistanceBetween(previous.GetComponentInChildren<RoadData>(), nearest.GetComponentInChildren<RoadData>()) != -1)//On checke qu'il y a quand même bien un passage
      {
        rslt.Push(i, nearest);
        previous = nearest;
        i++;
      }
      toVisit.Remove(nearest);
    }
    return rslt;
  }

  private static GameObject GetNearestObject(GameObject origin, List<GameObject> gameObjects)
  {
    float minDistance = Mathf.Infinity;
    GameObject nearest = null;
    foreach(GameObject nearObject in gameObjects)
    {
      float distance = Vector2.Distance(nearObject.transform.position, origin.transform.position);
      if(distance < minDistance)
      {
        minDistance = distance;
        nearest = nearObject;
      }
    }
    return nearest;
  }
}
