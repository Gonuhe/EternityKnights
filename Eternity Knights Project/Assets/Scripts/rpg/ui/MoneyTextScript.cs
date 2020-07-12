using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SmartLocalization;

/**
* Script permettant de gérer l'actualisation du compteur d'argent du joueur en 
 mode RPG
**/
public class MoneyTextScript : MonoBehaviour 
{
  public Text moneyText;
	
  protected void Start()
  {
  	RefreshText();
    EventsManager.AddListener(Events.RPG_MONEY_CHANGED,RefreshText);
  }
  
  public void RefreshText()
  {
    moneyText.text= LanguageManager.Instance.GetTextValue("RPG.Money") + GameManager.instance.RPGData.money;
  }
}
