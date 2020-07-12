using UnityEngine;
using System.Collections;


/**
 * Classe générale destinée à représenter le joueur. Contient les informations de bases.
 **/
public class Player : MonoBehaviour
{

  public bool combatModeActivable = true;//pour pouvoir le désactiver dans certain cas.

  public const int MAX_HEALTH = 100;
  private const int MIN_HEALTH = 0;

  public int health;
    
  public void SetHealth(int value)
  {
      if(value < 0)
        value = 0;
      if(value >= MIN_HEALTH && value <= MAX_HEALTH)
      {
        health = value;
        EventsManager.Trigger(Events.RPG_HEALTH_CHANGED);
      }
      if(health <= 0)
        Die();
  }

  public void AttackMe(int value)
  {
    SetHealth(health - value);
  }

  public int attack = 25;

  /**
   * Retourne l'attaque effective, c'est-à-dire tenant compte de tous les modificateurs (équipement, statut,...)
   **/
  public int getActualAttack ()
  {
    //TODO c'est le moment d'appliquer toute sorte de modificateurs.
    return attack;
  }
  
  public bool isInsideBuilding=false;
  public Building container=null; //sera différent de null uniquement si le joueur est dans un bâtiment <=> isInsideBuilding==true

	// Use this for initialization 
  public void Start ()
  {
	
	}
	
	// Update is called once per frame
	void Update ()
  {
	}

  /**
   * Fonction destinée à restaurer les points de santé du joueur sur une certaine durée. Pour un effet instantané, utiliser les accesseurs.
   **/
  public void RestoreHealth(int toRestore, int timeInSeconds)
  {
    StartCoroutine(RestoreHealthCoroutine(toRestore,timeInSeconds));
  }

  /**
   * Fonction destinée à dégrader les points de santé du joueur sur une certaine durée. Pour un effet instantané, utiliser les accesseurs.
   **/
  public void DegradeHealth(int toDegrade, int timeInSeconds)
  {
    RestoreHealth(-toDegrade,timeInSeconds);
  }

  private IEnumerator RestoreHealthCoroutine(int toRestore, int timeInSeconds)
  {
    for(int i = 0; i<timeInSeconds; i++)
    {
      health += toRestore/timeInSeconds;
      yield return new WaitForSeconds(1);
    }
  }

  //TODO : comme ça, je pense qu'il peut y avoir des incohérences au niveau des gamemode. Par exemple, si en plein dialogue, un monstre s'approche, les deux modes seront activés ce qui risque de créer des bugs. A régler avec Oli et son expertise des modes
  public void CombatMode(bool b)
  {
    if(combatModeActivable)
    {
      if(b && GameManager.instance.activeGameMode == GameModes.RPG_MAIN)//TODO : 2e condition est pour patcher la note du todo ci-dessus, mais je suis pas convaincu
      {
        GameManager.instance.StackGameMode(GameModes.RPG_COMBAT_MODE);
      }
      if(!b && GameManager.instance.activeGameMode == GameModes.RPG_COMBAT_MODE)//TODO 2e condition idem + signifie que combat mode ne pas avoir d'enfant lorsqu'on quitte la zone d'activation du combat mode
      {
        GameManager.instance.UnstackGameMode(GameModes.RPG_COMBAT_MODE);
      }
    }
  }

  public void KilledMonster(Monster monster)
  {
    //ajouter xp, argent,...
    GetComponentInChildren<MonsterDetector>().Remove(monster);
  }

  /**
   * Fonction qui gère la mort du héros. Pour l'instant, on a qu'a afficher le game-over.
   **/
  public void Die()
  {
    GameManager.instance.GameOver();
  }
}

