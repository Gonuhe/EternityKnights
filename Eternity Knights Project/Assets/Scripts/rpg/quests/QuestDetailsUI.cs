using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestDetailsUI : MonoBehaviour
{

  private Quest displayingQuest;

  public Text questName;
  public Text questDescription;
  public Transform content;

  private QuestManager _questManager;

	void Start ()
  {
    _questManager = GameManager.instance.RPGData.questManager;
	}

  public void DisplayQuest(Quest quest)
  {
    CleanView();

    //ajouter les infos de la quêtes
    displayingQuest = quest;
    questName.text = displayingQuest.name;
    questDescription.text = displayingQuest.description;

    //Ajouter les queststep
    int questStepNumberInView = 0;
  
    List<QuestStep> questStepsDone = quest.questTree.GetDoneSteps();

    //prépaprer la view
    RectTransform contentRT = content.GetComponent<RectTransform>();
    int contentSize = questStepsDone.Count;
    contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, contentSize*20);

    foreach(QuestStep questStepDone in questStepsDone)
    {
      AddQuestStepToView(questStepDone, questStepNumberInView);
      questStepNumberInView++;
    }

    //ajouter la queststep active
    QuestStep activeStep = quest.questTree.GetActiveStep();
    if(_questManager.QuestActive(quest.id) && activeStep != null)
    {
      AddQuestStepToView(activeStep, questStepNumberInView);
    }
    questStepNumberInView++;

    StartCoroutine(FinalizeQuestStepScrollView(questStepNumberInView));
  }

  public void AddQuestStepToView(QuestStep questStep, int questStepNumberInView)
  {
    GameObject stepText = GameObject.Instantiate(Resources.Load("Prefabs/QuestStepText")) as GameObject;
    Text text = stepText.GetComponent<Text>();
    if(_questManager.QuestStepActive(displayingQuest.id, questStep.id))
    {
      text.text = CleanString(questStep.goalDescription);
      text.color = Color.red;
    }
    else
    {
      text.text = "v "+questStep.postGoalDescription == null ? CleanString(questStep.goalDescription) : CleanString(questStep.postGoalDescription);
    }

    RectTransform stepRectTransform = stepText.GetComponent<RectTransform>();
    stepRectTransform.SetParent(content);

    Vector3 position = stepRectTransform.localPosition;//Calculer la position
    position.y += questStepNumberInView*-20;
    stepRectTransform.localPosition = position;
  }

  public IEnumerator FinalizeQuestStepScrollView(int contentSize)
  {
    //Attendre 2 frames que la scrollbar se réactive et se mette à jour
    yield return null;
    yield return null;

    Scrollbar scrollBar = content.parent.parent.GetComponentInChildren<Scrollbar>();
    if(scrollBar != null)
      scrollBar.value = 0;//on affiche d'abord le bas qui contient la questStepActive
  }

  private void CleanView()
  {
    questName.text = "";
    questDescription.text = "";

    Scrollbar scrollBar = content.parent.parent.GetComponentInChildren<Scrollbar>();
    if(scrollBar != null)
      scrollBar.value = 1;


    foreach(Transform c in content.GetComponentsInChildren<Transform>())
      if(c != content)
        GameManager.instance.DestroyGameObject(c.gameObject);
  }

  private string CleanString(string toClean)
  {
    string rslt = toClean.Replace("\n","");
    rslt = rslt.Replace("  ", "");
    if(rslt[0] == ' ')
      rslt = rslt.Remove(0,1);//remove first char

    return rslt;
  }
}
