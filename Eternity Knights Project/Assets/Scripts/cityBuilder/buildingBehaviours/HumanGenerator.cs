using UnityEngine;
using System.Collections;

/**
 * Classe destinée à gérer la création d'hommes qui vont pouvoir migrer et se diriger vers la ville du city builder.
 * Cette classe doit etre attachée à un gameobject contenant un ensemble de tribu humaines, de manière à ce qu'elle
 * puisse choisir quelle tribu générera combien d'humain.
 * Elle peut aussi etre attachée à un ensemble de portails, puisque les tribus seront probablement dans une autre scène.
 **/
public class HumanGenerator : MonoBehaviour 
{

  public RoadData door;

  /**
   * Temps entre chaque génération automatique d'humain (certaines générations "manuelles" sont aussi faites, en fonction d'event ou autre...)
   **/
  public float generateInterval = 10.0f;

  private bool _generating = false;

  // Use this for initialization
  void Start () 
  {
    EventsManager.AddListener(Events.CITY_HOME_AVAILABLE,GenerateHumans);
    StartCoroutine(GenerateHumansPeriodically());
  }

  public void GenerateHumans()
  {
    if(!_generating)
      StartCoroutine(GenerateHumansRoutine());
  }

  /**
   * Fonction d'IA qui va gérer le nombre d'humain a générer, choisir une tribu pour, et lancer la commande à la tribu en question.
   **/
  private IEnumerator GenerateHumansRoutine()
  {
    _generating = true;

    //Destiné à représenter la partie de la population SDF tolérable. Si on veut que le nombre de personne générées ne soit pas exactement le meme que le nombre d'habitations possible
    int homelessFactor = 1;

    while (GameManager.instance.cityBuilderData.homeAvailable - homelessFactor*GameManager.instance.cityBuilderData.homeless > 0) //TODO : + Range pour ajouter de l'aléatoire ? 
    {
      if(door.roadLock.IsFree(Orientation.SOUTH))
      {
        GameObject newHuman = (GameObject)Instantiate(Resources.Load("Prefabs/HumanTest"), door.transform.position, Quaternion.identity);

        newHuman.GetComponent<RoadRouteManager>().occupiedRoad = door;
        door.roadLock.LockFor(Orientation.SOUTH,newHuman);

        newHuman.GetComponent<Human>().SearchHome();
        
        GameManager.instance.cityBuilderData.homeless++;
      }
      yield return new WaitForSeconds(1 + Random.Range(0.25f,1.5F));
    }

    _generating = false;
    yield return null;
  }

  private IEnumerator GenerateHumansPeriodically()
  {
    while(true)
    {
      if(!_generating)
      {
        yield return StartCoroutine(GenerateHumansRoutine());
      }
      yield return new WaitForSeconds(generateInterval);
    }
  }
}
