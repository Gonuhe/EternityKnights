using UnityEngine;
using System.Collections;


/**
* Attribut à ajouter à un bâtiment pour en faire un entrepôt, à savoir un bâtiment
* capable de stocker et renvoyer des ressources.
**/
[RequireComponent(typeof(OrderManager))]
[RequireComponent(typeof(CarriersToStockFreightAreaInBehaviour))]
[RequireComponent(typeof(WorkPlace))]
[RequireComponent(typeof(Building))]
public class Warehouse : MonoBehaviour
{
  [HideInInspector]
  public OrderManager orderManager;
  
  protected void Start()
  {
    GameManager.instance.cityBuilderData.stockManager.AddToWarehouses(this); //TODO: quand on détruit l'entrepôt, l'en supprimer!!
    orderManager=GetComponent<OrderManager>();
    
    StartCoroutine(OrdersManagementCoroutine());
  }
  
  void OnDestroy()
  {
    GameManager.instance.cityBuilderData.stockManager.RemoveFromWarehouses(this);  
  }
  
  /**
  * Coroutine gérant l'expédition des commandes passées à l'entrepôt.
  **/
  private IEnumerator OrdersManagementCoroutine()
  {
    WorkPlace workPlace=GetComponent<WorkPlace>();
  	  
    while(true)
    {
      float workersRatio=workPlace.WorkersRatio();
    	
      if(!Utils.FloatComparison(workersRatio,0.0f,0.001f))
      {
        yield return new WaitForSeconds(0.5f/workersRatio);
      
        orderManager.SendOrderIfPossible();
      }
      else yield return new WaitForSeconds(1.0f);//On attend 1 seconde pour donner le temps aux travailleurs d'arriver et de former un ratio plus raisonnable
    } 	
  }
}