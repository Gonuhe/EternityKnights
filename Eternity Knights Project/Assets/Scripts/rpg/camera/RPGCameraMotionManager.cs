using UnityEngine;

public class RPGCameraMotionManager : MonoBehaviour
{
  void FixedUpdate()
  {
  	Vector2 playerPosition=GameManager.instance.player.transform.position;
  	Vector3 oldPosition=transform.position;
    transform.position=new Vector3(playerPosition.x,playerPosition.y,oldPosition.z);
  }
}