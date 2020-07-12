using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

  public const int MAX_HEALTH = 100;
  private const int MIN_HEALTH = 0;
  
  private bool _playerInDetectionArea;
  public bool playerInDetectionArea
  {
    get
    {
      return _playerInDetectionArea;	
    }
  }
  

  private bool _collidingWithPlayer=false;
  /**
   * A n'utiliser que pour du debug, pas en script, plutot utiliser Attack() pour que le jeu check si on meurt
   **/
  public int health = MAX_HEALTH;

  public void Attack(int damage)
  {
    health -= damage;
    health = health <=MIN_HEALTH ? MIN_HEALTH : health;
    health = health <= MAX_HEALTH ? health : MAX_HEALTH;
    if(health==0)
    {
      GameManager.instance.player.GetComponent<Player>().KilledMonster(this);
      GameObject.Destroy(gameObject);
    }
  }

  public float moveStat;
  public int attackStat;

  protected void Start() 
  {
    StartCoroutine(ApplyDamages());
  }
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter2D(Collider2D other)
  {
    if(other.gameObject.tag == "Player")
    {
      _playerInDetectionArea=true;
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if(other.gameObject.tag == "Player")
    {
      _playerInDetectionArea=false;
    }
  }
  
  void OnCollisionEnter2D(Collision2D collision)
  {
  	Player player=collision.collider.GetComponent<Player>();
    if(player!=null)
    {
      _collidingWithPlayer=true;
      GetComponent<MoveManager>().movePaused=true;
    }
  }
  
  void OnCollisionExit2D(Collision2D collision)
  {
    Player player=collision.collider.GetComponent<Player>();
    if(player!=null)
    {
      _collidingWithPlayer=false;
      GetComponent<MoveManager>().movePaused=false;
    }
  }
  
  private IEnumerator ApplyDamages()
  {
    while(true)
    {
      if(_collidingWithPlayer)
      {
        Player player=GameManager.instance.player.GetComponent<Player>();
        player.AttackMe(attackStat);
        yield  return new WaitForSeconds(1.0f);//TODO 1 seconde, arbitraire
      }
      else yield return new WaitForSeconds(0.1f);
    }
  }
}
