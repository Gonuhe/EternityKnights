using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WorkPlace))]
public class Prefecture : Building
{
  private WorkPlace _workPlace;

  public RoadData door;

  private int _gonePrefects = 0;

  protected new void Start ()
  {
    _workPlace = GetComponent<WorkPlace>();
    StartCoroutine(GeneratePrefect());
  }

  private IEnumerator GeneratePrefect()
  {
    while(true)
    {
      if(_workPlace.workerCount > 0 && door.roadLock.IsFree(Orientation.SOUTH))
      {
        GameObject newPrefect = (GameObject)Instantiate(Resources.Load("Prefabs/Prefect"), door.transform.position, Quaternion.identity);

        newPrefect.GetComponent<RoadRouteManager>().occupiedRoad = door;
        door.roadLock.LockFor(Orientation.SOUTH,newPrefect);

        newPrefect.GetComponent<Prefect>().prefecture = gameObject;
        _gonePrefects++;
      }
      yield return new WaitUntil(() => _gonePrefects == 0);
      yield return new WaitForSeconds(120.0f/(float)_workPlace.workerCount); // si 1 travailleur, resort après 2min, si 4 travailleurs, toutes les 30 secondes
    }
  }

  public void ComeBack(Prefect prefect)
  {
    _gonePrefects--;
    GetComponentInChildren<RoadData>().roadLock.UnlockFor(prefect.GetComponent<MoveManager>().orientation,prefect.gameObject);
    GameObject.Destroy(prefect.gameObject);
  }
}
