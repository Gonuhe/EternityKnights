using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class QuestTree
{ 
//ATTRIBUTS DU XML--------------------------------------------------------------
  [XmlArray("steps")]
  [XmlArrayItem("step")]
  public List<QuestStep> stepsList; 
  
  public QuestTreeNode root;
//------------------------------------------------------------------------------
  
  private Dictionary<string,QuestStep> _stepsDictionary=new Dictionary<string,QuestStep>();
  private HashSet<string> _stepsDone=new HashSet<string>();
  private QuestTreeNode _currentNode;
  
  [XmlIgnoreAttribute]
  public bool treeTraversalFinished=false;
  
  public QuestStep QuestStepRoot()
  {
    return _stepsDictionary[root.stepId];  
  }
  
  public QuestStep GetQuestStep(string id)
  {
    return _stepsDictionary[id];  
  }
  
  private IEnumerator<QuestStep> GetChildren(QuestTreeNode root)
  {
    foreach(QuestTreeNode nextNode in root.children)
    {
      yield return _stepsDictionary[nextNode.stepId];	
    }
  }
  
  //précondition: TreeTraversalFinished=false ; sinon,c'est que la quête est finie
  public void IncrementCurrent(string subStep,int value)
  {
    QuestStep currentStep=_stepsDictionary[_currentNode.stepId];
    currentStep.IncrementSubStep(subStep,value);  
    
    if(currentStep.Validated())
    {
      currentStep.End();
    	
      List<QuestTreeNode> availableNextNodes=new List<QuestTreeNode>();
      foreach(QuestTreeNode child in _currentNode.children)
      {
      	QuestStep childStep=_stepsDictionary[child.stepId];
      	
        if(childStep.EvaluateSpecs())
          availableNextNodes.Add(child);
      }
      
      int availableNextNodesCount=availableNextNodes.Count;
      if(availableNextNodesCount>0)
      {
      	_stepsDone.Add(currentStep.id);
      	_currentNode=availableNextNodes[Random.Range(0,availableNextNodesCount)];
      	
      	QuestStep newCurrent=_stepsDictionary[_currentNode.stepId];
      	newCurrent.Begin();
        //TODO un event quelconque pour indiquer le passage à la QuestStep suivante
      }
      else 
        treeTraversalFinished=true;
    }
  }

  public QuestStep GetActiveStep()
  {
    if(!treeTraversalFinished)
      return _stepsDictionary[_currentNode.stepId];
    else
      return null;
  }
  
  public bool StepActive(string stepId)
  {
    return !treeTraversalFinished && _currentNode.stepId==stepId;  
  }
  
  public bool StepDone(string stepId)
  {
    return _stepsDone.Contains(stepId);  
  }

  public List<QuestStep> GetDoneSteps()
  {
    Queue<QuestTreeNode> toVisit = new Queue<QuestTreeNode>();
    List<QuestStep> questStepDone = new List<QuestStep>();
    toVisit.Enqueue(root);
    while(toVisit.Count>0)
    {
      QuestTreeNode questStepNode = toVisit.Dequeue();
      QuestStep qs;
      if(_stepsDictionary.TryGetValue(questStepNode.stepId, out qs) && StepDone(qs.id))
      {
        questStepDone.Add(qs);
        foreach(QuestTreeNode child in questStepNode.children)
          toVisit.Enqueue(child);
      }
    }
    return questStepDone;
      
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
    foreach(QuestStep step in stepsList)
    {
      _stepsDictionary[step.id]=step;  
      step.FinaliseXMLData();
    }
    
    _currentNode=root;
  }
}