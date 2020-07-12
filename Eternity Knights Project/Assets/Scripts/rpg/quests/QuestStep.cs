using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class QuestStep
{ 
//Attributs du XML--------------------------------------------------------------
  [XmlArray("preconditions")]
  [XmlArrayItem("condition")]
  public List<QuestCondition> preconditions; 
  
  [XmlArray("invariants")]
  [XmlArrayItem("condition")]
  public List<QuestCondition> invariants;
  
  [XmlAttribute("id")]
  public string id;
  
  [XmlAttribute("preAction")]
  public string preAction;
  
  [XmlAttribute("postAction")]
  public string postAction;
  
  public string postGoalDescription;
  
  public string goalDescription;
  
  [XmlArray("subSteps")]
  [XmlArrayItem("subStep")]
  public List<QuestSubStep> subStepsList; 
//------------------------------------------------------------------------------
  
  private Dictionary<string,QuestSubStep> _subSteps=new Dictionary<string,QuestSubStep>();
  
  public bool EvaluateSpecs()
  {
    return EvaluatePreconditions() && EvaluateInvariants();
  }
  
  public bool EvaluatePreconditions()
  {
    foreach(QuestCondition pre in preconditions)
    {
      if(!pre.Evaluate()) return false;	
    }
    
    return true;
  }
  
  public bool EvaluateInvariants()
  {
    foreach(QuestCondition inv in invariants)
    {
      if(!inv.Evaluate()) return false;
    }
    
    return true;
  }
  
  public void Begin()
  {
  	CallPreAction();
  	
    foreach(QuestSubStep subStep in _subSteps.Values)
    {
      subStep.CallStartupFunction();
    }
  }
  
  public void End()
  {
    CallPostAction();
  }
  
  public void IncrementSubStep(string id,int value)
  {
    _subSteps[id].Increment(value);
  }
  
  public bool Validated()
  {
    foreach(QuestSubStep subStep in _subSteps.Values)
    {
      if(!subStep.Validated()) return false;	
    }
    
    return true;
  }
  
  private void CallPostAction()
  {
    if(postAction!=null)
  	{
      string[] functionInfo=postAction.Split(':');
      Utils.ExecuteNotAttachedScriptFunction(functionInfo[0],functionInfo[1]);
    }  
  }
  
  private void CallPreAction()
  {
    if(preAction!=null)
  	{
      string[] functionInfo=preAction.Split(':');
      Utils.ExecuteNotAttachedScriptFunction(functionInfo[0],functionInfo[1]);
    }  
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
    foreach(QuestSubStep subStep in subStepsList)
    {
      _subSteps[subStep.id]=subStep;
    }
  }
}