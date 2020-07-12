using UnityEngine;

public class CityBuilderCameraMotionManager : MonoBehaviour
{
  public float speed;
  public int detectionZoneThickness;


  public Vector2 MinBound = new Vector2(-228,-228);
  public Vector2 MaxBound = new Vector2(250,230);
  
  void Update()
  {
  	Vector3 newCameraPosition=transform.position;  
  	  
    if(Input.mousePosition.x > Screen.width-detectionZoneThickness)
      newCameraPosition.x+= speed*Time.deltaTime;
    else if(Input.mousePosition.x  < detectionZoneThickness)
      newCameraPosition.x-= speed*Time.deltaTime;
  
    if(Input.mousePosition.y > Screen.height-detectionZoneThickness)
      newCameraPosition.y+= speed*Time.deltaTime;
    else if(Input.mousePosition.y  < detectionZoneThickness)
      newCameraPosition.y-= speed*Time.deltaTime;
  
    if(CanMoveThere(newCameraPosition))
      transform.position=newCameraPosition;
  }
  
  private bool CanMoveThere(Vector3 newCameraPosition)
  {
    return newCameraPosition.y < MaxBound.y && newCameraPosition.y > MinBound.y && newCameraPosition.x < MaxBound.x && newCameraPosition.x > MinBound.x;
  }
}