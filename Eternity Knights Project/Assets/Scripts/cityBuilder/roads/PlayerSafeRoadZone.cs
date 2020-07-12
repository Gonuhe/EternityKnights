using UnityEngine;

public class PlayerSafeRoadZone : MonoBehaviour
{	
  void OnTriggerEnter2D(Collider2D other)
  {
    RoadData roadData=other.GetComponent<RoadData>();
    if(roadData!=null)
      roadData.roadLock.markedByPlayer=true;
  }
  
  void OnTriggerExit2D(Collider2D other)
  {
    RoadData roadData=other.GetComponent<RoadData>();
    if(roadData!=null)
      roadData.roadLock.markedByPlayer=false;
  }
}