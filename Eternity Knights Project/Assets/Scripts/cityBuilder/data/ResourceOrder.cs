/**
* Classe représentant une commande (à savoir, une demande de ressources) 
* effectuée auprès d'un entrepôt.
**/
public class ResourceOrder
{
  //Contenu de la commande
  public ResourceShipment shipment;
  
  //Destination de la commande
  public BuildingStock deliveryPlace;
  
  public ResourceOrder(ResourceShipment shipment,BuildingStock deliveryPlace)
  {
    this.shipment=shipment;
    this.deliveryPlace=deliveryPlace;
  }
}