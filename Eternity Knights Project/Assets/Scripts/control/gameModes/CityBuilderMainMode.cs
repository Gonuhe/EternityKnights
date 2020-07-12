using System.Collections;
using UnityEngine;

/**
* Mode de jeu principal du city builder, correspondant à l'affichage de la ville,
* avec sa trésorerie et le menu de construction à l'écran.
**/
public class CityBuilderMainMode : GameMode
{	
  public override void ApplyPreconditions()
  {
  }
	
  public override void UpdateMode()
  {
    if(Input.GetAxis("Mouse ScrollWheel")>0)
    {
      GameManager.instance.SetGameMode(GameModes.RPG_MAIN);
      GameManager.instance.TransitionCameraSize(CameraSize.RPG_CAMERA_SIZE);
    }
  }
  
  public override void StartMode()
  {
  	GameManager gameManager=GameManager.instance;
  	
    gameManager.cityBuilderCanvas.enabled=true;
    gameManager.player.GetComponent<PlayerMoveManager>().DisableMove();
    gameManager.mainCamera.GetComponent<CityBuilderCameraMotionManager>().enabled=true;
  }
  
  public override void StopMode()
  {
    GameManager gameManager=GameManager.instance;
    
    gameManager.cityBuilderCanvas.enabled=false;
    gameManager.mainCamera.GetComponent<CityBuilderCameraMotionManager>().enabled=false;
  }
  
  protected override IEnumerator TransitionToMode(GameMode newMode)
  {
    return null;
  }
}