using UnityEngine;
using System.Collections;

/**
* Sous-mode de CityBuilderMainMode servant au placement d'un bâtiment par
* le joueur dans le city builder.
**/
public class CityBuilderDestroyMode : GameMode
{
  private GameObject _currentPlacer;
	
  public override void ApplyPreconditions()
  {
    GameManager.instance.UnstackGameModesTo(GameModes.CITY_BUILDER_MAIN);
  }
  
  public override void UpdateMode()
  { 
  }
  
  public override void StartMode()
  {
    _currentPlacer=GameManager.instance.InstantiatePrefab("Prefabs/AntiBuildingPlacer");
    
    AntiBuildingPlacer antiPlacer=_currentPlacer.GetComponent<AntiBuildingPlacer>();
    PlacerController placerController=_currentPlacer.GetComponent<PlacerController>();
    
    placerController.subPlacerPrefab.toPlace=antiPlacer.toPlace;
    placerController.callingMode=this;
  }
  
  public override void StopMode()
  {
    GameManager.instance.DestroyGameObject(_currentPlacer);
    _currentPlacer=null;
  }
  
  protected override IEnumerator TransitionToMode(GameMode newMode)
  {
    return null;
  }
}