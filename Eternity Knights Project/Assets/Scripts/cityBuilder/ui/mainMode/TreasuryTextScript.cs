using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SmartLocalization;

/**
* Script permettant de gérer l'actualisation du compteur de trésorerie du mode
* city builder.
**/
public class TreasuryTextScript : MonoBehaviour 
{
  public Text treasuryText;
	
  protected void Start()
  {
  	RefreshText();
    EventsManager.AddListener(Events.CITY_TREASURY_CHANGED,RefreshText);
  }
  
  public void RefreshText()
  {
    treasuryText.text= LanguageManager.Instance.GetTextValue("CityBuilder.Treasury") + GameManager.instance.cityBuilderData.treasury;
  }
}
