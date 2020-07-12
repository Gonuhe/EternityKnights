using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System;

public class QuestCondition
{
  public const string QUEST_DONE_CONDITION="quest";
  public const string STEP_DONE_CONDITION="stepDone";
  public const string STEP_ACTIVE_CONDITION="stepActive";
  public const string CHARACTER_ALIVE_CONDITION="characterAlive";
  public const string NARRATIVE_PROPERTY_CONDITION="narrativeProperty";
  public const string ITEM_ACQUIRED_CONDITION="objectAcquired";
  public const string AND_CONDITION="and";
  public const string OR_CONDITION="or";
	
//ATTRIBUTS DU XML--------------------------------------------------------------
  [XmlAttribute("type")]
  public string type;
  
  [XmlAttribute("mustBe")]
  public bool mustBe=true;
  
  [XmlAttribute("arg")]
  public string arg;
  
  [XmlArray("subConditions")]
  [XmlArrayItem("condition")]
  public List<QuestCondition> subConditions;
//------------------------------------------------------------------------------
  
  private QuestManager _questManager;
  
  public QuestCondition()
  {
    _questManager=GameManager.instance.RPGData.questManager;
  }
  
  public bool Evaluate()
  {
    switch(type)
    {
      case QUEST_DONE_CONDITION: return EvaluateQuestDoneCondition()==mustBe;
      case STEP_DONE_CONDITION: return EvaluateStepDoneCondition()==mustBe;
      case STEP_ACTIVE_CONDITION: return EvaluateStepActiveCondition()==mustBe;	  
      case CHARACTER_ALIVE_CONDITION: return EvaluateCharacterAliveCondition()==mustBe;
      case NARRATIVE_PROPERTY_CONDITION: return EvaluateNarrativePropertyCondition()==mustBe;
      case ITEM_ACQUIRED_CONDITION: return EvaluateItemAcquiredCondition()==mustBe;
      case AND_CONDITION: return EvaluateAndCondition()==mustBe;
      case OR_CONDITION: return EvaluateOrCondition()==mustBe;
      default: return false;
    }
  }
  
  private bool EvaluateQuestDoneCondition()
  {
    return _questManager.QuestAccomplished(arg);
  }
  
  private bool EvaluateStepDoneCondition()
  {
  	string[] args=arg.Split(':');
  	return _questManager.QuestStepDone(args[0],args[1]);
  }
  
  private bool EvaluateStepActiveCondition()
  {
    string[] args=arg.Split(':');
  	return _questManager.QuestStepActive(args[0],args[1]);
  }
  
  private bool EvaluateCharacterAliveCondition()
  {
  	return _questManager.CharacterAlive(arg);
  }
  
  private bool EvaluateNarrativePropertyCondition()
  {//Si on a un : dans l'arg, c'est qu'on appelle une propriété locale. Pour une globale, l'arg est directement l'id  
    if(arg.Contains(":"))
    {
      string[] args=arg.Split(':');
      return _questManager.GetLocalNarrativeProperty(args[0],args[1]);
    }

  	return _questManager.GetGlobalNarrativeProperty(arg);  
  }
  
  private bool EvaluateItemAcquiredCondition()
  {
    return GameManager.instance.RPGData.inventory.HaveItem(Int32.Parse(arg));  
  }
  
  private bool EvaluateAndCondition()
  {
  	foreach(QuestCondition condition in subConditions)
  	{
      if(!condition.Evaluate()) return false;	
  	}

  	return true;
  }
  
  private bool EvaluateOrCondition()
  {
    foreach(QuestCondition condition in subConditions)
  	{
      if(condition.Evaluate()) return true;	
  	}

  	return false;
  }
}