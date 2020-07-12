using UnityEngine;
using System;
using System.Collections.Generic;

/**
* Classe centralisant les opérations de pathfinding sur les routes.
**/
public class RoadsPathfinding
{
  /**
  * Recherche de chemin sur le réseau routier pour se déplacer entre deux points
  * en évitant les autres personnages. Cette recherche est destinée à la réalisation
  * effective de portions de mouvement, car elle est plus coûteuse que le RouteStar, 
  * et il ne serait dont pas une bonne idée de la faire tourner sur tout le réseau
  * d'un coup à profondeur maximale.
  **/
  public static Stack<RoadData> RoadPathStar(RoadData goal,RoadData start,int keyPointsSpacing,string startOrientation,RoadData finalDestination)
  {
  	if(goal==null) return null;
  	
  	SearchData searchData=new SearchData(goal,start,keyPointsSpacing,startOrientation);
  	
  	Func<RoadData,RoadData,bool> goalFunction= goal==finalDestination ? (Func<RoadData,RoadData,bool>)RouteStarGoal : (Func<RoadData,RoadData,bool>)NearlyAtGoal;
  	
    return AStar(searchData,7*keyPointsSpacing,RoadPathStarNeighborhood,RoadPathStarCost,RoadPathStarRebuild,goalFunction);//TODO "7*" à ajuster si nécessaire, c'est arbitraire au possible. 
  }
  
  public static bool NearlyAtGoal(RoadData currentRoad,RoadData goal)//TODO ça ne sert pas en fait... Supprimer !
  { 
    return currentRoad==goal;// || RealDistanceBetween(currentRoad,goal)<=3;//TODO 3 arbitraire => Cette valeur doit être strictement inférieure à l'espace entre les keypoints, sinon ce-dernier ne sert à rien!!
  }
	
  public static float RoadPathStarCost(SearchData searchData,RoadData toEvaluate,int step)
  {
    /*
    coût=
    	distance au goal à vol d'oiseau
    	+
    	nombre (genre 2 ou 3) * nombre d'occupants 
    	+
    		constante (aucune idée de combien...)
    		*
    			nbre de réservations sur toEvaluate
    			/
    			distance entre la case à évaluer et la case courante (paramètre à ajouter, donc...)   	
    */
    
    if(toEvaluate.transform.position!=searchData.goal.transform.position)
    	return  Vector2.Distance(searchData.goal.transform.position,toEvaluate.transform.position)
    		+ 2*toEvaluate.roadLock.locksNber
    		+ 2*(toEvaluate.roadLock.reservationsNber/Vector2.Distance(toEvaluate.transform.position,searchData.start.transform.position))
    		+step;
    else return  0.0f;
  }
	
  public static Stack<RoadData> RoadPathStarRebuild(SearchData searchData)
  {
    Stack<RoadData> rslt=null;
    
    RoadData currentAccess=null;
    if(searchData.accessRoads.TryGetValue(searchData.goal.transform.position,out currentAccess))
    {
      rslt=new Stack<RoadData>();	
    
      rslt.Push(searchData.goal);
      
      while(currentAccess!=searchData.start)
      {
        rslt.Push(currentAccess);
        currentAccess=searchData.accessRoads[currentAccess.transform.position]; //Au cas où le goal est juste à côté.
      }
    }
    
    return rslt;  
  }
	
  public static List<RoadData> RoadPathStarNeighborhood(SearchData searchData,RoadData currentRoad)
  {//TODO: diagonales!!
  	List<RoadData> rslt;
    if(searchData.start==currentRoad) rslt=currentRoad.AccessibleFreeNeighbors(searchData.startOrientation);
    else rslt=currentRoad.Neighbors();

    return rslt;
  }
	
  /**
  * Effectue une recherche d'itinéraire entre deux points du réseau routier.
  **/
  public static Stack<RoadData> RouteStar(RoadData goal,RoadData start,int keyPointsSpacing,string startOrientation)
  {
  	if(goal==null) return null;
  	  
  	SearchData searchData=new SearchData(goal,start,keyPointsSpacing,startOrientation);  
    return AStar(searchData,-1,RouteStarNeighborhood,RouteStarCost,RouteStarRebuild,RouteStarGoal);
  }
  
