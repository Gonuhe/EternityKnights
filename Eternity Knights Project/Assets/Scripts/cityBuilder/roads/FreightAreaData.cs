using UnityEngine;
using System.Collections.Generic;

public class FreightAreaData : MonoBehaviour
{
  public HashSet<Collider2D> treatedColliders=new HashSet<Collider2D>();  
  
  //Stock du bâtiment auquel appartient la zone de frêt
  [HideInInspector]
  public BuildingStock parentStock;
  
  /*
  Nombre de transporteurs pouvant partir de la zone de frêt (donc: nombre de 
  transporteurs qui peuvent au maximum être en cours de livraison pour le 
  bâtiment possédant cette zone de frêt)
  */
  public int carriersNber;
  
  /*
  Prefab à utiliser comme prototype pour les transporteurs partant de cette zone
  de frêt.
  */
  public ResourceCarrier carrierPrefab;
  
  //Nombre de transporteurs disponibles (donc, qui ne sont pas en cours de livraison)
  /*private*/public int _availableCarriers;
  
  public int availableCarriers
  {
    get
    {
      return _availableCarriers;
    }
  }
  
  public int carrierCapacity
  {
    get
    {
      return carrierPrefab==null ? 0: carrierPrefab.capacity;	
    }
  }
  
  [HideInInspector]
  public FreightAreaIn freightAreaIn;
  
  [HideInInspector]
  public FreightAreaOut freightAreaOut;
  
  protected void Awake()
  {
  	/*
  	Les conditions sur l'assignation des freightArea permet de les setter à des
  	objets définis dans un autre Awake s'exécutant avant celui-ci (ce qui peut
  	être très utile pour les marchands d'agora, afin d'éviter les problèmes si,
  	comme dans le test, ils sont déjà présents dans le préfab instancié). 
  	*/
  	if(freightAreaIn==null)
      freightAreaIn=GetComponentInChildren<FreightAreaIn>();
  
    if(freightAreaOut==null)
      freightAreaOut=GetComponentInChildren<FreightAreaOut>();
  
    _availableCarriers=carriersNber;
    parentStock=GetComponent<BuildingStock>();
  }
  
  /**
  * Ajoute au stock associé le chargement du transporteur passé en paramètre 
  * (qui se trouve dans la FreightAreaIn ) et
  * détruit ensuite celui-ci.
  **/
  public void AddShipmentToStockAndDestroy(ResourceCarrier carrier)
  {  	
    if(carrier.shipment!=null)
      parentStock.AddToStock(carrier.shipment);
  
    freightAreaIn.road.roadLock.UnlockFor(carrier.GetComponent<MoveManager>().orientation,carrier.gameObject);
    
    GameManager.instance.DestroyGameObject(carrier.gameObject);
    treatedColliders.Remove(carrier.GetComponent<Collider2D>());
    _availableCarriers++;
  }
  
  /**                                          
  * Ajoute au stock associé le chargement du transporteur passé en paramètre et
  * renvoie ensuite celui-ci à son point d'origine (en lui retirant bien entendu
  * son chargement).
  **/
  public void AddShipmentToStockAndSendBack(ResourceCarrier carrier)
  {
  	if(carrier.shipment!=null)
  	{
      parentStock.AddToStock(carrier.shipment);
      carrier.shipment=null;
    }
    
    SendBack(carrier);
  }
  
  /**
  * Renvoie un transporteur à son point d'origine.
  **/
  public void SendBack(ResourceCarrier carrier)
  {
    carrier.destination=carrier.origin;
    carrier.GetComponent<Collider2D>().enabled=false;
    
    freightAreaOut.EnqueueCarrierToSend(carrier);  
  }
  
  /**
  * Renvoie un transporteur en lui assignant un chargement.
  **/
  public void SendBack(ResourceCarrier carrier,ResourceShipment shipment)
  {
    carrier.shipment=shipment;
    
    SendBack(carrier);
  }  
  
  /**
  * Instantie un nouveau transporteur sur la freightAreaOut en désactivant
  * son collider, avec un chargement à null. 
  **/
  public ResourceCarrier InstantiateCarrierWithoutCollider()
  {
    ResourceCarrier newCarrier=((GameObject)Instantiate(carrierPrefab.gameObject,freightAreaOut.transform.position,Quaternion.identity)).GetComponent<ResourceCarrier>();//TODO voir si changer la coordonnée z de l'objet n'est pas nécessaure pour qu'il n'apparaisse pas DERRIERE la rue ( à mon avis, si)
    newCarrier.GetComponent<Collider2D>().enabled=false;//On annule le collider pour l'instantiation uniquement (plus défensif), c'est la FreighAreaIn qui le réactivera
    _availableCarriers--;
    
    return newCarrier;
  }
  
  /**
  * Envoie un nouveau transporteur vers destination, avec pour orientation de 
  * départ orientation et pour chargement shipment.
  **/
  public void SendCarrier(BuildingStock destination,string orientation,ResourceShipment shipment)
  {//TODO: la nouvelle version de ceci, qui enqueue sur la FreightAreaOut, permet de faire moins de vérifications avant l'appel de cette méthode=> il y a sans doute des trucs à simplifier ailleurs dans le code.
    ResourceCarrier newCarrier=InstantiateCarrierWithoutCollider();
    SendCarrier(newCarrier,destination,orientation,shipment);
  }  
  
  /**
  * Envoie un transporteur (généré AVANT l'appel à la méthode) vers destination, 
  * avec pour orientation de départ orientation et pour chargement shipment.
  **/
  public void SendCarrier(ResourceCarrier toSend,BuildingStock destination,string orientation,ResourceShipment shipment)
  {//TODO: la nouvelle version de ceci, qui enqueue sur la FreightAreaOut, permet de faire moins de vérifications avant l'appel de cette méthode=> il y a sans doute des trucs à simplifier ailleurs dans le code.
    toSend.shipment=shipment;
    toSend.origin=parentStock;
    toSend.destination=destination;
    toSend.GetComponent<MoveManager>().orientation=orientation;//TODO orientation: faire en sorte que les appels à cette méthode donnent bien tous la bonne orientation pour sortir de FAout
    
    freightAreaOut.EnqueueCarrierToSend(toSend);
  }  
}