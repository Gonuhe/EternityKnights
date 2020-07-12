using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(EvolutionManager))]
[RequireComponent (typeof(AgoraOrderManager))]
[RequireComponent(typeof(KeepAsideManagementFreightAreaInBehaviour))]
public class Agora : Building
{
  //représente le nombre d'emplacements présents sur cette agora. Attention, ne représente pas le nombre de spot disponible !!
  public int spots = 3;


  public AgoraMerchant[] merchants;

  protected new void Awake() 
  {
    base.Awake();
    merchants = new AgoraMerchant[spots];
  }

  protected new void Start()
  {
    base.Start();
    StartCoroutine(OrdersSendingCoroutine());
  }
  
  new void Update() 
  {
    base.Update();
  }

  private bool TakeSpot(int spot, AgoraMerchant merchant)
  {
    merchants[spot] = merchant;
    return true;
  }

  private bool TakeSpot(AgoraMerchant merchant)
  {
    for(int i = 0; i < spots; i++)
    {
      if(merchants[i] == null)
        return TakeSpot(i, merchant);
    }
    
    return false;
  }

  private bool LeaveSpot(int spot)
  {
    merchants[spot] = null;
    //TODO détruire le marchand
    return true;
  }

  public bool LeaveSpot(AgoraMerchant merchant)
  {
    for(int i = 0; i < spots; i++)
    {
      if(merchants[i] == merchant)
        return LeaveSpot(i);
    }
    
    return false;
  }


  public IEnumerator<AgoraMerchant> MerchantsEnumerator()
  {
  	AgoraMerchant[] merchantsToIterateOn=merchants;
    for(int i=0;i<merchantsToIterateOn.Length;i++)
    {
      AgoraMerchant merchant=merchantsToIterateOn[i];
      
      if(merchant!=null) 
        yield return merchant;
    }
  }
  
  public void AddMerchant(AgoraMerchant merchant)
  {
    TakeSpot(merchant);
    //TODO gérer le retour de TakeSpot ?
  }
  
  private IEnumerator OrdersSendingCoroutine()
  {
    while(true)
    {
      IEnumerator<AgoraMerchant> merchantsEnum=MerchantsEnumerator();
      while(merchantsEnum.MoveNext())
      {
      	AgoraMerchant merchant=merchantsEnum.Current;
        merchant.orderManager.SendOrderIfPossible();
        yield return new WaitForSeconds(1.0f);
      }
      
      yield return new WaitForSeconds(1.0f);
    }
  }
}
