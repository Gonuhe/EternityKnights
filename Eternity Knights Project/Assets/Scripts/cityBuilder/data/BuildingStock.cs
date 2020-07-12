using UnityEngine;
using System.Collections.Generic;

/**
* Classe contenant le sotck d'un bâtiment, ainsi que les opérations possibles sur 
* celui-ci.
**/
[RequireComponent(typeof(StockLock))]
[RequireComponent(typeof(FreightAreaData))]
public class BuildingStock : MonoBehaviour
{
  //clés=noms des ressources, valeurs=quantités
  private Dictionary<string,int> _stock=new Dictionary<string,int>();
  
  //Retourne la quantité totale en stock, toutes ressources confondues.
  private int _totalStock=0;
  
  [HideInInspector]
  public StockLock stockLock;
  
  [HideInInspector]
  public FreightAreaData freightAreaData;
  
  public int totalStock
  {
    get
    {
      return _totalStock;	
    }
  }
  
  protected void Awake()
  {
    stockLock=GetComponent<StockLock>();
    freightAreaData=GetComponent<FreightAreaData>();
  }
  
  /**
  * Ajoute la quantité passée en paramètre au stock de la ressource passée
  * en paramètre.
  *
  * pré: amount doit être positif
  **/
  public void AddToStock(string resourceRef,int amount) 
  {
    int currentStock=0;
    _stock.TryGetValue(resourceRef,out currentStock);
    _stock[resourceRef]=currentStock+amount;
    _totalStock+=amount;
  }
  
  /**
  * Ajoute le chargement passé en paramètre au stock.
  **/
  public void AddToStock(ResourceShipment shipment)
  {
    AddToStock(shipment.resourceName,shipment.amount);  
  }
  
  /**
  * Retourne le stock de la ressource identifiée par resourceRef.
  **/
  public int StockFor(string resourceRef) 
  {
    int currentStock=0;
    
    _stock.TryGetValue(resourceRef,out currentStock);
    
    return currentStock;
  }
  
  /**
  * Retourne la capacité de stockage restante pour une ressource.
  **/
  public int FreeSpaceFor(string resourceRef)
  {
    return stockLock.LimitFor(resourceRef)-StockFor(resourceRef);
  }
  
  /**
  * Retire du stock de la ressource resourceRef la quantité passée en paramètre.
  * 
  * pré: amount doit être positif
  **/
  public void RemoveFromStock(string resourceRef,int amount)
  {
    _stock[resourceRef]-=amount;
    _totalStock-=amount;
    
    if(_stock[resourceRef]==0)
      _stock.Remove(resourceRef);
  }
  
  /**
  * Retire du stock le chargement passé en paramètre.
  **/
  public void RemoveFromStock(ResourceShipment shipment)
  {
    RemoveFromStock(shipment.resourceName,shipment.amount);  
  }
}