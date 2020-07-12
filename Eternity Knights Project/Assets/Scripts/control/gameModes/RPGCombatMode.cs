using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Mode de jeu principal du RPG... Attrapez-les tous !
**/
public class RPGCombatMode : GameMode
{

  [HideInInspector]
  public float attackScope = 0.5f;

  public bool active;

  public override void ApplyPreconditions()
  {
  }
	
  public override void UpdateMode()
  {
    if(Input.GetButtonDown("Physical Attack"))
    {
      PhysicalAttack();
    }
  }
  
  public override void StartMode()
  {
    active = true;
    SpriteRenderer playerSR = GameManager.instance.player.GetComponent<SpriteRenderer>();
    playerSR.sprite = Resources.LoadAll<Sprite>("Test/Pavel")[1];
  }
  
  public override void StopMode()
  {
    active = false;
    SpriteRenderer PlayerSR =GameManager.instance.player.GetComponent<SpriteRenderer>();
    PlayerSR.sprite = Resources.LoadAll<Sprite>("Test/Pavel")[0];
  }
  
  protected override IEnumerator TransitionToMode(GameMode newMode)
  {
    return null;
  }

  /**
   * Fonction qui gère l'attacque de base, c'est à dire celle qui est lancée au click gauche, sans passé par la roue
   **/
  public void PhysicalAttack()
  {
    List<Monster> attackedMonsters = GetFrontMonsters();
    for(int i = 0; i < attackedMonsters.Count; i++)
    {
      attackedMonsters[i].Attack(GameManager.instance.player.GetComponent<Player>().attack);
    }
  }

  public List<Monster> GetFrontMonsters()
  {
    Collider2D[] frontObjects = Utils.GetFrontObjects(GameManager.instance.player.GetComponent<MoveManager>(),attackScope);
    List<Monster> frontMonsters = new List<Monster>();

    for(int i = 0; i<frontObjects.Length; i++)
    {
      if(!frontObjects[i].isTrigger)
      {
        Monster monster = frontObjects[i].gameObject.GetComponent<Monster>();
        if(monster != null)//condition a laisser. Notons que player est dans frontObjects, il ne peut donc pas avoir de script monster attaché sinon il s'attaque lui-même (important si on fait une classe mère à monster et player).
          frontMonsters.Add(monster);
      }
    }
    return frontMonsters;
  }

 
}