using UnityEngine;
using System.Collections;

public class QuestUITrigger : MonoBehaviour
{
  private bool visible = false;

	// Use this for initialization
	void Start ()
  {
	}
	
	// Update is called once per frame
	void Update ()
  {
	  if(Input.GetButtonDown("Fire1"))
    {
      showQuestUIPanel(!visible);
    }
	}

  void showQuestUIPanel(bool b)
  {
    if(b)
    {
      GameManager.instance.StackGameMode(GameModes.QUEST_UI_MODE);
    }
    else
    {
      GameManager.instance.UnstackGameMode(GameModes.QUEST_UI_MODE);
    }
    visible = b;
  }


}
