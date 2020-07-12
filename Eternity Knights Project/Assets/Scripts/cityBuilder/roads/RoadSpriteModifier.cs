using UnityEngine;

public class RoadSpriteModifier : MonoBehaviour
{
  public string roadTexturesPathPrefix;
  
  /*
  Les quatres suffixes correspondant à des points cardinaux peuvent se 
  concaténer, la convention est alors de le faire dans le sens horloger, 
  en suivant la rose des vents standard
  */
  public const string NORTH_SUFFIX="N";
  public const string SOUTH_SUFFIX="S";
  public const string EAST_SUFFIX="E";
  public const string WEST_SUFFIX="W";
  public const string NOTHING_SUFFIX="nothing";
	
  private RoadData _road;
  private SpriteRenderer _spriteRenderer;
  
  protected void Awake()
  {
    _road=GetComponent<RoadData>();
    _spriteRenderer=GetComponent<SpriteRenderer>();
  }
  
  public void CheckNeighborhood()//à appeler quand les voisins de la route concernée ont changé
  {
    string suffix="";
    
    if(_road.northNeighboringRoad!=null) suffix+=NORTH_SUFFIX;
    if(_road.eastNeighboringRoad!=null) suffix+=EAST_SUFFIX;
    if(_road.southNeighboringRoad!=null) suffix+=SOUTH_SUFFIX;
    if(_road.westNeighboringRoad!=null) suffix+=WEST_SUFFIX;
    
    if(suffix=="") suffix=NOTHING_SUFFIX;
    
    _spriteRenderer.sprite= Resources.Load(roadTexturesPathPrefix+suffix,typeof(Sprite)) as Sprite;
  }
}