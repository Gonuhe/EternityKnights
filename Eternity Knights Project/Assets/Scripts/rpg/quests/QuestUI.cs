using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization;

public class QuestUI : MonoBehaviour
{
  public Text questTextField;
  public RectTransform content;
  public Scrollbar scrollBar;
  public QuestDetailsUI questDetailsUI;

  private QuestManager _questManager;

  protected void Awake()
  {
    GameModes.QUEST_UI_MODE.uIPanel = this;
  }

	// Use this for initialization
	protected void Start ()
  {
    _questManager = GameManager.instance.RPGData.questManager;
    questTextField.text = "     "+LanguageManager.Instance.GetTextValue("Main.Quests");

    gameObject.SetActive(false);
	}


  public void LoadScrollView()
  {
    Dictionary<string,Quest> activeQuests = _questManager.activeQuests;
    HashSet<string> finishedQuests = _questManager.accomplishedQuestsId;

    int contentSize = activeQuests.Count + finishedQuests.Count;
    content.sizeDelta = new Vector2(content.sizeDelta.x, contentSize*30);
    int i = 0;
    foreach(string activeQuestId in activeQuests.Keys)
    {
      AddQuestToScrollView(activeQuestId, i);
      i++;
    }

    foreach(string finishedQuest in finishedQuests)
    {
      AddQuestToScrollView(finishedQuest, i);
      i++;
    }
  }

  private void AddQuestToScrollView(string questId, int questNumberInView)
  {
    //On récupère les informations dont on a besoin pour afficher dans la scrollview
    Quest quest;
    if(_questManager.QuestActive(questId))
      _questManager.activeQuests.TryGetValue(questId,out quest);
    else //TODO finishedQuest : plutôt avoir une cache avec juste les noms et les id (dummy) si jamais on clique sur le bouton on affiche les détails
    {
      quest = new Quest();
      quest.id = questId;
      quest.name = questId;
    }

    //On créée le boutton
    Button questButton = GameObject.Instantiate(Resources.Load<Button>("Prefabs/QuestButton"));
    questButton.transform.SetParent(content,false);

    Vector3 position = questButton.GetComponent<RectTransform>().localPosition;//Calculer la position du boutton
    position.y += questNumberInView*-30;
    questButton.GetComponent<RectTransform>().localPosition = position;

    questButton.GetComponentInChildren<Text>().text = quest.name;
    if(_questManager.QuestActive(questId))
      questButton.image.color = Color.green;

    questButton.onClick.AddListener(() => ShowQuestDetails(questId));
  }


  private void ShowQuestDetails(string questId)
  {
    if(_questManager.QuestActive(questId))
    {
      Quest quest;
      if(_questManager.activeQuests.TryGetValue(questId,out quest))
        questDetailsUI.DisplayQuest(quest);
    }
    //TODO else : les finished quest
  }

}
