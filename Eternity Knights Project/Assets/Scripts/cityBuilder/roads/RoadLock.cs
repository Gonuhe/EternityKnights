using UnityEngine;
using System;
using System.Collections.Generic;

/**
* Classe servant à contrôler l'accessibilité des cases de route.
**/
[Serializable]
public class RoadLock
{
  public const int NO_SPLIT=0;
  public const int HORIZONTAL_SPLIT=1;
  public const int VERTICAL_SPLIT=2;
  public const int DIAGONAL_SPLIT=3;
	
  public int split;
  private Dictionary<string,GameObject> _occupants=new Dictionary<string,GameObject>();
  public int locksNber=0;
  public int reservationsNber=0;
  public bool markedByPlayer=false;
  public bool neverAccessible=false;//Utile à passer à true sur les prefabs qui utilisent des routes "décoratives", qui ne doivent pas être accessibles
  
  /*
  Ces variables sont uniquement utilisées entre le lock et l'unlock dans le cas 
  d'un DIAGONAL_SHIFT
  */
  private GameObject _diagShiftMainOccupant;
  private string _diagShiftMainOccupantOrientation;//Il est important de garder ceci dans une variable séparée (l'orientation peut changer entre lock et unlock, il faut donc la conserver)
  
  /*
  Variables uniquement utilisées dans le cas où les deux occupants de la case la verrouillent
  dans la même orientation (ce n'est possible que très rarement, et uniquement avec
  les DIAGONAL_SPLIT, dans le cas où l'un des deux veut bouger dans l'orientation
  de l'autre. On tolère ça avec cette petite astuce, car l'empêrcher crée des
  deadlocks (les perso se bloquent entre eux). 
  Ces deux variables vaqlent null tant qu'on n'est pas dans ce cas particulier.
  */
  private GameObject _doubleLockOccupant;
  private string _doubleLockOrientation;

  
  public bool IsFree()
  {
    return split==NO_SPLIT;  
  }
  
  public bool IsFree(string orientation)
  {  	    
    return !neverAccessible && !markedByPlayer && locksNber<2 && !_occupants.ContainsKey(orientation);
  }
  
  public void LockFor(string orientation,GameObject newOccupant)//vérifier auparavant que l'orientation est libre avec OrientationFree!!!
  {
  	if(_occupants.ContainsKey(orientation))//C'est donc qu'on a un lock double! Cette condition n'arrive que quand le split va devenir diagonal
  	{
  	  _doubleLockOccupant=newOccupant;
  	  _doubleLockOrientation=orientation;
  	}
    else//lock standard
  	_occupants.Add(orientation,newOccupant);//Pas fait avec un [] pour bien provoquer une exception si la clé est déjà contenue!
  	  
    if(locksNber==0)
    {
      switch(orientation)
      {
        case Orientation.NORTH: split=VERTICAL_SPLIT; break;
        case Orientation.SOUTH: split=VERTICAL_SPLIT; break;
        case Orientation.EAST: split=HORIZONTAL_SPLIT; break;
        case Orientation.WEST: split=HORIZONTAL_SPLIT; break;
      }
    }
    else
    {
      bool diagonalSplitNS=(orientation==Orientation.NORTH || orientation==Orientation.SOUTH) && split==HORIZONTAL_SPLIT;
      bool diagonalSplitEW=(orientation==Orientation.EAST || orientation==Orientation.WEST) && split==VERTICAL_SPLIT;
    	
      if(diagonalSplitNS || diagonalSplitEW)
      {
        split=DIAGONAL_SPLIT;        
        ApplyMoveShiftsOnObjectsForDiagonalSplit(GetOccupantDifferentFrom(newOccupant).GetComponent<MoveManager>(),newOccupant.GetComponent<MoveManager>());
      }
      else 
        ApplyMoveShiftsOnObjectsForRegularSplits();
    }
    
    locksNber++;
      
  }
  
