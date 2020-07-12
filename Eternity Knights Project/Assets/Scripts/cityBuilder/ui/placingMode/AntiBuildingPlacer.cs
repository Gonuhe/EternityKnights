using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/**
* Un BuildingPlacer d'antimatière! Blague à part, il sert à retirer des bâtiments.
**/
public class AntiBuildingPlacer : BuildingPlacer
{
  private HashSet<PlacementData> _collidingBuildings=new HashSet<PlacementData>();
	
  protected void Awake()
  {
    toPlace=GetComponent<PlacementData>();//Champ hérité de BuildingPlacer, ici setté à la valeur du PlacementData attaché à l'objet pour permettre de réutiliser le code ;)
  }
  
  public override void OnTriggerEnter2D(Collider2D other)
  {
    PlacementData placementData=other.GetComponent<PlacementData>();
    DungeonData dungeonData=other.GetComponent<DungeonData>();
    
    if(placementData!=null && dungeonData==null)
      _collidingBuildings.Add(placementData);	
  }
  
  public override void OnTriggerExit2D(Collider2D other)
  {
    PlacementData placementData=other.GetComponent<PlacementData>();
    DungeonData dungeonData=other.GetComponent<DungeonData>();
    
    if(placementData!=null && dungeonData==null)
      _collidingBuildings.Remove(placementData);
  }
  
  public override bool CanPlace()
  {
    return true;
  }
  
  public override void TryToPlaceBuilding()
  {
    foreach(PlacementData toRemove in _collidingBuildings)
    {
      //TODO: ici, notifier les ResourceCarriers en cours de livraison éventuels
      toRemove.SetDestroying(true);
    }
    
    _collidingBuildings=new HashSet<PlacementData>();
  }
}