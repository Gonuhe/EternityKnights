using UnityEngine;
using System.Collections;

/**
* Classe contenant les différentes données servant à maintenir et gérer l'aspect
* city builder du jeu.
**/
public class CityBuilderData
{
  public StockManager stockManager=new StockManager();
	
  private int _treasury=0;

  private int _homeAvailable=0;
  private int _homeless=0;
  private int _population=0;
	
  public int treasury
  {
    set
    {
      _treasury=value;
      EventsManager.Trigger(Events.CITY_TREASURY_CHANGED);
    }
    
    get
    {
      return _treasury;	
    }
  }

  public int homeAvailable
  {
    set
    {
      _homeAvailable=value;
      EventsManager.Trigger(Events.CITY_HOME_AVAILABLE);
    }

    get
    {
      return _homeAvailable;
    }
  }

  public int homeless
  {
    set
    {
      _homeless=value;
    }
    
    get
    {
      return _homeless;
    }
  }

  public int population
  {
    set
    {
      _population=value;
      EventsManager.Trigger(Events.CITY_POPULATION_CHANGED);
    }

    get
    {
      return _population;
    }
  }
}