  public void UnlockFor(string orientation,GameObject target) 
  {
  	if(locksNber==1)
  	{
  	  split=NO_SPLIT;	
  	}
    else if(split==DIAGONAL_SPLIT)
    {
      foreach(string orientationKey in _occupants.Keys)//Cette boucle itère donc max 2 fois, elle sert juste à retrouver l'orientation du deuxième occupant
      {
        if(orientationKey!=orientation)  
        {
          if(orientationKey==Orientation.NORTH || orientationKey==Orientation.SOUTH) split=VERTICAL_SPLIT;
          else split=HORIZONTAL_SPLIT;
        }
      }
      
      UnapplyMoveShiftsOnObjectsForDiagonalSplit();
    }
    else
    {
  	  switch(orientation)
      {
        case Orientation.NORTH:  
      	    split=VERTICAL_SPLIT; 
          break;
        case Orientation.SOUTH:  
      	    split=VERTICAL_SPLIT; 
          break;
        case Orientation.EAST:  
      	    split=HORIZONTAL_SPLIT; 
          break;
        case Orientation.WEST:  
      	    split=HORIZONTAL_SPLIT; 
          break;
      }
      
      UnapplyMoveShiftsOnObjectsForRegularSplits();	
    }
    
    locksNber--;
  
    if(orientation==_doubleLockOrientation)//OK car orientation ne vaudra jamais null, donc on teste ici que c'est bien !=null aussi
    {
      if(target!=_doubleLockOccupant)
      {
        _occupants[orientation]=_doubleLockOccupant;//On remplace donc l'occupant dans le dictionnaire.
      }
      
      _doubleLockOccupant=null;
      _doubleLockOrientation=null;        
    }
    else _occupants.Remove(orientation);
  }
  
  private void ApplyMoveShiftsOnObjectsForDiagonalSplit(MoveManager mainOccupant,MoveManager newOccupant)//MainOccupant est celui qui se trouve dans la case depuis le plus longtemps
  {
  	_diagShiftMainOccupant=mainOccupant.gameObject;
  	_diagShiftMainOccupantOrientation=mainOccupant.orientation;
  	  
  	string mainOrientation=mainOccupant.orientation;
    mainOccupant.UseShift(mainOrientation);
    mainOccupant.UseShift(Orientation.PerpendicularOrientation(mainOrientation));
    
    string mainOrientationOpposite=Orientation.OppositeOrientation(mainOrientation);
    newOccupant.UseShift(mainOrientationOpposite);
    newOccupant.UseShift(Orientation.PerpendicularOrientation(mainOrientationOpposite));
  }
  
  private void ApplyMoveShiftsOnObjectsForRegularSplits()
  {
    foreach(KeyValuePair<string,GameObject> occupantData in _occupants)
    {
      GameObject occupant=occupantData.Value;
      string shiftOrientation=Orientation.PerpendicularOrientation(occupantData.Key);
      occupant.GetComponent<MoveManager>().UseShift(shiftOrientation);
    } 
  }
  
  private void UnapplyMoveShiftsOnObjectsForRegularSplits()
  {
    foreach(KeyValuePair<string,GameObject> occupantData in _occupants)
    {
      GameObject occupant=occupantData.Value;
      
      string shiftOrientation=Orientation.PerpendicularOrientation(occupantData.Key);
      occupant.GetComponent<MoveManager>().ReleaseShift(shiftOrientation);
    }  
  }
  
  private void UnapplyMoveShiftsOnObjectsForDiagonalSplit()
  {
  	MoveManager mainOccupant=_diagShiftMainOccupant.GetComponent<MoveManager>();
  
  	mainOccupant.ReleaseShift(_diagShiftMainOccupantOrientation);
  	mainOccupant.ReleaseShift(Orientation.PerpendicularOrientation(_diagShiftMainOccupantOrientation));
  	
  	MoveManager secondOccupant;
  	if(_doubleLockOccupant!=null)
  	{
  	  secondOccupant=_doubleLockOccupant.GetComponent<MoveManager>();
  	  _doubleLockOccupant=null;
  	  _doubleLockOrientation=null;
  	}
  	else secondOccupant= GetOccupantDifferentFrom(_diagShiftMainOccupant).GetComponent<MoveManager>();
  	
  	string mainOrientationOpposite=Orientation.OppositeOrientation(_diagShiftMainOccupantOrientation);
  	secondOccupant.ReleaseShift(mainOrientationOpposite);
  	secondOccupant.ReleaseShift(Orientation.PerpendicularOrientation(mainOrientationOpposite));
  	
    _diagShiftMainOccupant=null;
  	_diagShiftMainOccupantOrientation=null;
  }
  
  /**
  * Retourne le premier occupant de cette case (découvert en itérant) 
  * différent de other, ou null s'il n'y en a aucun qui remplisse cette condition.
  * Donc: comme il y a maximum deux occupants, retourne celui qui n'est PAS other.
  *
  * Passer null à cette fonction dans le cas où il n'y a qu'un seul occupant
  * retourne cet occupant.
  **/
  private GameObject GetOccupantDifferentFrom(GameObject other)
  {
    foreach(GameObject occupant in _occupants.Values)
    {
      if(occupant!=other) return occupant;	
    }
    
    return null;
  }
}