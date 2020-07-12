using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combat : MonoBehaviour
{

  private float _attackScope = 0.5f;

  public float attackScope
  {
    get { return _attackScope;}
    set { _attackScope = value;}
  }

	// Use this for initialization
  void Start ()
  {
    this.enabled = false;
  }
	
	// Update is called once per frame
  void Update ()
  {
    if(Input.GetButtonDown("Physical Attack"))
    {
      PhysicalAttack();
    }
  }

  void OnEnable()
  {
    SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
    sr.sprite = Resources.LoadAll<Sprite>("Test/Pavel")[1];

  }

  void OnDisable()
  {
    SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
    sr.sprite = Resources.LoadAll<Sprite>("Test/Pavel")[0];
  }

  /**
   * Fonction qui gère l'attaque de base, c'est à dire celle qui est lancée au click gauche, sans passé par la roue
   **/
  public void PhysicalAttack()
  {
    List<Monster> attackedMonsters = GetFrontMonsters();
    for(int i = 0; i < attackedMonsters.Count; i++)
    {
      attackedMonsters[i].health -= gameObject.GetComponent<Player>().attack;
    }
  }

  public List<Monster> GetFrontMonsters()
  {
    Collider2D[] frontObjects = GetFrontObjects(attackScope);
    List<Monster> frontMonsters = new List<Monster>();

    for(int i = 0; i<frontObjects.Length; i++)
    {
      Monster monster = frontObjects[i].gameObject.GetComponent<Monster>();
      if(monster != null)
        frontMonsters.Add(monster);
    }
    return frontMonsters;
  }
    
  private Collider2D[] GetFrontObjects(float lookUpZoneSide)
  {
    Vector2 playerPosition=transform.position;

    Vector2 diagTop=Vector2.zero;
    Vector2 diagDown=Vector2.zero;

    MoveManager _moveManager=GetComponent<MoveManager>();

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
