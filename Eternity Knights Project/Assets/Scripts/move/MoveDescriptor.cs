using UnityEngine;
using System;

/**
* Classe servant à coordonner les mouvements. L'idée est qu'un MoveDescriptor 
* donné est entièrement responsable de la coordination du mouvement d'un point A 
* à un point B.
**/
public class MoveDescriptor
{
  private Vector2 _destination;
  private Vector2 _start;
  private float _xStepLength;
  private float _yStepLength;
  private int _currentStep;
  private int _totalSteps;
  private float _xMultiplier;
  private float _yMultiplier;
  private string _orientation="";
  
  public string orientation
  {
    get
    {
      return _orientation;
    }
  }
  
  public MoveDescriptor(Vector2 destination,Vector2 start,float speed,float floatTolerance)
  {
    _destination=destination;
    _start=start;
    _totalSteps=(int)(speed/Time.deltaTime);
    _currentStep=0;
    
    float xDistance=Mathf.Abs(_start.x-_destination.x);
    float yDistance=Mathf.Abs(_start.y-_destination.y);
    
    _xStepLength=xDistance/_totalSteps;
    _yStepLength=yDistance/_totalSteps;
    
    Vector2 multipliers=Orientation.ShiftsAndOrientation(start,destination,out _orientation,floatTolerance);
    _xMultiplier=multipliers.x;
    _yMultiplier=multipliers.y;
  }
  
  /**
  * Fait progresser le mouvement de ce MoveDescriptor d'une étape et retourne la 
  * prochaine position de l'objet l'utilisant pour se déplacer.
  **/
  public Vector2 Decrement()
  {
    _currentStep++;
    
    if(MoveDone()) 
      return _destination;
    else
    {
      float rsltX=_start.x+_currentStep*_xStepLength*_xMultiplier;
      float rsltY=_start.y+_currentStep*_yStepLength*_yMultiplier;
      
      return new Vector2(rsltX,rsltY);
    }
  }
  
  /**
  * Retourne true <==> le mouvement du MoveDescriptor est intégralement terminé.
  **/
  public bool MoveDone()
  {
    return _currentStep==_totalSteps;  
  }
  
  /**
  * Retire le décalage du aux cases splitées en x (passé en paramètre) au mouvement
  * effectué par ce descriptor (à n'appeler que lorsque ce décallage rapproche 
  * l'objet mouvant de sa destination).
  **/
  public void retrieveXShift(float shift)//TODO majuscule
  {
    int nberOfSteps=Math.Abs((int)(shift/_xStepLength));
    _currentStep+=nberOfSteps;
  }
  
  /**
  * Retire le décalage du aux cases splitées en y (passé en paramètre) au mouvement
  * effectué par ce descriptor (à n'appeler que lorsque ce décallage rapproche 
  * l'objet mouvant de sa destination).
  **/
  public void retrieveYShift(float shift)//TODO majuscule
  {
    int nberOfSteps=Math.Abs((int)(shift/_yStepLength));
    _currentStep+=nberOfSteps;
  }
}