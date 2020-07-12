using UnityEngine;

public class ShiftDescriptor
{
  public const float SHIFT_DISTANCE=0.20f;
	
  private Vector2 _stepVector;
  private Vector2 _multipliers;
  private int _stepsNber;
  private int _currentStep=0;
  private bool _incrementMode=false;
  private int _usesCounter=0;
  
  public string orientation;

  public ShiftDescriptor(int steps,string orientation)
  {
  	this.orientation=orientation;
  	  
    int xMultiplier=0;
    int yMultiplier=0;
    
    if(orientation.Contains(Orientation.NORTH)) yMultiplier=1;
    else if(orientation.Contains(Orientation.SOUTH)) yMultiplier=-1;
    
    if(orientation.Contains(Orientation.EAST)) xMultiplier=1;
    else if(orientation.Contains(Orientation.WEST)) xMultiplier=-1;
    
    float stepValue=SHIFT_DISTANCE/steps;
    
    _multipliers=new Vector2(xMultiplier,yMultiplier);
    
    _stepVector=_multipliers*stepValue;
    
    _stepsNber=steps;
  }
  
  public Vector2 NextShift()
  {
  	if(_incrementMode) return NextShiftForIncrementMode();
    else return NextShiftForDecrementMode();
  }
  
  private Vector2 NextShiftForIncrementMode()
  {
    if(_currentStep!=_stepsNber)
      _currentStep++;
    
    if(_currentStep==_stepsNber) return _multipliers*SHIFT_DISTANCE;
    else return _stepVector*_currentStep;
  }
  
  private Vector2 NextShiftForDecrementMode()
  {
    if(_currentStep!=0)
      _currentStep--;
    
    if(_currentStep==0) return Vector2.zero;
    else return _stepVector*_currentStep;
  }
  
  public void ReserveUse()
  {
    _incrementMode=true;
    _usesCounter++;
  }
  
  public void ReleaseUse()
  {
  	if(_usesCounter>0) //Ne sera pas toujours le cas si le shift a été annulé pour être récupéré dans un mouvement (donc, c'est important)
      _usesCounter--;
    
    if(_usesCounter==0)
      _incrementMode=false;
  }
  
  public Vector2 Lookup()//retourne tout le shift sans rien changer à l'état 
  {
    if(_currentStep==0) return Vector2.zero;
    else if(_currentStep==_stepsNber) return _multipliers*SHIFT_DISTANCE;
    else return _currentStep*_stepVector;
  }
  
  public void Cancel()
  {
    _currentStep=0;
    _incrementMode=false;
    _usesCounter=0;
  }
}