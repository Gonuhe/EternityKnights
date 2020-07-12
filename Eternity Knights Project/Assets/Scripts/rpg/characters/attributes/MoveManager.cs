using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

/**
* Attribut définissant un objet capable de se déplacer (au moyen de MoveDescriptors).
**/
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class MoveManager : MonoBehaviour
{	
  //Orientation courante de l'objet.
  public string _orientation=Orientation.SOUTH;

  public string orientation
  {
    get {return _orientation;}
    set
    {
      _orientation = value;
      if(_animator != null)
      {
        switch (value)
        {
        case Orientation.SOUTH:
          _animator.SetInteger("Orientation", 180);
          break;
        case Orientation.WEST:
          _animator.SetInteger("Orientation", 270);
          break;
        case Orientation.NORTH:
          _animator.SetInteger("Orientation", 0);
          break;
        case Orientation.EAST:
          _animator.SetInteger("Orientation", 90);
          break;
        }
        if(_animator != null && gameObject.tag == "Player")
        {
          switch (value)
          {
          case "NorthEast":
            _animator.SetInteger("Orientation", 45);
            break;
          case "SouthEast":
            _animator.SetInteger("Orientation", 135);
            break;
          case "SouthWest":
            _animator.SetInteger("Orientation", 225);
            break;
          case "NorthWest":
            _animator.SetInteger("Orientation", 315);
            break;
          }
        }
      }
    }
  }
  
  public bool movePaused=false;
  
  private IEnumerator<MoveDescriptor> _currentMoveEnumerator;
  private UnityEvent _moveDoneEvent=new UnityEvent();
  
  private Dictionary<string,ShiftDescriptor> shifts=new Dictionary<string,ShiftDescriptor>();
  private Vector2 _basePosition;
  private SpriteRenderer _spriteRenderer;
  private Animator _animator;

  
  private static readonly float PERSPECTIVE_MULTIPLIER=-1000f;

  protected void Awake()
  {
    foreach(string orientation in Orientation.ALL_SIMPLE)//Donc: on ne permet pas de shifts en diagonale
    {
      shifts[orientation]=new ShiftDescriptor(5,orientation);//TODO 5 arbitraire	
    }
    
    _basePosition=transform.position;
    _spriteRenderer=GetComponent<SpriteRenderer>();
    _animator=GetComponent<Animator>();
  }
  
  void FixedUpdate()
  {
    if(!movePaused && _currentMoveEnumerator!=null)
    {
      if(_currentMoveEnumerator.Current!=null)
      {
      	IEnumerator<MoveDescriptor> enumeratorBeforeCondition=_currentMoveEnumerator;
      	
        if(_currentMoveEnumerator.Current.MoveDone() && !_currentMoveEnumerator.MoveNext())
        {
          
          if(_currentMoveEnumerator==enumeratorBeforeCondition)//indique que le mouvement est terminé
          {
            _currentMoveEnumerator=null;
            _moveDoneEvent.Invoke();
            _moveDoneEvent=new UnityEvent();
          }
          else if(_currentMoveEnumerator!=null)//Arrivera si MoveNext a déclenché un onStuck qui setté _currentMoveEnumerator à une nouvelle valeur
            _currentMoveEnumerator.MoveNext();
        }
      }
      else
      {
      	  _currentMoveEnumerator.MoveNext(); //appelé uniquement pour démarrer l'énumerateur et dans le cas des mouvements du joueur
      }
      
      if(_currentMoveEnumerator!=null && _currentMoveEnumerator.Current!=null)
      {
        _basePosition=_currentMoveEnumerator.Current.Decrement();
        orientation=_currentMoveEnumerator.Current.orientation;//TODO: dommage de faire ça à chaque fois, une fois après chaque changement de MD suffirait...
      }
    }
    
    if(_animator != null && !IsMoving())
      _animator.SetInteger("Orientation",-1);
    
    Vector2 shiftVector=Vector2.zero;
    
    foreach(ShiftDescriptor shiftDescriptor in shifts.Values)
    {
      shiftVector+=shiftDescriptor.NextShift();
    }
    
    Vector2 newPosition=_basePosition+shiftVector;
    
    _spriteRenderer.sortingOrder=((int)(newPosition.y*PERSPECTIVE_MULTIPLIER))%32767;//32767 est un maximum imposé par l'API de Unity
    
    GetComponent<Rigidbody2D>().MovePosition(newPosition);
  }
  
  public void AddMoveEndListener(UnityAction listener)
  {
    _moveDoneEvent.AddListener(listener);  
  }
  
  public void Move(IEnumerator<MoveDescriptor> moveEnumerator)
  {
    //TODO: uniquement si on n'est pas en train de bouger?? Peut-être annuler le dernier MD avant??
    _currentMoveEnumerator=moveEnumerator;
  }
  
  public void UseShift(string shiftOrientation)
  {
    shifts[shiftOrientation].ReserveUse();
  }
  
  public void ReleaseShift(string shiftOrientation)
  {
    shifts[shiftOrientation].ReleaseUse();  
  }
  
  public void UseShiftForMovement(MoveDescriptor descriptor)//bien préciser que ça annule le shift et le passe sur le MD
  {
  	ShiftDescriptor shiftDescriptor=shifts[descriptor.orientation];
    Vector2 shiftUntilNow=shiftDescriptor.Lookup();
    shiftDescriptor.Cancel();
    
    if(!Utils.FloatComparison(shiftUntilNow.x,0.0f,0.0001f)) descriptor.retrieveXShift(shiftUntilNow.x);
    else if(!Utils.FloatComparison(shiftUntilNow.y,0.0f,0.0001f)) descriptor.retrieveYShift(shiftUntilNow.y);
  }
  
  /**
  * Appelé pour indiquer qu'un mouvement a avorté (car plus moyen de trouver un
  * chemin, par exemple)
  **/
  public void CancelMove()
  {
    _currentMoveEnumerator=null;
    _moveDoneEvent=new UnityEvent();//Pour éviter d'appeler quand le mouvement n'a pas pu être accompli
  }
  
  public bool IsMoving()
  {
    return _currentMoveEnumerator!=null && _currentMoveEnumerator.Current != null;  
  }
}