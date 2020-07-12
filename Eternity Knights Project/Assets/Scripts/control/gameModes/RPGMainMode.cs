using System.Collections;
using UnityEngine;

/**
* Mode de jeu principal du RPG... Attrapez-les tous !
**/
public class RPGMainMode : GameMode
{
  private GameManager _gameManager;
	
  private bool _stopped=false;
	
  public override void ApplyPreconditions()
  {
  }
  
  public override void UpdateMode()
  {
    if(Input.GetAxis("Mouse ScrollWheel")<0)
    {
      _gameManager.SetGameMode(GameModes.CITY_BUILDER_MAIN);
      
      if(_gameManager.player.isInsideBuilding)
      {
      	Vector3 containerBuildingLocation=_gameManager.player.container.transform.position;
      	Vector3 initialPosition=_gameManager.mainCamera.transform.position;
        _gameManager.mainCamera.transform.position=new Vector3(containerBuildingLocation.x,containerBuildingLocation.y,initialPosition.z);
      }
      
      _gameManager.TransitionCameraSize(CameraSize.CITY_BUILDER_CAMERA_SIZE);
    }
  }
  
  public override void StartMode()
  {
  	_gameManager=GameManager.instance;  	  
  	_stopped=false;
  	  
    _gameManager.RPGCanvas.enabled=true;
    _gameManager.player.GetComponent<PlayerMoveManager>().EnableMove();
    
    _gameManager.InitiateRPGCameraView();
  }
  
  public override void StopMode()
  {
    _gameManager.RPGCanvas.enabled=false;
    _gameManager.mainCamera.GetComponent<RPGCameraMotionManager>().enabled=false;
    _stopped=true;
  }
  
  protected override IEnumerator TransitionToMode(GameMode newMode)
  {
    return null;
  }
}