  public static bool RouteStarGoal(RoadData currentRoad,RoadData goal)
  {
    return currentRoad==goal;  
  }
	
  public static float RouteStarCost(SearchData searchData,RoadData toEvaluate,int step)
  {
  	if(toEvaluate==null || searchData.goal==null)//Car la route peut avoir été détruite, il faut donc vérifier!!
  	  return Single.PositiveInfinity;
  	  
    return  Vector2.Distance(searchData.goal.transform.position,toEvaluate.transform.position) 
    		+ step
    		+ 4*Density(toEvaluate); 
  }
  
  private static float Density(RoadData toEvaluate)
  {
    float rslt=toEvaluate.roadLock.locksNber;
    foreach(RoadData neighbor in toEvaluate.Neighbors())
    {
      rslt+=neighbor.roadLock.locksNber;	
    }
    
    return rslt;
  }
  
  public static Stack<RoadData> RouteStarRebuild(SearchData searchData)
  {
    Stack<RoadData> rslt=null;
    
    RoadData currentAccess=null;
    if(searchData.accessRoads.TryGetValue(searchData.goal.transform.position,out currentAccess))
    {
      rslt=new Stack<RoadData>();	
    
      rslt.Push(searchData.goal);
      
      int counter=0; //TODO ajuster ça un peu mieux et mettre un random
      while(currentAccess!=searchData.start)
      {
        if(counter%searchData.keyPointsSpacing==0) rslt.Push(currentAccess);
        currentAccess=searchData.accessRoads[currentAccess.transform.position];
        counter++;
      }
    }
    
    return rslt;  
  }
	
  public static List<RoadData> RouteStarNeighborhood(SearchData searchData,RoadData currentRoad)
  {
    return currentRoad.Neighbors(includeNeverAccessible:false);	  
  }
	
  //Si maxDepth vaut -1, pas de profondeur max
  public static Stack<RoadData> AStar(SearchData searchData,int maxDepth,Func<SearchData,RoadData,List<RoadData>> neighborhoodFunction,Func<SearchData,RoadData,int,float> costFunction,Func<SearchData,Stack<RoadData>> rebuildFunction,Func<RoadData,RoadData,bool> goalFunction)
  { 
  	if(goalFunction(searchData.start,searchData.goal)) return new Stack<RoadData>();
  	  
  	  
    PriorityQueue<float,RoadData> priorityQueue=new PriorityQueue<float,RoadData>();   
    HashSet<RoadData> treated=searchData.visited;
    Dictionary<Vector3,RoadData> accessRoads=new Dictionary<Vector3,RoadData>();
    
    searchData.accessRoads=accessRoads;
    
    treated.Add(searchData.start);
    RoadData currentRoad=searchData.start;
     
    bool stop=false;
    int step=1;
    bool noMaxDepth=maxDepth==-1;
    while(!goalFunction(currentRoad,searchData.goal) && !stop  && (noMaxDepth || step<=maxDepth))
    {
      foreach(RoadData neighbor in neighborhoodFunction(searchData,currentRoad))
      {
        if(!treated.Contains(neighbor))
        {
          treated.Add(neighbor);	
          accessRoads.Add(neighbor.transform.position,currentRoad);
          priorityQueue.Push(costFunction(searchData,neighbor,step),neighbor);
        }
      }
      
      stop=priorityQueue.IsEmpty();
      step++;
      
      if(!stop)
        currentRoad=priorityQueue.Pop();
    }
    
    if(currentRoad==searchData.goal)
      return rebuildFunction(searchData);
    else 
      return null;
  }
  
  public static float RealDistanceBetween(RoadData start,RoadData goal)
  {
  	SearchData searchData=new SearchData(goal,start,4,Orientation.SOUTH);//TODO Le 4 est ici totalement arbitraire, il n'est pas utilisé. + Orientation a définir en fonction de celle du bâtiment.
    Stack<RoadData> path=RoadsPathfinding.AStar(searchData,-1,RouteStarNeighborhood,RouteStarCost,RouteStarRebuild,RouteStarGoal);
    
    return path==null ? -1.0f : (float)path.Count;
  }
}
