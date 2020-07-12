using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Reflection;

/**
 * Classe chargée de gerer les dialogues.
 **/
//TODO splitter la classe en un modèle et une vue ?
public class DialogManager
{
  public static DialogManager instance;// Instanciée dans Awake de GameManager

  private GameObject _dialogFrame;
  private Text _dialogFrameText;

  public bool  dialogStarted = false;
  private DialogXML _dialogXML;
  private int _index;

  private bool _choice = false;
  //Contient le dernier choix effectué. Dans une variable globale de façon à pouvoir enregistrer le choix le cas échéant, dans une postAction.
  private Choice _lastValidatedChoice;
    
  private DefaultDialogLauncher _caller;

  private string _dialogFolderPath = "Texts/Dialogs/";
    
    
  public DialogManager ()
  {
    _dialogFrame = GameObject.Find("DialogFrame");
    _dialogFrameText = _dialogFrame.GetComponentInChildren<Text>();
    _dialogFrame.SetActive(false);

  }

  public void StartDialog(string path, DefaultDialogLauncher caller)
  {
    SetCaller(caller);
    caller.talking = true;
    StartDialog(path);
  }

  public bool DialogExists(string path)
  {
    TextAsset asset = Resources.Load(_dialogFolderPath+path) as TextAsset;
    return asset != null;
  }
    
  private void StartDialog(string path)//On a besoin du caller pour unstack dialog avant d'exécuter la dialogPostAction
  {     
    GameManager.instance.StackGameMode(GameModes.DIALOG_MODE);
    dialogStarted = true;
    _dialogFrame.SetActive(true);
    _index=0;

    TextAsset asset = Resources.Load(_dialogFolderPath+path) as TextAsset;
    Stream stream = new MemoryStream(asset.bytes);
      
    XmlSerializer serializer = new XmlSerializer(typeof(DialogXML));
    _dialogXML = serializer.Deserialize(stream) as DialogXML;
    stream.Close();
     
    NextDialog();
  }
    
  public void NextDialog()
  {
    if(_index < _dialogXML.dialog.Count)
    {
      if(_dialogXML.dialog[_index].preAction != null)
        ExecutePostAction(_dialogXML.dialog[_index].preAction);
      if(_dialogXML.dialog[_index].text == Sentence.END_OF_CONV)
      {
        if(_dialogXML.dialog[_index].postAction != null)
          ExecutePostAction(_dialogXML.dialog[_index].postAction);
        StopDialog();
      }
      else
      {
        _dialogFrameText.text = ParseSentenceForArgs(_dialogXML.dialog[_index].text);
        DisplayChoice();
        //TODO : do some other stuff like show character artwork
        if(_dialogXML.dialog[_index].postAction != null)
          ExecutePostAction(_dialogXML.dialog[_index].postAction);
        if(!_choice)
        {
          _index++;
        }
      }
    }
    else //on a atteint la fin du fichier
      StopDialog();
  }

  public void StopDialog()
  {
    _dialogFrameText.text = "";
    _dialogFrame.SetActive(false);
    _caller.UnstackDialog();
    if(_dialogXML.postAction != null)
      ExecutePostAction(_dialogXML.postAction);
    _dialogXML = null;
    dialogStarted = false;
    _caller.talking = false;
    _caller.FinishedDialog();
    _caller = null;
    GameManager.instance.UnstackGameMode(GameModes.DIALOG_MODE);
  }

  public bool IsThereChoice()
  {
    return _choice;
  }

  /**
   * Fonction qui permet de détecter s'il y a un choix sur cette sentence et, le cas échéant, l'afficher
   **/
  private void DisplayChoice()
  {
    if(_dialogXML.dialog[_index].choices.Count != 0)
    {
      _choice = true;
      ChoiceManager.instance.gameObject.SetActive(true);
      ChoiceManager.instance.LoadChoices(_dialogXML.dialog[_index].choices);
    }
    else
    {
      _choice = false;
    }
  }

