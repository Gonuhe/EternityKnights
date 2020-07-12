using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Classe responsable de centraliser les données et opérations propres à la gestion
* des ressources de la ville et de la façon dont elles sont entreposées.
**/
public class StockManager
{
  //Contient tous les entrepôts de la ville
  private HashSet<Warehouse> _warehouses=new HashSet<Warehouse>();
  
  //Contient tous les batiments de la ville capables de recevoir une commande de marchandises à livrer (entrepôts et industries, donc)
  private HashSet<OrderManager>  _orderManagers=new HashSet<OrderManager>();
  
  //Contient tous les batiments de la ville capables de recevoir une commande de marchandises à garder sur place (par exemple, agoras, donc)
  private HashSet<KeepAsideOrderManager> _keepAsideOrderManagers=new HashSet<KeepAsideOrderManager>();
  
  /**
  * Coroutine à lancer pour effectuer une recherche parmi les entrepôts disponibles 
  * quel est le plus proche remplissant une condition donnée. Pour les calculs
  * de distance, ce sont ici les freight area d'ENTREE (in) qui sont utilisées, ATTENTION!
  **/
  public static IEnumerator FindNearestWarehouseSatisfyingCondition(RoadData startPoint,Func<Warehouse,bool> acceptanceFunction,Cell<Warehouse> rslt,HashSet<FreightAreaIn> toIgnore)
  {
    PriorityQueue<float,Warehouse> sortedWarehouses=new PriorityQueue<float,Warehouse>();
    IEnumerator<Warehouse> allWarehouses=GameManager.instance.cityBuilderData.stockManager.AllWarehouses();
    while(allWarehouses.MoveNext())
    {
      Warehouse warehouse=allWarehouses.Current;
      
      if(warehouse!=null)// Car il peut avoir été détruit depuis le début de la coroutine !
      {
        FreightAreaIn warehouseIn=warehouse.orderManager.freightAreaData.freightAreaIn;
        if(!toIgnore.Contains(warehouseIn) && acceptanceFunction(warehouse))
        {
          float distanceToWarehouse=Vector2.Distance(warehouseIn.transform.position,startPoint.transform.position);
          sortedWarehouses.Push(distanceToWarehouse,warehouse);
        }
      }
    }
    
    if(!sortedWarehouses.IsEmpty())
    {
      Warehouse bestWarehouse=sortedWarehouses.Pop();
      float pathDistance=RoadsPathfinding.RealDistanceBetween(startPoint,bestWarehouse.orderManager.freightAreaData.freightAreaIn.road);
      
      yield return null;
      
      while(!sortedWarehouses.IsEmpty() && (pathDistance>sortedWarehouses.LowestKey() || pathDistance<0.0f))
      {
        Warehouse nearestAsTheCrowFlies=sortedWarehouses.Pop();
        float newPathDistance=RoadsPathfinding.RealDistanceBetween(startPoint,nearestAsTheCrowFlies.orderManager.freightAreaData.freightAreaIn.road);
        if(newPathDistance>=0.0f && (newPathDistance<pathDistance || pathDistance<0.0f))
        {
          pathDistance=newPathDistance;
          bestWarehouse=nearestAsTheCrowFlies;
        }
        yield return null;
      }
      
      if(pathDistance>=0.0f)
  	    rslt.value=bestWarehouse;
  	}
  }
  
  public IEnumerator<Warehouse> AllWarehouses()
  {
    return SafeEnumeratorFor(_warehouses);
  }
  
  public void AddToWarehouses(Warehouse toAdd)
  {
    _warehouses.Add(toAdd);  
  }
  
  public void RemoveFromWarehouses(Warehouse toRemove)
  {
    _warehouses.Remove(toRemove);  
  }
  
  public IEnumerator<OrderManager> AllOrderManagers()
  {
    return SafeEnumeratorFor(_orderManagers);
  }
  
  public void AddToOrderManagers(OrderManager toAdd)
  {
    _orderManagers.Add(toAdd);  
  }
  
  public IEnumerator<KeepAsideOrderManager> AllKeepAsideOrderManagers()
  {
    return SafeEnumeratorFor(_keepAsideOrderManagers);
  }
  
  public void AddToKeepAsideOrderManagers(KeepAsideOrderManager toAdd)
  {
    _keepAsideOrderManagers.Add(toAdd);  
  }
  
  private IEnumerator<T> SafeEnumeratorFor<T> (HashSet<T> set)
  {
    HashSet<T> toIterateOn=new HashSet<T>(set);//On créée une copie du HashSet qui contient les mêmes références.
    //Utiliser un clone comme ci-dessus est nécessaire car en faisant cela, on évite le plantage du à la modification du set en cours d'itération lazy
    
    /*
    TODO ce qui se trouve ci-dessous concernant le fait que ce n'est pas suffisant s'est vérifié, 
    mais l'explication semble incorrecte (lévaluation ne fonctionne pas comme ça... investiguer !)
    
    --
    On vérifie ici qu'il n'a pas été détruit, MAIS CE N'EST PAS SUFFISANT, il peut l'être entre le moment où la condition est évaluée et celui où l'élément est retourné!!!!
    --
    
    
    */
    
    foreach(T element in toIterateOn) 
    {
      if(element!=null) yield return element;//La comparaison à null d'un GameObject vaut true si l'objet a été détruit	
    }
  }
}