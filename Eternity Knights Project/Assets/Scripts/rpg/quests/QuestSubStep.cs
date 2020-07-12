using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class QuestSubStep
{
//ATTRIBUTS DU XML--------------------------------------------------------------
  [XmlAttribute("id")]
  public string id;
  
  [XmlAttribute("validationCount")]
  public int validationCount;
  
  [XmlAttribute("startupFunction")]
  public string startupFunction;//fction à lancer à l'activation (vérification continue,...) +> + un moyen d'arrêter la coroutine!
  
  [XmlAttribute("postAction")]
  public string postAction;
//------------------------------------------------------------------------------
  
  private int _currentCount=0;
  
  public bool Validated()
  {
    return _currentCount>=validationCount; 
  }
  
  public void CallStartupFunction()
  {
  	if(startupFunction!=null)
  	{
  	  string[] functionInfo=startupFunction.Split(':');
      Utils.ExecuteNotAttachedScriptFunction(functionInfo[0],functionInfo[1]);
    }
  }
  
  public void Increment(int value)
  {
    _currentCount++;
  }
}