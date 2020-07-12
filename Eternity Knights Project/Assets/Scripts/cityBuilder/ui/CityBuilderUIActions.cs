using UnityEngine;

/**
* Classe à attacher à l'objet contenant le GameManager.
* Elle définit les différentes actions associées aux interactions à effectuer
* avec l'interface utilisateur du mode city builder.
**/
public class CityBuilderUIActions : MonoBehaviour
{
  public void TriggerBuildingPlacingFor(PlacementData toPlace)
  {
    GameManager gameManager=GameManager.instance;
  	CityBuilderPlacingMode placingMode=GameModes.CITY_BUILDER_PLACING;
          
    if(!gameManager.isModeActive(placingMode))
      GameManager.instance.StackGameMode(placingMode);
  
    placingMode.toPlace=toPlace;
  }
  
  public void TriggerDestroyMode()
  {
    GameManager gameManager=GameManager.instance;
  	CityBuilderDestroyMode destroyMode=GameModes.CITY_BUILDER_DESTROY;
          
    if(!gameManager.isModeActive(destroyMode))
      GameManager.instance.StackGameMode(destroyMode);
  }
}
