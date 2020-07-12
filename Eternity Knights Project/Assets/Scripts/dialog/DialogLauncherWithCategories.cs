using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization;

/**
 * Un lanceur de dialogue avec une catégorie affublée au personnage. Si ce personnage n'a pas de dialog, il en choisira un au hasard dans sa catégorie et l'affichera.
 * Si un Dialog est déjà setté, il reste prioritaire. Ceci est destiné aux PNJ qui ne sont pas géré par script (dans une quête, par exemple) pour ne pas qu'ils disent tous la même chose.
 **/
[RequireComponent (typeof(Collider2D))]
[RequireComponent (typeof(Selectable))]
public class DialogLauncherWithCategories:DefaultDialogLauncher
{
  public string categoryPath = "default";

  new protected void OnTriggerEnter2D(Collider2D collider)
  {
    if(collider.gameObject.tag == "Player")
      _controlHint.text = LanguageManager.Instance.GetTextValue("Main.PressExamineToTalk");
  }

  new public void Talk()
  {
    if(!DialogManager.instance.dialogStarted)
    {
      if(dialog != null)
        dialog = PickDialog();
    }
    base.Talk();
  }

  private string PickDialog()
  {
    Object[] allDialogs = DialogManager.instance.GetAllDialogsForCategory(categoryPath+"/");

    return categoryPath+"/"+allDialogs[Random.Range(0,allDialogs.Length)].name;
  }

  new public void FinishedDialog()
  {
    dialog = null;
  }
}