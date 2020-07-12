using UnityEngine;

/**
* Attribut à attacher à un objet équipé d'un collider pour en faire une zone
* de téléportation autre pour le joueur.
**/
public class WarpZone : MonoBehaviour
{
  public Transform destination;
  
  //Pour la gestion des WarpZones menant à l'intérieur d'un bâtiment
  public bool leadsInside=false;
  public Building destinationBuilding;//vaut null si !leadsInside
  
  
  void OnTriggerEnter2D(Collider2D collider)
  {
  	Player player=collider.GetComponent<Player>();
    PlayerMoveManager playerMoveManager=collider.GetComponent<PlayerMoveManager>(); 
  	  
  	if(playerMoveManager!=null && !collider.isTrigger)//On ne téléporte donc que le joueur
  	{
      Transform toWarp=collider.transform;
      Vector3 destinationVector=destination.position;
      playerMoveManager.DisableMove();
      toWarp.position=new Vector3(destinationVector.x,destinationVector.y,toWarp.position.z);
      player.isInsideBuilding=leadsInside;
      player.container=destinationBuilding;
      playerMoveManager.EnableMove();
    }
  }
}