  private string ParseSentenceForArgs(string text)
  {
    for(int i = 1; text.Contains("%"+i+"%"); i++)
    {
      ArgumentProvider argProv = _caller.GetComponent<ArgumentProvider>();
      text = text.Replace("%"+i+"%", argProv.GetArgument(i));
    }
    return text;
  }

  /**
   * Affiche le texte suivant à l'écran, en fonction du choix donné.
   **/
  public void ValidateChoice()
  {
    _lastValidatedChoice = ChoiceManager.instance.ValidateChoice();
    ChoiceManager.instance.gameObject.SetActive(false);
    _index = GetSentenceIndexById(_lastValidatedChoice.goToId);//TODO check if _index != -1
    NextDialog();
  }

  public Choice GetLastValidatedChoice()
  {
    return _lastValidatedChoice;
  }

  /**
   * Retourne le véritable index (la position du dialogue dans la liste) d'une sentence en fonction de son id.
   * Retourne -1 si il n'y a aucune sentence avec cette id.
   **/
  private int GetSentenceIndexById(int id)
  {
    for(int i = 0; i < _dialogXML.dialog.Count;i++)
    {
      if(_dialogXML.dialog[i].id == id)
        return i;
    }
    return -1;
  }

  /**
   * Exécute la postAction associée à la sentence qui possède l'index index.
   * index est un argument nécessaire car si 
   **/
  private void ExecutePostAction(string postAction)
  {
    if(postAction.Contains(":"))
    {
      string[] splittedPostAction = postAction.Split(':');
      ExecutePostActionNotAttached(splittedPostAction[0], splittedPostAction[1]);
    }
    else
      ExecutePostActionAttached(postAction);
  }

  private void ExecutePostActionAttached(string name)
  {//TODO sécuriser un peu tout ça, envoyer des exceptions si la méthode n'existe pas, si _caller est null
    DialogPostAction[] scripts = _caller.GetComponents<DialogPostAction>();
    DialogPostAction scriptToExecute = scripts[_dialogXML.dialog[_index].id-1];//Les id commencent à partir de 1 (car 0 est la valeur par défaut) mais les scripts commencent à 0 !

    Type t = scriptToExecute.GetType();
    MethodInfo function = t.GetMethod(name);
    function.Invoke(scriptToExecute, null);
  }

  private void ExecutePostActionNotAttached(string scriptName, string functionName)
  {//TODO sécuriser un peu tout ça, envoyer des exceptions si la méthode n'existe pas
    DialogPostAction scriptToExecute = _caller.GetComponent<DialogPostAction>();
    if(scriptToExecute != null)
    {
      Type t = scriptToExecute.GetType();
      MethodInfo function = t.GetMethod(functionName);
      function.Invoke(scriptToExecute, null);
    }
    else
    {
      Type t = Type.GetType(scriptName);
      MethodInfo function = t.GetMethod(functionName);
      object o = Activator.CreateInstance(t);
      function.Invoke(o, null);
    }
  }

  /**
   * Privilégier l'utilisation de StartDialog avec Caller. Ceci peut-etre utiliser pour "hacker" postAction mais ce n'est pas conseillé.
   **/
  public void SetCaller(DefaultDialogLauncher caller)
  {
    _caller = caller;
  }


  public UnityEngine.Object[] GetAllDialogsForCategory(string category)
  {
    //TODO Retourne tous les asset qui sont présents dans le dossier de la catégorie. S'il y a des sous-dossiers ça ne marche pas car ensuite, on ne 
    //peut plus récupérer son path complet.

    return Resources.LoadAll(_dialogFolderPath+category);
  }

  /*private List<string> GetAllFiles(string root)
  {
    List<string> allFiles = new List<string>();

    foreach(string file in Directory.GetFiles(root))
      allFiles.Add(file);

    foreach(string directory in Directory.GetDirectories(root))
    {
      List<string> subFiles = GetAllFiles(directory);
      allFiles.AddRange(subFiles);
    }

    return allFiles;
  }*/

}

