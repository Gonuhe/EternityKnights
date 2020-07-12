using UnityEngine;

[RequireComponent(typeof(PlacementData))]
[RequireComponent(typeof(ResourceProducer))]
[RequireComponent(typeof(KeepAsideOrderManager))]
[RequireComponent(typeof(WellFreightAreaInBehaviour))]
public class Well : MonoBehaviour
{
  public int waterLimit;
	
  protected void Start()
  {
    StockLock stockLock=GetComponent<StockLock>();
    ResourceProducer producer=GetComponent<ResourceProducer>();
    
    /*
    Sur le prefab, la limite du stockLock est définie à 0. Elle est donc 
    définie ainsi pour toutes les ressources. On va simplement ici la définir 
    pour l'eau, qui est la seule ressource pouvant être stockée dans un puits.
    On va également imposer cette limite en limite de production.
    */
    
    producer.productionBufferSize=waterLimit;
    stockLock.totalStockLimit=waterLimit;
    stockLock.SetLimitFor(CityBuilderResources.WATER,waterLimit);
  }
}