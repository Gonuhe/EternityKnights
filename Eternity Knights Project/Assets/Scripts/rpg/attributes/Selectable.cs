using UnityEngine;
using UnityEngine.Events;

/**
* Attribut à attacher à tous les objets sélectionnables par le joueur. 
**/
public class Selectable : MonoBehaviour
{
  public UnityEvent examineEvent=new UnityEvent();
  //TODO ajouter des events pour les autres types d'interactions avec des objets
  
  public void Examine()
  {
    examineEvent.Invoke();
  }
}