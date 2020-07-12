using UnityEngine;
using System.Collections;

public class HomeEvolutionData : EvolutionData
{
  /*
  Ces différents GameObjects correspondent à ceux qui contiennent les 
  définitions (images et colliders) des niveaux d'évolution du bâtiment.
  Les définir comme ça est intéressant (plutôt que dans un tableau) car cela
  permet par exemple de faire un champ levels1To3, et donc de garder le même
  aspect pour plusieurs niveaux (c'est générique, mais je doute que ce soit 
  intéressant au final... Enfin soit, on peut le faire !).
  */
  public GameObject level0Object;
  public GameObject level1Object;
  public GameObject level2Object;
	
  public RoadData roadNotToHide;
  
  private Home _home;
  
  protected new void Awake()
  {
  	base.Awake();
    _home=GetComponent<Home>();  
  }
 
  public override void Evolve()
  {
    if(level==0)
    {
      ShowRoads();
      level0Object.SetActive(false);
      level1Object.SetActive(true);
      
      _home.AddToHomeConsumption(CityBuilderResources.WATER,60.0f);
    }   
    else if(level==1)
    {
      level1Object.SetActive(false);
      level2Object.SetActive(true);
      
      int toAdd = _home.capacity;
      _home.capacity+=toAdd;
      GameManager.instance.cityBuilderData.homeAvailable+=toAdd;
    }
    
    base.Evolve();//bien laisser à la fin car il incrémente le niveau!
  }
  
  public override void Devolve()
  {
    if(level==1)
    {
      HideRoads();
	  
	  _home.RemoveFromHomeConsumption(CityBuilderResources.WATER);
	  
      level1Object.SetActive(false);
      level0Object.SetActive(true);
    }
    else if(level==2)
    {
      level2Object.SetActive(false);
      level1Object.SetActive(true);
      int toRemove = _home.capacity/2;
      _home.capacity -= toRemove;
      if(_home.inhabitantsCount > _home.capacity)
      {
        _home.LeaveHouse(_home.inhabitantsCount-_home.capacity);
        GameManager.instance.cityBuilderData.population -= (_home.inhabitantsCount-_home.capacity);
      }
      else //Il y avait encore de la place dans cette maison, on mets à jour le nombre de places dispos dans la ville.
        GameManager.instance.cityBuilderData.homeAvailable-=toRemove;
    }

    base.Devolve();//bien laisser à la fin car il décrémente le niveau!
  }


  public override bool MustEvolve()
  {
  	switch(level)
  	{
      case 0: return _home.inhabitantsCount >= 1;
      case 1: return _home.buildingStock.StockFor(CityBuilderResources.WATER)>0;
      default: return false; 
  	}
  }
  public override bool MustDevolve()
  { 
  	switch(level)
  	{
      case 1: return _home.inhabitantsCount == 0;
      case 2: return _home.CouldNotConsume(CityBuilderResources.WATER);
      default: return false; 
  	} 
  }
  
  private void ShowRoads()
  {
    RoadData[] roads=GetComponentsInChildren<RoadData>();
    foreach(RoadData road in roads)
    {
      road.GetComponent<SpriteRenderer>().enabled=true;	
    }
  }
  
  private void HideRoads()
  {
    RoadData[] roads=GetComponentsInChildren<RoadData>();
    foreach(RoadData road in roads)
    {
      if(road!=roadNotToHide) road.GetComponent<SpriteRenderer>().enabled=false;	
    }
  }
}
