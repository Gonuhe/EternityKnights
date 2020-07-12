using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChoiceManager : MonoBehaviour
{

  public static ChoiceManager instance;

  private const int MAX_CHOICES_NUMBER = 3; // arbitraire. à changer en fonction du nombre de choix qu'on décide, ne pas oublier de modifier les objets dans la scene en conséquence.
  private int _choicesNumber = 0;
  private List<Choice> _choices;

  private Text[] _choicesText;
  

  private int _selected = 0;
  

  // Use this for initialization
  void Start ()
  {
    instance = this;
    _choicesText = new Text[MAX_CHOICES_NUMBER];
    for(int i = 0; i < MAX_CHOICES_NUMBER; i++)
    {
      _choicesText[i] = transform.GetChild(i).GetComponent<Text>();
    }
    gameObject.SetActive(false);//Une fois que je me suis bien initialisé, je me désactive car on a besoin de moi que s'il y a des choix, et c'est le dialogManager qui gère ça !
  }
  
  // Update is called once per frame
  void Update ()
  {
    if(_choicesNumber != 0)//Programmation défensive. Sert à éviter des bugs si on doit chipoter avec les setActive. 
    {
      if(Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0)
      {
        SelectChoice((_selected+1)%_choicesNumber);
      }
      if(Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0)
      {
        if(_selected-1 == -1)
          SelectChoice(_choicesNumber-1);
        else
          SelectChoice(_selected-1);
      }
    }
  }

  //TODO si choices.count > CHOICES_NUMBER raise error
  public void LoadChoices(List<Choice> choices)
  {
    _choices = choices;
    for(int i = 0; i < MAX_CHOICES_NUMBER && i < choices.Count; i++)
    {
      _choicesText[i].text = choices[i].text;
    }
    _choicesNumber = choices.Count;
    SelectChoice(0);
  }

  private void SelectChoice(int i)
  {
    if(i >= 0 && i < _choicesNumber)
    {
      _choicesText[_selected].text = _choicesText[_selected].text.Replace("> <i>","").Replace("</i>","");//TODO : bug si on souhaite mettre un mot en évidence en italique dans une phrase
      _choicesText[i].text = "> <i>"+_choicesText[i].text+"</i>";
      _selected = i;
    }
    else
    {
      //TODO raise exception
    }
  }

  /**
   * Retourne le choix sélectionné
   **/
  public Choice GetSelectedChoice()
  {
    return _choices[_selected];
  }

  /**
   * Retire l'écran de sélection de l'écran et retourne le choix sélectionné
   **/
  public Choice ValidateChoice()
  {
    for(int i = 0; i < _choicesNumber ; i++)
    {
      _choicesText[i].text = "";
    }
    return GetSelectedChoice();
  }

}
