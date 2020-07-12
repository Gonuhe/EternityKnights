using UnityEngine;

/**
* Attribut permettant au personnage du joueur de sélectionner des objets Selectable.
**/
public class PlayerSelectionManager : MonoBehaviour
{
  //Longueur d'un côté du carré servant de zone de détection au moment de la sélection.
  public float lookUpZoneSide=0.5f;
  
  private MoveManager _moveManager;
  private bool _selectionEnabled=false;

  protected void Start()
  {
    _moveManager=GetComponent<MoveManager>();
    EnableSelection();
  }
  
  protected void Update()
  {
    if(_selectionEnabled)
    {
      if(Input.GetButtonDown("Examine button"))
        Examine(GetFrontObjects());
    }
  }
  
  public void EnableSelection()
  {
    _selectionEnabled=true;
  }
  
  public void DisableSelection()
  {
    _selectionEnabled=false;
  }
  
  public void Examine(Collider2D[] frontObjects)
  {
    foreach(Collider2D frontObject in frontObjects)
    {
      Selectable selectableObject=frontObject.GetComponent<Selectable>();
      if(selectableObject!=null)
      {
        selectableObject.Examine();
        break;
      }
    }
  }
  
  public Collider2D[] GetFrontObjects()
  {
  	Vector2 playerPosition=transform.position;
  	
  	Vector2 diagTop=Vector2.zero;
  	Vector2 diagDown=Vector2.zero;
  	
  	switch(_moveManager.orientation)
  	{
  	  case Orientation.NORTH:
  	    diagTop=new Vector2(playerPosition.x-lookUpZoneSide/2.0f,playerPosition.y+lookUpZoneSide);
  	    diagDown=new Vector2(playerPosition.x+lookUpZoneSide/2.0f,playerPosition.y);
  	    break;
  	  case Orientation.SOUTH:
  	    diagTop=new Vector2(playerPosition.x-lookUpZoneSide/2.0f,playerPosition.y);
  	    diagDown=new Vector2(playerPosition.x+lookUpZoneSide/2.0f,playerPosition.y-lookUpZoneSide);
  	    break;
  	  case Orientation.WEST:
  	    diagTop=new Vector2(playerPosition.x-lookUpZoneSide,playerPosition.y+lookUpZoneSide/2.0f);
  	    diagDown=new Vector2(playerPosition.x,playerPosition.y-lookUpZoneSide/2.0f);
  	    break;
  	  case Orientation.EAST:
  	    diagTop=new Vector2(playerPosition.x,playerPosition.y+lookUpZoneSide/2.0f);
  	    diagDown=new Vector2(playerPosition.x+lookUpZoneSide,playerPosition.y-lookUpZoneSide/2.0f);
  	    break;
  	  default://c'est qu'on est orienté en diagonale
  	  	int yMultiplier= _moveManager.orientation.Contains(Orientation.NORTH) ? 1 : -1;
  	  	int xMultiplier= _moveManager.orientation.Contains(Orientation.EAST) ? 1 : -1;
  	  	diagTop=new Vector2(playerPosition.x+lookUpZoneSide*xMultiplier,playerPosition.y+lookUpZoneSide*yMultiplier);
  	  	diagDown=playerPosition;
  	  	break;
  	}
  	
    return Physics2D.OverlapAreaAll(diagTop,diagDown);
  }
}