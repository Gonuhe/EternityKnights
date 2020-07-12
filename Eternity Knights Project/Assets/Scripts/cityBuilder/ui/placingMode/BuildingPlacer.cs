using UnityEngine;
using System.Collections;
using System;

/**
* Script de gestion d'un placeur de bâtiment (la case verte ou rouge).
**/
public class BuildingPlacer : MonoBehaviour 
{
  public Renderer placerRenderer;
  public Material canBuildMaterial;
  public Material cannotBuildMaterial;
  
  public GameObject freightAreaInMarker;
  public GameObject freightAreaOutMarker;
  
  //Utilisée par le collider pour gérer la couleur du placeur
  private int _currentCollisions=0;
 
  //true ssi l'objet a été créé et démarré dans Unity (permet donc de différencier une instance ingame du prefab), après l'appel à Start
  private bool _started=false; 
  
  private PlacementData _toPlace;
  public PlacementData toPlace
  {
    set
    {
      transform.localScale=new Vector3(value.unitsWidth,value.unitsHeight,1);
      _toPlace=value;
      
      if(_started && HasMarkers())
        PlaceMarkers();
    }
    
    get
    {
      return _toPlace;
    } 
  }
  
  void Start()
  {
  	_started=true; 
  	
    if(HasMarkers())
      PlaceMarkers();
  }
  
  private bool HasMarkers()
  {
    return _toPlace.GetComponentInChildren<FreightAreaInPathMarker>()!=null && _toPlace.GetComponentInChildren<FreightAreaOutPathMarker>();
  }
  
  //PRE: _toPlace contient des scripts PathMarker pour ses FreightArea
  private void PlaceMarkers()
  {
  	//Destruction des marqueurs précédemment placés, s'il y en a
  	foreach(Transform child in transform)
  	{
      GameManager.instance.DestroyGameObject(child.gameObject);	
  	}
  	  
  	//Génération des marqueurs à mettre sur le placeur
  	float placerScalingFactorX=_toPlace.transform.localScale.x/(_toPlace.unitsWidth);
  	float placerScalingFactorY=_toPlace.transform.localScale.y/(_toPlace.unitsHeight);
  	
  	//FAIn
    Vector3 FAInLocal=_toPlace.GetComponentInChildren<FreightAreaInPathMarker>().transform.localPosition;
    FAInLocal.x*=placerScalingFactorX;
    FAInLocal.y*=placerScalingFactorY;
    GameObject FAInMarker=GameManager.instance.InstantiatePrefabAsChild(gameObject,freightAreaInMarker,transform.position);
    FAInMarker.transform.localPosition=FAInLocal;
    
    //FAOut
    Vector3 FAOutLocal=_toPlace.GetComponentInChildren<FreightAreaOutPathMarker>().transform.localPosition;
    FAOutLocal.x*=placerScalingFactorX;
    FAOutLocal.y*=placerScalingFactorY;
    GameObject FAOutMarker=GameManager.instance.InstantiatePrefabAsChild(gameObject,freightAreaOutMarker,transform.position);
    FAOutMarker.transform.localPosition=FAOutLocal;
  }
	
  public virtual void OnTriggerEnter2D(Collider2D other)
  {
    placerRenderer.material=cannotBuildMaterial;
    _currentCollisions++;
  }
  
  public virtual void OnTriggerExit2D(Collider2D other)
  {
    _currentCollisions--;
    
    if(_currentCollisions==0)
      placerRenderer.material=canBuildMaterial;
  }
  
  public virtual bool CanPlace()
  {
    return _currentCollisions==0;  
  }
  
  public virtual void TryToPlaceBuilding()
  {
  	CityBuilderData cityBuilderData=GameManager.instance.cityBuilderData;
    int cost=toPlace.cost;
    
  	if(CanPlace() && cityBuilderData.treasury >= cost)
    {
      GameObject newBuilding=Instantiate(toPlace.gameObject, transform.position, Quaternion.identity) as GameObject;
      
      ManagePlacementListener(newBuilding.GetComponent<PlacementData>());
      CallPlacementListenersOnChildren(newBuilding);
      
      cityBuilderData.treasury-=cost;
      GetComponent<Rigidbody2D>().WakeUp();
    }//TODO: un else avec un event qui déclenche un message disant qu'on n'a pas les sous serait une bonne chose.
  }
  
  private void CallPlacementListenersOnChildren(GameObject newBuilding)
  {
    foreach(Transform child in newBuilding.transform)
    {
      PlacementData childPlacementData=child.GetComponent<PlacementData>();
      
        if(childPlacementData!=null)
          ManagePlacementListener(childPlacementData);
      
      CallPlacementListenersOnChildren(child.gameObject);
    }  
  }
  
  private void ManagePlacementListener(PlacementData newBuilding)
  {
    PlacementListener newBuildingListener=newBuilding.GetComponent<PlacementListener>();
    if(newBuildingListener!=null)
    {
      CheckXNeighborhood(newBuilding,0.5f,newBuildingListener.OnNeighborEast);
      CheckXNeighborhood(newBuilding,-0.5f,newBuildingListener.OnNeighborWest);
      CheckYNeighborhood(newBuilding,0.5f,newBuildingListener.OnNeighborNorth);
      CheckYNeighborhood(newBuilding,-0.5f,newBuildingListener.OnNeighborSouth);
      newBuildingListener.OnPlacementDone();
    }  
  }
  
  private void CheckXNeighborhood(PlacementData buildingData,float xMultiplier,Action<GameObject> listenerToCall)//xMultiplier: +: à droite  -: à gauche
  {
  	float unitsWidth=(float)buildingData.unitsWidth;
  	float unitsHeight=(float)buildingData.unitsHeight;
  	Transform objectTransform=buildingData.transform;
  	  
    Vector2 diagTop=new Vector2(objectTransform.position.x+(xMultiplier/Mathf.Abs(xMultiplier))*(unitsWidth/2.0f)+xMultiplier,objectTransform.position.y+unitsHeight/2.0f);
    Vector2 diagDown=new Vector2(objectTransform.position.x+(xMultiplier/Mathf.Abs(xMultiplier))*(unitsWidth/2.0f),objectTransform.position.y-unitsHeight/2.0f);
    
    DetectNeighbors(listenerToCall,diagTop,diagDown);
  }
  
  private void CheckYNeighborhood(PlacementData buildingData,float yMultiplier,Action<GameObject> listenerToCall)//yMultiplier: +: haut  -: bas
  {
  	float unitsWidth=(float)buildingData.unitsWidth;
  	float unitsHeight=(float)buildingData.unitsHeight;
  	Transform objectTransform=buildingData.transform;
  	
    Vector2 diagTop=new Vector2(objectTransform.position.x+unitsWidth/2.0f, objectTransform.position.y+(yMultiplier/Mathf.Abs(yMultiplier))*(unitsHeight/2.0f)+yMultiplier);
    Vector2 diagDown=new Vector2(objectTransform.position.x-unitsWidth/2.0f, objectTransform.position.y+(yMultiplier/Mathf.Abs(yMultiplier))*(unitsHeight/2.0f));
    
    DetectNeighbors(listenerToCall,diagTop,diagDown);
  }
  
  private void DetectNeighbors(Action<GameObject> listenerToCall,Vector2 diag1,Vector2 diag2)
  {
    foreach(Collider2D collider in Physics2D.OverlapAreaAll(diag1,diag2))//TODO changer le layer!!
    {
      GameObject collided=collider.gameObject;
      if(collided.transform.position!=transform.position)//évite la collision avec le placeur
        listenerToCall(collided);  
    }
  }  
}
