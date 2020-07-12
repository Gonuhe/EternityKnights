using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("Quest")]
public class Quest
{
//ATTRIBUTS DU XML--------------------------------------------------------------
  [XmlAttribute("id")]
  public string id;
  
  [XmlAttribute("name")]
  public string name;
  
  public string description;
  
  [XmlAttribute("postAction")]
  public string postAction;
  
  [XmlAttribute("hidden")]
  public bool hidden=false;
  
  [XmlArray("preconditions")]
  [XmlArrayItem("condition")]
  public List<QuestCondition> preconditions;
  
  [XmlArray("invariants")]
  [XmlArrayItem("condition")]
  public List<QuestCondition> invariants;
  
  public QuestTree questTree;
//------------------------------------------------------------------------------
  
  private HashSet<string> _narrativePropertiesEqualToTrue=new HashSet<string>();
  
  public bool EvaluatePreconditions()
  {
  	foreach(QuestCondition pre in preconditions)
  	{
  	  if(!pre.Evaluate()) return false;
  	}
  		
    return questTree.QuestStepRoot().EvaluatePreconditions();
  }
  
  public bool EvaluateInvariants()
  {
  	foreach(QuestCondition inv in invariants)
  	{
  	  if(!inv.Evaluate()) return false;
  	}
  		
    return questTree.QuestStepRoot().EvaluateInvariants();
  }
  
  public void Begin()
  {
    questTree.QuestStepRoot().Begin();  
  }
  
  /**
  * Incrémente la substep de value si step est active. 
  * Cet appel peut donc faire progresser la quête.
  **/
  public void IncrementSubStepIfActive(string step,string subStep,int value)
  {
    if(questTree.StepActive(step))
      questTree.IncrementCurrent(subStep,value);  
  }
  
  /**
  * Appelée juste après l'activation de la quête.
  **/
  public void StartQuest()
  {
    questTree.QuestStepRoot().Begin();
  }
  
  /**
  * Appelée juste après l'accomplissement de la quête.
  **/
  public void EndQuest()
  {
  	CallPostAction();
  }
  
  private void CallPostAction()
  {
    if(postAction!=null)
  	{
      string[] functionInfo=postAction.Split(':');
      Utils.ExecuteNotAttachedScriptFunction(functionInfo[0],functionInfo[1]);
    }  
  }
  
  public bool StepActive(string step)
  {
    return questTree.StepActive(step);
  }
  
  public bool StepDone(string step)
  {
    return questTree.StepDone(step);
  }
  
  public bool Accomplished()
  {
    return questTree.treeTraversalFinished;
  }
  
  public bool GetLocalNarrativeProperty(string property)
  {
    return _narrativePropertiesEqualToTrue.Contains(property);
  }
  
  public void SetLocalNarrativeProperty(string property, bool value)
  {
    if(!value) _narrativePropertiesEqualToTrue.Remove(property);
    else if(!_narrativePropertiesEqualToTrue.Contains(property)) _narrativePropertiesEqualToTrue.Add(property);
  }
  
  /**
  * Se charge de convertir les champs chargés depuis le XML en des données plus 
  * utilisables (dictionnaires au lieu de listes, ...).
  * C'aurait été directement possible avec des propriétés si Microsoft n'avait
  * pas programmé l'API de C# avec le cul ! (et en Inde!!!) Du coup ça ne l'est
  * pas car les champs de collections sont assignés à une nouvelle collection,
  * PUIS celle-ci est remplie, au lieu de remplir avant d'assigner, comme l'aurait
  * voulu le bon sens... Sérieux, c'est des erreurs de première bac, ça ! ><
  **/
  public void FinaliseXMLData()
  {
    questTree.FinaliseXMLData();    
  }
}