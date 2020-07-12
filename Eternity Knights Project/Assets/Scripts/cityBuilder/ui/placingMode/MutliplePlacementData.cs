using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MultiplePlacementData
{//TODO: imposer un nombre max de sous-placeurs (ça bouffe vite, comme ils détectent en continu!!)
  private BuildingPlacer _subPlacerPrefab;
  private float _maxPlacedX;
  private float _maxPlacedY;
  private float _minPlacedX;
  private float _minPlacedY;
  private LinkedList<LinkedList<BuildingPlacer>> _allPlacers=new LinkedList<LinkedList<BuildingPlacer>>();
  private Vector3 _initialPlacementCoord;
  private string _horizontalOrientation;
  private string _verticalOrientation;
  private int _placersNber=0;
  
  public int maxPlacersNber=200;
  
  public MultiplePlacementData(BuildingPlacer subPlacerPrefab)
  {
    _subPlacerPrefab=subPlacerPrefab;
  }
  
  public void ReactToNewMousePosition(Vector3 mouseWorldCoord)
  { 	    
  	if(mouseWorldCoord.y>_maxPlacedY)
      BeyondYMax(mouseWorldCoord);
    else if(mouseWorldCoord.y<_minPlacedY)
      BeyondYMin(mouseWorldCoord);
    else
      YShrink(mouseWorldCoord);
  	
    if(mouseWorldCoord.x>_maxPlacedX)
      BeyondXMax(mouseWorldCoord);
    else if(mouseWorldCoord.x<_minPlacedX)
      BeyondXMin(mouseWorldCoord);
    else
      XShrink(mouseWorldCoord);
  }
  
  private void TakePlacerIntoAccount(BuildingPlacer placer)
  {
    Vector3 position=placer.transform.position;
    _placersNber++;
    
    if(position.x>_maxPlacedX) _maxPlacedX=position.x;
    else if(position.x<_minPlacedX) _minPlacedX=position.x;
    
    if(position.y>_maxPlacedY) _maxPlacedY=position.y;
    else if(position.y<_minPlacedY) _minPlacedY=position.y;
  }
  
  public void ResetFor(Vector3 initialCoord)
  {
    _initialPlacementCoord=initialCoord;
    _maxPlacedX=initialCoord.x;
    _maxPlacedY=initialCoord.y;
    _minPlacedX=initialCoord.x;
    _minPlacedY=initialCoord.y;
    _placersNber=0;
    _allPlacers=new LinkedList<LinkedList<BuildingPlacer>>();
    _allPlacers.AddFirst(new LinkedList<BuildingPlacer>());
  }
  
  public void DeletePlacers()
  {
    foreach(LinkedList<BuildingPlacer> line in _allPlacers)
    {
      foreach(BuildingPlacer placer in line)
      {
        GameManager.instance.DestroyGameObject(placer.gameObject);  
      }
    }
    
    ResetFor(_initialPlacementCoord);
  }
  
  private void BeyondXMax(Vector3 mouseWorldCoord)
  {
    ManageXOverflow(mouseWorldCoord,Orientation.EAST,_maxPlacedX);
  }
  
  private void BeyondXMin(Vector3 mouseWorldCoord)
  {
    ManageXOverflow(mouseWorldCoord,Orientation.WEST,_minPlacedX);
  }
  
  private void ManageXOverflow(Vector3 mouseWorldCoord,string orientation,float xBound)
  {
    if(_horizontalOrientation!=orientation)
    {
      DeletePlacers();
      _horizontalOrientation=orientation;	
    }
        
    int xMultiplier= _horizontalOrientation==Orientation.EAST ? 1 : -1;
    int yMultiplier= _verticalOrientation==Orientation.NORTH ? 1 : -1;
    
    PlacementData placementData=_subPlacerPrefab.toPlace;
    int placersToAdd= (int)(Mathf.Abs(mouseWorldCoord.x-xBound)/placementData.unitsWidth);

    /*
    La condition de cette boucle vérifie qu'il faut encore ajouter une colonne
    de placeurs et qu'on ne viole pas la limite de placeurs en l'ajoutant.
    */
    for(int i=1;i<=placersToAdd && _placersNber+_allPlacers.Count<maxPlacersNber;i++)
    {
      for(int y=0;y<_allPlacers.Count;y++)
      {  
        Vector3 newPosition=new Vector3(xBound+i*xMultiplier*placementData.unitsWidth,_initialPlacementCoord.y+y*yMultiplier*placementData.unitsHeight,_subPlacerPrefab.transform.position.z);
        BuildingPlacer newPlacer=CreateNewPlacer(newPosition);
        Enumerable.ElementAt(_allPlacers,y).AddLast(newPlacer);
        TakePlacerIntoAccount(newPlacer);
      }
    }
  }
   
  private void XShrink(Vector3 mouseWorldCoord)
  {
    float xBound=_horizontalOrientation==Orientation.EAST ? _maxPlacedX : _minPlacedX;
    PlacementData placementData=_subPlacerPrefab.toPlace;
    int placersToRemove= (int)(Mathf.Abs(mouseWorldCoord.x-xBound)/placementData.unitsWidth);
    
    for(int i=1;i<=placersToRemove;i++)
    {
      foreach(LinkedList<BuildingPlacer> line in _allPlacers)
      { 
      	if(line.Count!=0)//La première ligne sera dans ce cas de temps à autre, puisque le placeur d'origine n'est pas dans les données de placement multiple
      	{
          BuildingPlacer toDelete=line.Last.Value;
          line.RemoveLast();  
          GameManager.instance.DestroyGameObject(toDelete.gameObject);
          _placersNber--;
        }
      }
      
      int placementWidth=_subPlacerPrefab.toPlace.unitsWidth;
      if(_horizontalOrientation==Orientation.EAST) _maxPlacedX-=placementWidth;
      else if(_horizontalOrientation==Orientation.WEST) _minPlacedX+=placementWidth;
    }
  }
  
  private void BeyondYMax(Vector3 mouseWorldCoord)
  {
    ManageYOverflow(mouseWorldCoord,Orientation.NORTH,_maxPlacedY);
  }
  
  private void BeyondYMin(Vector3 mouseWorldCoord)
  {
    ManageYOverflow(mouseWorldCoord,Orientation.SOUTH,_minPlacedY);
  }
  
  private void ManageYOverflow(Vector3 mouseWorldCoord,string orientation,float yBound)
  {
    if(_verticalOrientation!=orientation)
    {
      DeletePlacers();
      _verticalOrientation=orientation;	
    }
        
    int xMultiplier= _horizontalOrientation==Orientation.EAST ? 1 : -1;
    int yMultiplier= _verticalOrientation==Orientation.NORTH ? 1 : -1;
    
    PlacementData placementData=_subPlacerPrefab.toPlace;
    int placersToAdd= (int)(Mathf.Abs(mouseWorldCoord.y-yBound)/placementData.unitsHeight);
    
    /*
    La condition de cette boucle vérifie qu'il faut encore ajouter une ligne 
    de placeurs et qu'on ne viole pas la limite de placeurs en l'ajoutant.
    */
    for(int i=1;i<=placersToAdd && _placersNber+_allPlacers.First.Value.Count<maxPlacersNber;i++)
    {
      LinkedList<BuildingPlacer> newLine=new LinkedList<BuildingPlacer>();
      _allPlacers.AddLast(newLine);
    	
      for(int x=0;x<=_allPlacers.First.Value.Count;x++)//<= et pas <, car le placeur de départ occupe une case mais n'est pas compris dans les collections (pou éviter de le supprimer), il faut donc compenser ça !
      {
      	Vector3 newPosition=new Vector3(_initialPlacementCoord.x+x*xMultiplier*placementData.unitsWidth,yBound+i*yMultiplier*placementData.unitsHeight,_subPlacerPrefab.transform.position.z);
        BuildingPlacer newPlacer=CreateNewPlacer(newPosition);
        newLine.AddLast(newPlacer);
        TakePlacerIntoAccount(newPlacer);
      }
    }
  }
  
  private void YShrink(Vector3 mouseWorldCoord)
  {
    float yBound=_verticalOrientation==Orientation.NORTH ? _maxPlacedY : _minPlacedY;
    PlacementData placementData=_subPlacerPrefab.toPlace;
    int placersToRemove= (int)(Mathf.Abs(mouseWorldCoord.y-yBound)/placementData.unitsHeight);
    for(int i=1;i<=placersToRemove;i++)
    {
      LinkedList<BuildingPlacer> lineToRemove=_allPlacers.Last.Value;  
      _allPlacers.RemoveLast();
      
      int placementHeight=_subPlacerPrefab.toPlace.unitsHeight;
      if(_verticalOrientation==Orientation.NORTH) _maxPlacedY-=placementHeight;
      else if(_verticalOrientation==Orientation.SOUTH) _minPlacedY+=placementHeight;
     
      foreach(BuildingPlacer toDelete in lineToRemove) 
      {
        GameManager.instance.DestroyGameObject(toDelete.gameObject);
        _placersNber--;
      }
    }
  }
  
  private BuildingPlacer CreateNewPlacer(Vector3 newPosition)
  {
    BuildingPlacer rslt=GameManager.instance.InstantiatePrefab(_subPlacerPrefab.gameObject,newPosition).GetComponent<BuildingPlacer>();
    rslt.toPlace=_subPlacerPrefab.toPlace;//nécessaire car Instantiate ne copie que les champs public, et donc pas _toPlace
    return rslt;
  }
  
  public void TryToPlaceEverything()
  {
    foreach(LinkedList<BuildingPlacer> line in _allPlacers)
    {
      foreach(BuildingPlacer placer in line)
      {
        placer.TryToPlaceBuilding();
        GameManager.instance.DestroyGameObject(placer.gameObject);
      }
    }
    
    ResetFor(_initialPlacementCoord);
  }
}