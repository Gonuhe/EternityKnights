using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class FreightAreaIn : MonoBehaviour
{  
  //RoadData de la route servant de zone de frêt
  [HideInInspector]
  public RoadData road;
  
  [HideInInspector]
  public FreightAreaEvent inEvent=new FreightAreaEvent();
  
  private HashSet<Collider2D> _freightAreaDataTreatedColliders;
    
  protected void Awake()
  {
    road=GetComponent<RoadData>();  
    _freightAreaDataTreatedColliders=GetComponentInParent<FreightAreaData>().treatedColliders;
  }
  
  /*
  Fait dans OnTriggerStay, car il faut attendre qu'ils soient à l'arrêt dans la 
  zone ; gérer ça avec OnTriggerEnter2D et attendre la fin du mouvement n'est 
  PAS une bonne idée, car si le collider d'un perso de passage effleure la case, 
  il déclenchera le code en question, ce qui est TRES MAUVAIS!!!!
  */
  void OnTriggerStay2D(Collider2D other)
  {
  	RoadRouteManager roadRouteManager=other.GetComponent<RoadRouteManager>();
  	MoveManager moveManager=other.GetComponent<MoveManager>();
    
    //Condition assez longue, mais nécessaire pour éviter les bugs dus à des collisions partielles avec certains colliders qui "effleureent" la zone de frêt
    if(!_freightAreaDataTreatedColliders.Contains(other) && roadRouteManager!=null && roadRouteManager.occupiedRoad==road && !moveManager.IsMoving())
    {      
      inEvent.Invoke(other);
      _freightAreaDataTreatedColliders.Add(other);
    }
  }
}