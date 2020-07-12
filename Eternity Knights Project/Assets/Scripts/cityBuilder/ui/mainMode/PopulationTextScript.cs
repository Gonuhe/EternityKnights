using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SmartLocalization;

/**
* Script permettant de gérer l'actualisation du compteur de trésorerie du mode
* city builder.
**/
public class PopulationTextScript : MonoBehaviour 
{
  public Text populationText;
	
  void Start()
  {
  	RefreshText();
    EventsManager.AddListener(Events.CITY_POPULATION_CHANGED,RefreshText);
  }
  
  public void RefreshText()
  {
    populationText.text= LanguageManager.Instance.GetTextValue("CityBuilder.Population") + GameManager.instance.cityBuilderData.population;
  }
}
