using UnityEngine;
using System.IO;
using System.Xml.Serialization;


public class TestQuestXML : MonoBehaviour
{
  protected void Start()
  {
  	GameManager.instance.RPGData.questManager.TryToActivateQuest("testMk2");
    /*TextAsset asset = Resources.Load("Test/quests/test") as TextAsset;
    Stream stream = new MemoryStream(asset.bytes);
      
    XmlSerializer serializer = new XmlSerializer(typeof(Quest));
    Quest rslt = serializer.Deserialize(stream) as Quest;
    stream.Close();*/
    
    //Debug.Log("ROOT  "+rslt.questTree.root.children.Count);
    /*foreach(QuestCondition pre in rslt.preconditions)
    {
      Debug.Log("COUNT  "+pre.subConditions.Count);
    }*/
  }
}