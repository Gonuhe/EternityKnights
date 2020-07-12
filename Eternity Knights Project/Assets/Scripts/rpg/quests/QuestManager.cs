using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/**
* Classe gérant tout l'accès aux données de quêtes et les appels à celles-ci par
* des entités extérieures.
**/
public class QuestManager
{
  public const string QUESTS_MAIN_DIR_PATH="Quests/";
	
  public HashSet<string> accomplishedQuestsId=new HashSet<string>();
  public Dictionary<string,Quest> activeQuests=new Dictionary<string,Quest>();
  private HashSet<string> _deadCharactersId=new HashSet<string>();
  private HashSet<string> _narrativePropertiesEqualToTrue=new HashSet<string>();//On stock uniquement celles qui valent true par souci d'économie de mémoire
  
  public QuestManager()
  {
    GameManager.instance.StartCoroutine(QuestManagementCoroutine());	  
  }
  
  /**
  * Charge en mémoire et active la quête d'id questId, à condition qu'elle n'ait
  * pas déjà été accomplie et que ses préconditions et son invariant soit remplis.
  * Si ces conditions ne sont pas respectées, cette méthode n'a aucun effet.
  **/
  public void TryToActivateQuest(string questId)
  {
  	if(!accomplishedQuestsId.Contains(questId))
  	{
      Quest newQuest=LoadQuest(questId);
      
      if(newQuest.EvaluatePreconditions() && newQuest.EvaluateInvariants())
      {
        activeQuests[questId]=newQuest;
        newQuest.StartQuest();
        //TODO notification ou quoi à l'écran, genre "quête ajoutée", mais uniquement si la quête n'est pas cachée
      }
    }
  }
  
  /**
  * Coroutine qui itère sur les quêtes pour vérifier que leurs invariants sont 
  * remplis, et les annule sinon. 
  * Cette coroutine boucle et vérifie une quête par frame (grosso modo, puisque
  * ce n'est pas tout-à-fait exact si on regarde bien les 2 yields qui peuvent 
  * s'enchaîner entre les boucles, mais bon, c'est du détail :p).
  **/
  private IEnumerator QuestManagementCoroutine()
  {
    while(true)
    {
      Dictionary<string,Quest> activeQuestsClone=new Dictionary<string,Quest>(activeQuests); //On clone pour éviter les erreurs dues à des modifications du ditctionnaire pendant la coroutine
      foreach(Quest quest in activeQuestsClone.Values)
      {
        if(!quest.EvaluateInvariants())
        {
          activeQuests.Remove(quest.id);
          //TODO une façon plus propre d'annuler ???
          //TODO notification comme quoi la quête n'est plus disponible, sauf si elle était cachée (pas d'appel à EndQuest, car 
        }
        
        yield return null;
      }
      
      yield return null;
    }
  }
  
  /**
  * Charge une quête depuis le XML la contenant. Ce XML DOIT se trouver dans
  * le répertoire référencé par le chemin QUESTS_MAIN_DIR_PATH et être nommé
  * questId.xml, où questId est l'identifiant de la quête à charger.
  **/
  private Quest LoadQuest(string questId)
  {
    TextAsset asset = Resources.Load(QUESTS_MAIN_DIR_PATH+questId) as TextAsset;
    Stream stream = new MemoryStream(asset.bytes);
      
    XmlSerializer serializer = new XmlSerializer(typeof(Quest));
    Quest rslt = serializer.Deserialize(stream) as Quest;
    stream.Close();
    
    rslt.FinaliseXMLData();
    
    return rslt;
  }
  
  /**
  * Retourne true ssi la quête questId a été accomplie avec sucès par le joueur.
  **/
  public bool QuestAccomplished(string questId)
  {
  	return accomplishedQuestsId.Contains(questId);  
  }
  
  /**
  * Retourne true ssi la quête questId est active, et donc en cours de résolution
  * (enfin, on espère !) par le joueur.
  **/
  public bool QuestActive(string id)
  {
    return activeQuests.ContainsKey(id);
  }
  
  /**
  * Retourne true ssi la quête questId est active, ainsi que son QuestStep stepId.
  **/
  public bool QuestStepActive(string questId,string stepId)
  {
  	bool rslt=false;
    Quest quest=null;
    if(activeQuests.TryGetValue(questId,out quest))
      rslt=quest.StepActive(stepId);
  
    return rslt;
  }
  
  /**
  * Retourne true si QuestStep stepId de la quête questId (qui est donc active)
  * a été exécuté OU si questId a déjà été intégralement accomplie 
  * (ATTENTION: comme les quêtes sont non-linéaires, une quête accomplie ne signifie
  * pas que toutes ses QuestStep l'ont été!! Pour des vérifications plus pointues,
  * il faut jouer avec des narrativeProperties globales).
  **/
  public bool QuestStepDone(string questId,string stepId)
  {
  	bool rslt=accomplishedQuestsId.Contains(questId);
  	
    Quest quest=null;
    if(!rslt && activeQuests.TryGetValue(questId,out quest))
      rslt=quest.StepDone(stepId);
  
    return rslt;
  }
  
  /**
  * Incrémente un QuestSubStep de value à condition que sa quête et sa QuestStep
  * soient actives. Cet appel fait donc progresser la quête (toujours sous les
  * conditions données précédemment), et peut potentiellement la terminer.
  **/
  public void TryToIncrement(string questId,string step,string subStep,int value)
  {
    Quest quest=null;
    if(activeQuests.TryGetValue(questId,out quest))
    {
      quest.IncrementSubStepIfActive(step,subStep,value);
      
      if(quest.Accomplished())
      {
        quest.EndQuest();
        activeQuests.Remove(questId);
        accomplishedQuestsId.Add(questId);
        //TODO event à déclencer pour indiquer la fin de la quête dans l'interface
      }
    }
  }
  
  /**
  * Retourne true ssi le personnage identifié par id est toujours en vie.
  **/
  public bool CharacterAlive(string id)
  {
    return !_deadCharactersId.Contains(id);  
  }
  
  /**
  * Marque le personnage identifié par id comme mort dans les données de quête.
  **/
  public void KillCharacter(string id)
  {
    _deadCharactersId.Add(id);  
  }
  
  public bool GetGlobalNarrativeProperty(string property)
  {
    return _narrativePropertiesEqualToTrue.Contains(property);
  }
  
  public void SetGlobalNarrativeProperty(string property, bool value)
  {
    if(!value) _narrativePropertiesEqualToTrue.Remove(property);
    else if(!_narrativePropertiesEqualToTrue.Contains(property)) _narrativePropertiesEqualToTrue.Add(property);
  }
  
  public bool GetLocalNarrativeProperty(string questId,string property)
  {
  	Quest quest=null;
  	if(activeQuests.TryGetValue(questId,out quest))
      return quest.GetLocalNarrativeProperty(property);
  
    return false;//faire attention avec ça, si on retourne false, ça peut être parce que la quête n'est pas active OU parce que la propriété et fausse dans la quête active!!!
  }
  
  public void SetLocalNarrativeProperty(string questId,string property, bool value)
  {
  	Quest quest=null;
  	if(activeQuests.TryGetValue(questId,out quest))
      quest.SetLocalNarrativeProperty(property,value);
  }
}