using UnityEngine;

/**
* Définit les constantes à utiliser pour caractériser l'orientation des objets
* mouvants. Pour obtenir des orientation composites, il suffit de concaténer
* les deux orientations sur lesquelles on se base.
**/
public class Orientation
{
  public const string NORTH="North";
  public const string SOUTH="South";
  public const string WEST="West";
  public const string EAST="East";	
  
  public static readonly string[] ALL_SIMPLE={NORTH,SOUTH,WEST,EAST};
  
  /**
  * Retourne les shifts et l'orientation associée à un mouvement entre start 
  * et destination.
  **///TODO: corriger dans la doc et le nom: mutliplicateurs, pas shifts!
  public static Vector2 ShiftsAndOrientation(Vector2 start,Vector2 destination,out string orientation,float floatTolerance)
  {
    float xDif=destination.x-start.x;
    float yDif=destination.y-start.y;
    
    float xMultiplier=Utils.FloatComparison(xDif,0.0f,floatTolerance) ? 0.0f : xDif/Mathf.Abs(xDif);
    float yMultiplier=Utils.FloatComparison(yDif,0.0f,floatTolerance) ? 0.0f : yDif/Mathf.Abs(yDif);
    
    orientation="";
    
    if(yMultiplier<0) orientation+=SOUTH;
    else if (yMultiplier>0) orientation+=NORTH;
    
    if(xMultiplier<0.0f) orientation+=WEST;
    else if (xMultiplier>0.0f) orientation+=EAST;
    
    return new Vector2(xMultiplier,yMultiplier);
  }
  
  public static string PerpendicularOrientation(string orientation)
  {
  	string rslt="";
  	
  	if(orientation.Contains(WEST)) rslt+=NORTH;
    else if(orientation.Contains(EAST)) rslt+=SOUTH;
  	
    if(orientation.Contains(NORTH)) rslt+=EAST;
    else if(orientation.Contains(SOUTH)) rslt+=WEST;
    
    return rslt;
  }
  
  public static string OppositeOrientation(string orientation)
  {
    switch(orientation)
    {
      case NORTH: return SOUTH;
      case SOUTH: return NORTH;
      case EAST: return WEST;
      case WEST: return EAST;
      default: return null;
    }
  }
}