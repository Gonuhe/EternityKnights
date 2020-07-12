using UnityEngine;
using System.Collections.Generic;

/**
* Attribut définissant tous les utilitaires de mouvement du joueur en mode RPG, 
* à attacher à l'objet constituant son personnage.
**/
public class PlayerMoveManager : MonoBehaviour
{
  public static readonly float WALKING_SPEED=0.2f;
  public static readonly float RUNNING_SPEED=0.1f;
	
  private MoveManager _moveManager;
  private bool _moveEnabled=false;
  private bool _running;
  private float _currentSpeed=WALKING_SPEED;

  private Animator _animator;
  
  protected void Start()
  {
    _moveManager=GetComponent<MoveManager>();
    _animator = GetComponent<Animator>();
    EnableMove();
  }

  protected void Update()
  {
    if(_moveEnabled)
    {
      if(Input.GetButtonDown("Player Run Trigger"))
      {
        _running=!_running;
      
        if(_running)
        {
          _currentSpeed=RUNNING_SPEED;
          //TODO changement d'animation
        }
        else
        {
          _currentSpeed=WALKING_SPEED;
          //TODO changement d'animation
        }
      }
    }
  }
  
  private IEnumerator<MoveDescriptor> PlayerMovement()
  {
  	while(_moveEnabled)
  	{
      float horizontalMove=Input.GetAxisRaw("Horizontal")/4.0f;
      float verticalMove=Input.GetAxisRaw("Vertical")/4.0f;
      
      horizontalMove=Utils.FloatComparison(horizontalMove,0.0f,0.00001f) ? 0.0f : horizontalMove;
      verticalMove=Utils.FloatComparison(verticalMove,0.0f,0.00001f) ? 0.0f : verticalMove;
      
      Vector2 movementVector=new Vector2(horizontalMove,verticalMove);
      
      if(movementVector!=Vector2.zero)
      {
      	Vector2 currentPosition=transform.position;
        Vector2 newPosition=movementVector+currentPosition;
        yield return new MoveDescriptor(newPosition,currentPosition,_currentSpeed,0.00001f);        
      }  
      else yield return null;
    }
  }
  
  public void DisableMove()
  {
    _moveEnabled=false;  
    _moveManager.CancelMove();
  }
  
  public void EnableMove()
  {
  	if(!_moveEnabled)
  	{
      _moveEnabled=true;
      _moveManager.Move(PlayerMovement());
    }
  }
}