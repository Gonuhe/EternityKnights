using UnityEngine;
using System.Collections;

/**
 * Classe qui regroupe les conditions pour évoluer et les actions à faire pour évoluer.
 * Classe abstraite mère de tous les EvolutionData. Elle contient donc aussi les conditions à respecter TOUT le temps (ne pas oublier d'appeler base dans les classes filles) de même que les actions communes.
 * Lorsqu'un bâtiment possède des évolutions, il faut lui attacher un EvolutionManager et lui fournir un EvolutionData en paramètre.
 **/
[RequireComponent(typeof(EvolutionManager))]
public abstract class EvolutionData : MonoBehaviour 
{
  protected Building building;

  public int level = 0;

  protected void Awake()
  {
    building=GetComponent<Building>();
  }
  
  /**
   * Fonction mère de tous les EvolutionData. Doit être appelée via base.Evolve().
   * Elle augmente le niveau, c'est-à-dire qu'une fois base.evolve appelée level contient déjà le nouveau niveau.
   **/
  public virtual void Evolve ()
  {
    level++;
  }

  /**
   * Fonction mère de tous les EvolutionData. Doit être appelée via base.Devolve().
   * Elle augmente le niveau, c'est-à-dire qu'une fois base.Devolve appelée level contient déjà le nouveau niveau.
   **/
  public virtual void Devolve ()
  {
    level--;
  }

  /**
   * Condition a respecter pour évoluer.
   * Faire appel à base.MustEvolve pour bien checker les conditions des EvolutionData parents
   **/
  public virtual bool MustEvolve ()
  {
    return building.health >= 50;  //TODO TEST mais pas tellement
  }

  /**
   * Condition a respecter pour dévoluer.
   * Faire appel à base.MustDevolve pour bien checker les conditions des EvolutionData parents
   **/
  public virtual bool MustDevolve ()
  {
    return building.health <= 0;
  }
}
