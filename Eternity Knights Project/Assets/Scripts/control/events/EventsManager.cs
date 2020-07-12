using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/**
* Gestionnaire d'évènements, conçu pour être utilisé comme un singleton.
**/
public class EventsManager
{
  /*
  Contient l'instance de l'EventManager à utiliser. Le champ instance 
  permet d'y accéder de façon sûre, en lecture seule.
  */
  private static EventsManager _realInstance;
  
  //Contient les différents évènements enregistrés, associés à leur nom.
  private Dictionary<string,UnityEvent> _eventsDictionary;
  
  public static EventsManager instance
  {
    get
    {
      if(_realInstance==null) _realInstance=new EventsManager();
      
      return _realInstance;
    }
  }
  
  protected EventsManager()
  {
    _eventsDictionary=new Dictionary<string,UnityEvent>();  
  }
  
  /**
  * Ajoute un écouteur à un évènement, en créant l'événement s'il n'existe pas 
  * encore.
  **/
  public static void AddListener(string eventName,UnityAction toExecute)
  {
    UnityEvent relatedEvent=null;
    if(!instance._eventsDictionary.TryGetValue(eventName,out relatedEvent))
    {
      relatedEvent=new UnityEvent();
      instance._eventsDictionary.Add(eventName,relatedEvent);
    }
    
    relatedEvent.AddListener(toExecute); 
  }
  
  /**
  * Retire un écouteur d'un évènement, s'il existe.
  **/
  public static void RemoveListener(string eventName,UnityAction toRemove)
  {
    UnityEvent relatedEvent=null;
    if(instance._eventsDictionary.TryGetValue(eventName,out relatedEvent))
    {
      relatedEvent.RemoveListener(toRemove);	
    }
  }
  
  /**
  * Déclenche un évènement.
  **/
  public static void Trigger(string eventName)
  {
    UnityEvent targetEvent=null;
    if(instance._eventsDictionary.TryGetValue(eventName,out targetEvent))
      targetEvent.Invoke();
  }
}