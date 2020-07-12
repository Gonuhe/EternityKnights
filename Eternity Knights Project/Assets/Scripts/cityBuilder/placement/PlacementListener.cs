using UnityEngine;
using UnityEngine.Events;

/**
* Attribut à ajouer à un bâtiment pour faire en sorte qu'il observe les bâtiments
* voisins (à savoir, ceux qui sont juste à côté de lui) lors de son placement.
**/
public class PlacementListener : MonoBehaviour
{
  //Evénements correspondant à la détection d'un voisin
  public GameObjectEvent neighborNorth;
  public GameObjectEvent neighborSouth;
  public GameObjectEvent neighborEast;
  public GameObjectEvent neighborWest;
  
  //Evénement appelé une fois que le placement du bâtiment a été effectué
  public UnityEvent placementDone;
	
  //Les fonctions suivantes déclenchent les événements précités.
  public void OnNeighborNorth(GameObject neighbor)
  {
    neighborNorth.Invoke(neighbor);
  }
  
  public void OnNeighborSouth(GameObject neighbor)
  {
  	neighborSouth.Invoke(neighbor);
  }
  
  public void OnNeighborEast(GameObject neighbor)
  {
    neighborEast.Invoke(neighbor);
  }
  
  public void OnNeighborWest(GameObject neighbor)
  {
    neighborWest.Invoke(neighbor);
  }
  
  public void OnPlacementDone()
  {
    placementDone.Invoke();  
  }
}