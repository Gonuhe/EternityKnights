using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Collider2D))]
public class MonsterDetector : MonoBehaviour
{

  public HashSet<Monster> nearMonsters = new HashSet<Monster>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void OnTriggerEnter2D(Collider2D other)
  {
    if(!other.isTrigger)//pour être sûr que c'est bien notre trigger qui collisione le monstre et pas l'inverse
    {
      Monster m = other.GetComponent<Monster>();
      if(m!=null && !nearMonsters.Contains(m))
      {
        nearMonsters.Add(m);
        checkActivateCombatMode();
      }
    }
  }

  public void OnTriggerExit2D(Collider2D other)
  {
    if(!other.isTrigger)
    {
      Monster m = other.GetComponent<Monster>();
      if(m!=null && nearMonsters.Contains(m))
      {
        nearMonsters.Remove(m);
        checkActivateCombatMode();
      }
    }
  }

  public void Remove(Monster m)
  {
    nearMonsters.Remove(m);
    checkActivateCombatMode();
  }

  public void Add(Monster m)
  {
    nearMonsters.Add(m);
    checkActivateCombatMode();
  }

  public void Refresh()
  {
    nearMonsters.Clear();
    float radius = GetComponent<CircleCollider2D>().radius;
    Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position,radius, LayerMask.NameToLayer("Monster"));
    for(int i = 0; i<collisions.Length; i++)
    {
      Monster m = collisions[i].GetComponent<Monster>();
      if(m != null)
        nearMonsters.Add(m);
    }
    checkActivateCombatMode();
  }

  private void checkActivateCombatMode()
  {
    if(GameModes.RPG_COMBAT_MODE.active && nearMonsters.Count == 0)
      GameManager.instance.player.GetComponent<Player>().CombatMode(false);
    if(!GameModes.RPG_COMBAT_MODE.active && nearMonsters.Count > 0)
      GameManager.instance.player.GetComponent<Player>().CombatMode(true);
  }

}
