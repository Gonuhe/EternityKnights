using System.Collections.Generic;
using UnityEngine;

public class SearchData
{
  //Champs à fournir pour lancer la recherche
  public RoadData goal;
  public RoadData start;
  public int keyPointsSpacing;
  public string startOrientation;
  
  //Champ initialisé par les fonctions de recherche en cours de route
  public Dictionary<Vector3,RoadData> accessRoads;
  
  //Ce champ peut être assigné dès le départ à un ensemble non-vide pour forcer l'algo à ignorer des cases
  public HashSet<RoadData> visited=new HashSet<RoadData>();
  
  public SearchData(RoadData goal, RoadData start, int keyPointsSpacing,string startOrientation)
  {
    this.goal=goal;
    this.start=start;
    this.keyPointsSpacing=keyPointsSpacing;
    this.startOrientation=startOrientation;
  }
}