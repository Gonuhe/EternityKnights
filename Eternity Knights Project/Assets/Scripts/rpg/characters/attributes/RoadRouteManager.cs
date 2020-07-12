using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
* Attribut indiquant que l'objet auquel il est attaqué peut se mouvoir sur les
* routes et contenant toutes les opérations nécessaires pour ce faire.
**/
[RequireComponent(typeof(MoveManager))]
public class RoadRouteManager : MonoBehaviour
{
  //Route sur laquelle se trouve actuellement l'objet
  public RoadData occupiedRoad;
  public RetryDataEvent onStuckEvent;
  
  public const int MAX_MOVE_TRIES=4;
  
  private int _keyPointsModulo;
  
  private MoveManager _moveManager;

  public float speed = 0.5f;

  
  void Awake()
  {
    _keyPointsModulo=UnityEngine.Random.Range(3,8);
    _moveManager=GetComponent<MoveManager>(); 
  }
  
  /**
  * Lance le mouvement de l'objet vers destination.
  **/
  public void MoveTo(RoadData destination)
  {
  	if(destination==null) onStuckEvent.Invoke(null);
  	  
  	if(occupiedRoad!=destination)
      StartCoroutine(MoveToWithMultipleTries(destination,0.0f,null));
  }
  
  /**
  * Lance le mouvement de l'objet vers destination après delay secondes.
  **/
  public void MoveTo(RoadData destination,float delay,RetryData retryData)
  {
  	if(destination==null) onStuckEvent.Invoke(null);
  	  
  	if(occupiedRoad!=destination)
      StartCoroutine(MoveToWithMultipleTries(destination,delay,retryData));
  }
  
  private IEnumerator MoveToWithMultipleTries(RoadData destination,float delay,RetryData retryData)
  {
  	if(delay>0.0f) yield return new WaitForSeconds(delay);
  	  
    string currentOrientation=_moveManager.orientation;
    
    Stack<RoadData> moveKeyPoints=null;
    int tries=0;
    while(moveKeyPoints==null)
    {
    	
      if(tries>0) yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f,3.0f));
      moveKeyPoints=RoadsPathfinding.RouteStar(destination,occupiedRoad,_keyPointsModulo,currentOrientation);

      tries++;
    }

    _moveManager.Move(FollowMoveKeyPoints(moveKeyPoints,destination,retryData));
  }
    
  private IEnumerator<MoveDescriptor> FollowMoveKeyPoints(Stack<RoadData> moveKeyPoints,RoadData finalDestination,RetryData retryData)
  {
  	RoadData lastKeyPoint=occupiedRoad;
    foreach(RoadData keyPoint in moveKeyPoints)
    {
      if(keyPoint!=null && keyPoint.Neighbors().Contains(occupiedRoad))
        AnticipateNextOrientation(occupiedRoad,keyPoint,_moveManager.orientation);
    	
      RoadData lastRoad=occupiedRoad;
      while(occupiedRoad != keyPoint)
      {
      	string currentOrientation=_moveManager.orientation;
        Stack<RoadData> roadPath=RoadsPathfinding.RoadPathStar(keyPoint,occupiedRoad,_keyPointsModulo,currentOrientation,finalDestination);
        
        if(roadPath==null)//On annule cet énumérateur et on relance la recherche 
        {
          if(retryData==null)
            retryData=new RetryData(lastKeyPoint,finalDestination);
       
          retryData.IncrementCounter();
          
          _moveManager.CancelMove();
          
          if(retryData.triesCounter==MAX_MOVE_TRIES)
            onStuckEvent.Invoke(retryData);
          else
            MoveTo(finalDestination,UnityEngine.Random.Range(0.25f,1.5f),retryData); //Ce qui relance donc une coroutine qui va s'occuper de ça!
          
          yield break;
        }
        else if(roadPath.Count==0)
        {
          break; //Si le roadPath est vide, c'est qu'on est déjà à destination, ou suffisamment proche de celle-ci pour passer au prochain keyPoint
        }
        
        RoadData nextRoad=roadPath.Pop();
        
        if(nextRoad!=lastRoad || UnityEngine.Random.Range(1,100)==1)
        {
          foreach(RoadData toReserve in roadPath)
          {    
           toReserve.roadLock.reservationsNber++;  
          }
        
          MoveDescriptor toApply= new MoveDescriptor(nextRoad.transform.position,occupiedRoad.transform.position,speed,0.1f);
        
          string orientationToUnlock=currentOrientation;
          
          if(MovementWithoutUnshift(occupiedRoad.roadLock,currentOrientation,toApply))//Partage la case avec un autre dans une orientation contraire
            _moveManager.UseShiftForMovement(toApply);
          else if(occupiedRoad.roadLock.split!=RoadLock.DIAGONAL_SPLIT)//!NoNeedToRelock())
          {
            occupiedRoad.roadLock.UnlockFor(currentOrientation,gameObject);
            occupiedRoad.roadLock.LockFor(toApply.orientation,gameObject);
            orientationToUnlock=toApply.orientation;  
          }
        
          nextRoad.roadLock.LockFor(toApply.orientation,gameObject);
          //+++++> C'est la ligne ci-dessus qui est chiante: avec un diag split, on risque de relocker dans la mme direction que le contenu principal...>TROUVER UNE SOLUTION
          //>>>> cas de double split à gérer via une condition spéciale (offre spéciale, SUPER PROMO) dans le RoadLock (avec qques var globales :/)

          yield return toApply;
        
          lastRoad=occupiedRoad;
          occupiedRoad.roadLock.UnlockFor(orientationToUnlock,gameObject);
          occupiedRoad=nextRoad;
      
          foreach(RoadData toFree in roadPath)
          {
           toFree.roadLock.reservationsNber--;  
          }
          
          if(roadPath.Count!=0)//Donc, s'il y a encore une case sur laquelle bouger par la suite jusqu'au prochain keypoint
          { 
          	RoadData secondNextRoad=roadPath.Pop();
              AnticipateNextOrientation(nextRoad,secondNextRoad,toApply.orientation);
          }
        }
        else
          yield return null;
      }
      
      lastKeyPoint=keyPoint;
    }  
    
    yield return null;
  }
  
  private void AnticipateNextOrientation(RoadData currentRoad,RoadData nextRoad,string currentOrientation)
  {
    if(currentRoad.roadLock.locksNber==1)
    {          	  
      string nextOrientation="";
      Orientation.ShiftsAndOrientation(currentRoad.transform.position,nextRoad.transform.position,out nextOrientation,0.1f);
            
      if(nextOrientation!=currentOrientation)
      {
        currentRoad.roadLock.UnlockFor(currentOrientation,gameObject);
        currentRoad.roadLock.LockFor(nextOrientation,gameObject);
              
        _moveManager.orientation=nextOrientation;
      }
    }	  
  }
  
  private bool MovementWithoutUnshift(RoadLock roadLock,string orientation,MoveDescriptor descriptor)
  {
    return (roadLock.split==RoadLock.HORIZONTAL_SPLIT && orientation==Orientation.WEST && descriptor.orientation==Orientation.NORTH)
           || (roadLock.split==RoadLock.HORIZONTAL_SPLIT && orientation==Orientation.EAST && descriptor.orientation==Orientation.SOUTH)
           || (roadLock.split==RoadLock.VERTICAL_SPLIT && orientation==Orientation.NORTH && descriptor.orientation==Orientation.EAST)
           || (roadLock.split==RoadLock.VERTICAL_SPLIT && orientation==Orientation.SOUTH && descriptor.orientation==Orientation.WEST);
  }
}