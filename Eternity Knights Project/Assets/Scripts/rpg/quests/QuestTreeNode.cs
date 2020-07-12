using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class QuestTreeNode
{
//ATTRIBUTS DU XML--------------------------------------------------------------
  [XmlAttribute("stepId")]
  public string stepId;
  
  [XmlArray("children")]
  [XmlArrayItem("child")]
  public List<QuestTreeNode> children;
//------------------------------------------------------------------------------
}