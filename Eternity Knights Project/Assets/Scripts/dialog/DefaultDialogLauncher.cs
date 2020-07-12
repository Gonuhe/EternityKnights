using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization;

/**
 * Le lanceur de dialogue par défaut, à ajouter sur un gameobject pour lui permettre de parler. Aucune action supplémentaire n'est posssible avec ce type de dialoglauncher.
 * Doit aussi être la classe mère de tous les autres dialogLauncher
 **/
[RequireComponent (typeof(Collider2D))]
[RequireComponent (typeof(Selectable))]
public class DefaultDialogLauncher:MonoBehaviour
{
    

  protected Text _controlHint;

  public string dialog;

  private Stack<string> _dialogStack = new Stack<string>();

  public bool talking = false;
        
  void Start ()
  {
    _controlHint = GameObject.Find("ControlHint").GetComponent<Text>();
  }
    
  protected void OnTriggerEnter2D(Collider2D collider)
  {
    if(collider.gameObject.tag == "Player" && dialog != null)
      _controlHint.text = LanguageManager.Instance.GetTextValue("Main.PressExamineToTalk");
  }
    
  void OnTriggerExit2D(Collider2D collider)
  {
    _controlHint.text = "";
  }

  public void Talk()
  {
    if(!DialogManager.instance.dialogStarted)
    {//Si le dialogue n'est pas encore commencé, on le commence
      _controlHint.text = "";
      if(DialogManager.instance.DialogExists(dialog))
        DialogManager.instance.StartDialog(dialog, this);//first sentence
    }
    else if(DialogManager.instance.IsThereChoice())
    {//il y avait des choix sur l'écran courant
      DialogManager.instance.ValidateChoice();
    }
    else
    {//Simplement montrer la phrase suivante
      DialogManager.instance.NextDialog();
    }
  }

  //a utiliser dans héritage
  public void FinishedDialog()
  {}

  public void StackDialog(string path)
  {
    if(DialogManager.instance.DialogExists(path))
    {
      _dialogStack.Push(dialog);
      dialog = path;
    }
  }

  public string UnstackDialog()
  //TODO exception
  {
    if(_dialogStack.Count != 0)
    {
      string r = dialog;
      dialog = _dialogStack.Pop();
      return r;
    }
    return null;
  }

}