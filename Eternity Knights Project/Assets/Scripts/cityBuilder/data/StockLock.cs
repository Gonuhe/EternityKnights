using System;
using System.Collections.Generic;
using UnityEngine;

/**
* Attribut contrôlant les limitations de stock d'un bâtiment.
**/
[Serializable]
public class StockLock : MonoBehaviour
{
  /*
  Dictionnaire contenant pour chaque ressource la quantité maximale qui peut en 
  être stockée, à condition de respecter en plus la limite imposée par 
  totalStockLimit.
  */
  public Dictionary<string,int> resourcesStockLimit=new Dictionary<string,int>();
  
  //Qantité maximale totale, toutes ressources confondues, qui peut être stockée dans le bâtiment.
  public int totalStockLimit;
  
  //Stock auquel se rapporte ce StockLock. 
  private BuildingStock _stock;  
  
  protected void Awake()
  {
    _stock=GetComponent<BuildingStock>();
    
    foreach(string resourceRef in CityBuilderResources.ALL_RESOURCES)
    {
      resourcesStockLimit[resourceRef]=totalStockLimit;
    }
  }
  
  /**
  * Retourne la quantité maximale de la ressource resoureceRef que peut encore
  * accueillir le stock associé à ce StockLock.
  **/
  public int LimitFor(string resourceRef)
  {
    return Math.Min(totalStockLimit,resourcesStockLimit[resourceRef]);
  }
  
  /**
  * Définit la limite de stock pour une ressource particulière.
  **/
  public void SetLimitFor(string resourceRef,int limit)
  {
    resourcesStockLimit[resourceRef]=limit;  
  }
  
  /**
  * Retourne true ssi le stock associé à ce StockLock peut accueillir le 
  * chargement passé en paramètre sans violer les limitations qu'il impose.
  **/
  public bool AvailableFor(ResourceShipment shipment)
  {
    bool stockNotFull= _stock.totalStock+shipment.amount <= totalStockLimit;
    bool resourceLimitOk= _stock.StockFor(shipment.resourceName) <= LimitFor(shipment.resourceName);
    
    return stockNotFull && resourceLimitOk;
  }
}
