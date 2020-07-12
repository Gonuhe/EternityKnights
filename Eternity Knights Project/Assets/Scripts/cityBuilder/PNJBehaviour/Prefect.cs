using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prefect : MonoBehaviour
{

  public GameObject prefecture;

  private PriorityQueue<int, GameObject> _tour;

  public int effectZoneRadius = 200;

  private bool _continueTour=false;

  private int _tryAgainToVisitHome; // Pour visiter une maison, il insiste bien, pour éviter des effets bizarres par la suite.
  
  private int _trytoRevisitCount = 5; //Quand il a fini son tour, il réessaie les maisons qu'il n'a pas pu visiter 5 fois, puis il avandonne et rentre chez lui.

  FreightAreaIn goalFA;
  
  void Start()
  {
    StartCoroutine(goOnTour(Utils.GetNearObjects(prefecture, LayerMask.NameToLayer("buildings"), effectZoneRadius)));
  }

  public IEnumerator goOnTour(List<GameObject> toVisit)
  {
    if(prefecture != null)
    {
      List<GameObject> toReVisit = new List<GameObject>();
      toVisit.Remove(prefecture);
      _tour = TSP.GetItinary(prefecture, toVisit);
      GameObject _currentDestination;
      while(!_tour.IsEmpty())
      {
        _continueTour=false;
        _tryAgainToVisitHome = 50;
        
        _currentDestination = _tour.Pop();
        goalFA = _currentDestination.GetComponentInChildren<FreightAreaIn>();
        if(goalFA!= null && RoadsPathfinding.RealDistanceBetween(GetComponent<RoadRouteManager>().occupiedRoad, goalFA.road) != -1)
        //Il existe encore une route vers ce batiment
        {
          GetComponent<RoadRouteManager>().MoveTo(goalFA.road);
          yield return new WaitUntil(() => _continueTour || goalFA == null || gameObject.GetComponent<RoadRouteManager>().occupiedRoad == goalFA.road);
          if(goalFA != null && _continueTour)//On a pas pu atteindre la maison et on a choisi de la skipper
            toReVisit.Add(goalFA.gameObject);
          if(goalFA != null && !_continueTour)
          {
            Building building = _currentDestination.GetComponent<Building>();
            if(building != null)
              building.RestoreHealth();
            GetComponent<SpriteRenderer>().enabled = false;//TODO il est toujours sur la FA et donc le lock est toujours activé !
            yield return new WaitForSeconds(Random.Range(2.0f,5.0f));
            GetComponent<SpriteRenderer>().enabled = true;
          }
        }
      }
      if(toReVisit.Count != 0 && _trytoRevisitCount-- != 0)
        StartCoroutine(goOnTour(toReVisit));
      else
        StartCoroutine(GoBack());
    }
  }

  public IEnumerator GoBack()
  {
    RoadRouteManager roadRouteManager=GetComponent<RoadRouteManager>();
    while(true)
    {
      RoadData destination = null;
      if(prefecture != null)// elle existe encore
        destination = prefecture.GetComponent<Prefecture>().door;
      else //Elle n'existe plus
      {
        GameObject nearestHome = Utils.GetNearestObjectWithPath(roadRouteManager.occupiedRoad,"Home", null);
        if(nearestHome!=null)//sinon ça boucle
          destination = nearestHome.GetComponentInChildren<FreightAreaIn>().road;
      }
      if(destination != null)
      {
        roadRouteManager.MoveTo(destination);
        yield return new WaitUntil(() => destination == null || GetComponent<RoadRouteManager>().occupiedRoad == destination);
        if(destination != null)//Sinon, ça boucle
        {
          destination.roadLock.UnlockFor(GetComponent<MoveManager>().orientation,gameObject);
          if(prefecture != null)
            prefecture.GetComponent<Prefecture>().ComeBack(this);
          GameObject.Destroy(gameObject);
        }
      }
      yield return null;
    }
  }

  public void OnStuck(RetryData retryData)
  {
    if(--_tryAgainToVisitHome==0)
    {
      _continueTour=true;
    }
    else
    {
      if(goalFA != null)
        GetComponent<RoadRouteManager>().MoveTo(goalFA.road);
      else
        Debug.Log("if you see this, I'm a prefect and I'm stuck, I won't move again, and this is real real bad !");
    }
  }
}
