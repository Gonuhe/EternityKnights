using UnityEngine;
using System.Collections;

/**
* Classe contenant les différentes données servant à maintenir et gérer l'aspect
* RPG du jeu.
**/
public class RPGData
{	
  private int _money=0;

  public Inventory inventory = new Inventory();
  
  public int money
  {
    set
    {
      _money=value;
      EventsManager.Trigger(Events.RPG_MONEY_CHANGED);
    }
    
    get
    {
      return _money;	
    }
  }
  
  public QuestManager questManager=new QuestManager();
}
