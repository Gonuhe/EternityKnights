using UnityEngine;
using System.Collections;

/**
* Sous-mode de CityBuilderMainMode servant au placement d'un bâtiment par
* le joueur dans le city builder.
**/
public class CityBuilderPlacingMode : GameMode
{
  private GameObject _currentPlacer;
  
  public PlacementData toPlace
  {
    set
    {
      _currentPlacer.GetComponent<BuildingPlacer>().toPlace=value;
      _currentPlacer.GetComponent<PlacerController>().subPlacerPrefab.toPlace=value;
    }
    
    get
    {
      return _currentPlacer.GetComponent<BuildingPlacer>().toPlace;	
    }
  }
  
  public override void ApplyPreconditions()
  {
    GameManager.instance.UnstackGameModesTo(GameModes.CITY_BUILDER_MAIN);
  }
	
  public override void UpdateMode()
  { 
  }
  
  public override void StartMode()
  {
    _currentPlacer=GameManager.instance.InstantiatePrefab("Prefabs/BuildingPlacer");  
    _currentPlacer.GetComponent<PlacerController>().callingMode=this;
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