using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Human : MonoBehaviour
{

  //A utiliser uniquement lors de la recherche de maisons.
  private HashSet<GameObject> _visitedHomes = new HashSet<GameObject>();
  private int _homeSearchTries = 0;

  private Coroutine _currentRoutine;


  // Use this for initialization
  protected void Awake ()
  {
  }

  public void SearchHome()
  {
    _currentRoutine = StartCoroutine(SearchHomeCoroutine());
  }
    
  private IEnumerator SearchHomeCoroutine()
  {
    GameObject _goalGameObject = Utils.GetNearestObjectWithPath(GetComponent<RoadRouteManager>().occupiedRoad,"Home", _visitedHomes);
    Home _goal = _goalGameObject == null ? null : _goalGameObject.GetComponent<Home>();
    if(_goal != null)
    {
      _visitedHomes.Add(_goal.gameObject);//Cette maison existe et est testée.//TODO Ajoute un peu trop facilement à visitedHomes dans le cas où une maison évolue. (cfr bug faux boss de fin de la veille mia!) plus défensif!!!

      if(_goal.HaveFreeRooms())//si elle a de la place
      {
        //On y va
        FreightAreaIn goalFA = _goal.GetComponent<FreightAreaData>().freightAreaIn;
        GetComponent<RoadRouteManager>().MoveTo(goalFA.GetComponent<RoadData>());
        _homeSearchTries++;
        
        yield return new WaitUntil(() => _goal == null || GetComponent<RoadRouteManager>().occupiedRoad == goalFA.road);

        if(_goal != null && _goal.MoveIn())
        {
          goalFA.road.roadLock.UnlockFor(GetComponent<MoveManager>().orientation,gameObject);
          GameObject.Destroy(gameObject);
          yield break; 
        }
      }
      //!HaveFreeRooms || !MoveIn
      _currentRoutine = StartCoroutine(SearchOtherHome());
    }
    else
    {
      _homeSearchTries++;
      _currentRoutine = StartCoroutine(SearchOtherHome());
    }
  }

  private IEnumerator SearchOtherHome()
  {
    if(_homeSearchTries < 5)
    {
      yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));        
      _currentRoutine = StartCoroutine(SearchHomeCoroutine());
    }
    else
    {
      _currentRoutine = StartCoroutine(GiveUpSearchingHome());
      //Debug.Log("CURRENT ROUTINE: "+_currentRoutine);
    }
  }

  private IEnumerator GiveUpSearchingHome()
  {//Debug.Log("GIVE UP SEARCHING ");
    GameObject portal = GameObject.Find("Portal");//TODO : Find ! Pas bien !
    gameObject.GetComponent<RoadRouteManager>().MoveTo(portal.GetComponentInChildren<RoadData>());
    yield return new WaitUntil(() => GetComponent<RoadRouteManager>().occupiedRoad == portal.GetComponentInChildren<RoadData>());
    portal.GetComponentInChildren<RoadData>().roadLock.UnlockFor(GetComponent<MoveManager>().orientation,gameObject);
    GameManager.instance.cityBuilderData.homeless--;
    GameObject.Destroy(gameObject);
  }

  public void StuckSearchingHome(RetryData retryData)
  {//Debug.Log("STUCK SEARCHING ");
    if(_currentRoutine != null) //TODO
      StopCoroutine(_currentRoutine);
    _currentRoutine=StartCoroutine(SearchOtherHome());
  }

}
