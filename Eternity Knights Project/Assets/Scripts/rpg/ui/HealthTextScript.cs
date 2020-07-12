using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SmartLocalization;

/**
* Script permettant de gérer l'actualisation du compteur de trésorerie du mode
* city builder.
**/
public class HealthTextScript : MonoBehaviour 
{
  public Text healthText;
	
  protected void Start()
  {
  	RefreshText();
    EventsManager.AddListener(Events.RPG_HEALTH_CHANGED,RefreshText);
  }
  
  public void RefreshText()
  {
    healthText.text= LanguageManager.Instance.GetTextValue("RPG.Health")
      + GameManager.instance.player.GetComponent<Player>().health.ToString("F2")//pour avoir 2 décimale 
      + "/" + Player.MAX_HEALTH;
  }
}
