using System;

/**
* Classe représentant un chargement de marchandises.
* Chaque chargement ne concerne qu'un seul type de ressource, dans une quantité
* variable.
**/
[Serializable]
public class ResourceShipment
{
  //Identifiant de la ressource contenue dans le chargement.
  public string resourceName;
  
  //Quantité de la ressource (nombre d'unités, donc) contenue dans ce chargement
  public int amount;
  
  public ResourceShipment(string resourceName,int amount)
  {
    this.resourceName=resourceName;
    this.amount=amount;
  }
  
  public void AddShipment(ResourceShipment toAdd)
  {
    if(toAdd!=null && toAdd.resourceName==resourceName)
      amount+=toAdd.amount;
  }
}