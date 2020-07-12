using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkPlace : MonoBehaviour
{
  //Capacité par défaut, à changer dans le fils de WorkPlace en fonction du batiment.
  public int capacity = 4;
  public int workerCount = 0;

  //intéressant à garder pour pouvoir virer facilement.
  private Dictionary<Home,int> _workerHouses;

  private int _hiringMaxRadius = 100;


	// Use this for initialization
  void Start ()
  {
    _workerHouses = new Dictionary<Home, int> (capacity);
    EventsManager.AddListener(Events.CITY_POPULATION_CHANGED,HirePeople);//TODO le faire plutôt via une coroutine qui tourne tous les X temps
    HirePeople(capacity);
	}
	
	// Update is called once per frame
  void Update ()
  {

  }

  void HirePeople()
  {
    HirePeople(capacity - workerCount);
  }

  void HirePeople(int toHire)
  {
    HireDefaultStrategy(toHire);
  }

  /**
   * Engager des gens, selon plusieurs stratégie. Pour l'instant, une seule stratégie, la default, d'autres sont possibles (engager un max de gens dans un minimum de maisons,
   * engager des gens selon la maison qui a accès à le moins de travail, engager selon un taux de chomage,...).
   **/
  void HirePeople(int toHire, string strategy)
  {
    switch (strategy)
    {
      case "default":
        HireDefaultStrategy(toHire);
        break;
    }
    if(workerCount == capacity)
      EventsManager.RemoveListener(Events.CITY_POPULATION_CHANGED,HirePeople);
  }
  
  /**
  * Retourne un float entre 0 et 1 correspondant au ratio de travailleurs présents
  * dans le bâtiments par rapport au nombre qu'il lui en faut pour fonctionner
  * à plein régime (correspondant donc au champ capacity).
  **/
  public float WorkersRatio()
  {
  	if(capacity==0) return 1.0f;
  	
    return ((float)workerCount)/((float)capacity);
  }

  /**
   * Stratégie "aléatoire", on prend où on trouve en premier
   **/
  private void HireDefaultStrategy(int toHire)
  {
    List<GameObject> nearHomes = Utils.GetNearObjects(gameObject,"Home",_hiringMaxRadius);
    
    Dictionary<Home,int> hired = new Dictionary<Home,int>(toHire);

    foreach(GameObject nearHome in nearHomes)
    {

      Home nearHomeComponent = nearHome.GetComponent<Home>();
      if(nearHomeComponent.GetAvailableForWork() > 0)
      {
        //on engage

        //combien ?
        int howMuch = Mathf.Min(new int[2]{nearHomeComponent.GetAvailableForWork(), capacity - workerCount});

        //on engage
        hired.Add(nearHomeComponent, howMuch);
        workerCount += howMuch;

        //on prévient la maison qu'on a engagé quelqu'un de chez elle
        nearHomeComponent.Hire(this, howMuch);
      }
      if(workerCount >= capacity)
        break;
    }

    // Fusionner hired avec _workerHouse pour ajouter les recrues dans le pool de l'entreprise.
    foreach(KeyValuePair<Home,int> pair in hired)
    {
      int previousWorkersAtThatHouse;
      if(_workerHouses.TryGetValue(pair.Key, out previousWorkersAtThatHouse))
      {
        _workerHouses.Remove(pair.Key);
        _workerHouses.Add(pair.Key, previousWorkersAtThatHouse + pair.Value);
      }
      else
      {
        _workerHouses.Add(pair.Key, pair.Value);
      }
    }
  }

  public void Quit(Home home, int number)
  {
    int currentNumber;
    if(_workerHouses.TryGetValue(home, out currentNumber)) // il y avait des gens dans cette maison, sinon, envoyer une exception !
    {
      _workerHouses.Remove(home);
      if(currentNumber > number)
      {
        _workerHouses.Add(home, currentNumber - number);
      }
    } 
  }

  void OnDestroy()
  {
    foreach(KeyValuePair<Home, int> workerHouse in _workerHouses)
    {
      workerHouse.Key.Fire(this, workerHouse.Value);
    }
    //TODO faire poper les gens.
  }
}
