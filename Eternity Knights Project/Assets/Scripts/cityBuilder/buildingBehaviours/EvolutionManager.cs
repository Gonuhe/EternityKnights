using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Building))]
public class EvolutionManager : MonoBehaviour
{
  [HideInInspector]
  public EvolutionData evolutionData;

  private Building building;

  //Temps par seconde entre les refreshs pour checker l'évolution et la dévolution.
  public float timeBetweenRefresh = 0.5f;

  void Awake()
  {
    evolutionData=GetComponent<EvolutionData>();  
    building=GetComponent<Building>();
  }
  
  void Start()
  {
    StartCoroutine(launchCheckAndApplyRoutine());
  }

  private IEnumerator launchCheckAndApplyRoutine()
  {
    while(true)
    {
      if(!building.placementData.HasPeopleOnView())
        checkAndApply();
      yield return new WaitForSeconds(timeBetweenRefresh);
    }
  }

  public void checkAndApply()
  {
    if(evolutionData != null && evolutionData.MustEvolve())
    {
      evolutionData.Evolve();
    }
    else if (evolutionData != null && evolutionData.MustDevolve())
    {
      evolutionData.Devolve();
    }
  }

}
