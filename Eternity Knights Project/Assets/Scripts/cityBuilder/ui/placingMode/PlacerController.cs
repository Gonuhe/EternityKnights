using UnityEngine;
using System;

public class PlacerController : MonoBehaviour
{
  public BuildingPlacer subPlacerPrefab;
  public GameMode callingMode;//doit être défini à la création du prefab par le mode appelant
  
  private BuildingPlacer _controlledPlacer;
  private bool _placing;
  private MultiplePlacementData _multiplePlacementData;
	
  protected void Awake()
  {
  	_controlledPlacer=GetComponent<BuildingPlacer>();
    _multiplePlacementData=new MultiplePlacementData(subPlacerPrefab);  
  }
  
  protected void Start()
  {
  	UpdatePosition();
  }
  
  void Update()//Update et pas FixedUpdate ici car le rigidbody détecte les collisions en mode continu, et Update permet plus de réactivité à la souris!
  {
  	CheckInputs();
  	if(!_placing) UpdatePosition();
  	else _multiplePlacementData.ReactToNewMousePosition(GameManager.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition));
  }

  private void CheckInputs()
  {  	  
    if(Input.GetMouseButtonDown(1)) //Un clic droit met fin au mode de placement de bâtiment
  	{
  	  GameManager.instance.UnstackGameMode(callingMode);
  	}
  	else
  	{
  	  if(!_placing && Input.GetMouseButton(0) && _controlledPlacer.CanPlace() && !GameManager.instance.mouseInUI)//Le clic de placement de bâtiments peut être maintenu
  	  {
  	    _placing=true;
  	    _multiplePlacementData.ResetFor(transform.position);
      }
      
      if(_placing && Input.GetMouseButtonUp(0))
      {
      	_controlledPlacer.TryToPlaceBuilding();
      	_multiplePlacementData.TryToPlaceEverything();
        _placing=false;
      }
    }
  }
  
  private void UpdatePosition()
  {
    Camera mainCamera=GameManager.instance.mainCamera;
    Vector3 mousePosition=Input.mousePosition;
  	
    Vector3 worldMousePosition=mainCamera.ScreenToWorldPoint(mousePosition);
    int unitsX=(int)worldMousePosition.x;
    int unitsY=(int)worldMousePosition.y;
    
    float xAdder=0.0f;
    float yAdder=0.0f;

    if(transform.localScale.x%2 == 1) xAdder=-0.5f;
    if(transform.localScale.y%2 == 1) yAdder=-0.5f;
    
    transform.position=new Vector3(unitsX+xAdder,unitsY+yAdder,0);
  }
  
  void OnDestroy()
  {
    _multiplePlacementData.DeletePlacers();
  }
}
  

  
