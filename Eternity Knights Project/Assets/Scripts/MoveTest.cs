using UnityEngine;

public class MoveTest : MonoBehaviour
{
  public RoadData goal;

  protected void Start()
  {
  	if(GetComponent<RoadRouteManager>().occupiedRoad.roadLock.IsFree(GetComponent<MoveManager>().orientation))
  	  GetComponent<RoadRouteManager>().occupiedRoad.roadLock.LockFor(GetComponent<MoveManager>().orientation,gameObject);
    GetComponent<RoadRouteManager>().MoveTo(goal);
  }
}