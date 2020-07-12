using UnityEngine;
using System.Collections;

/**
 * SCRIPT DE TEST pour tester l'EvolutionData sur les maisons, mais qui pourra servir de base pour l'EvolutionData final.
 **/
public class AgoraLvl1EvolutionData : EvolutionData 
{

  private Agora _agora;

  public AgoraLvl1EvolutionData(Agora agora)
  {
    _agora = agora;
    building = agora.GetComponent<Building>();
  }

  public override void Evolve()
  {
    base.Evolve();
    AgoraMerchant[] merchantsNew = new AgoraMerchant[_agora.spots*2];
    for(int i = 0; i < _agora.spots; i++)
      merchantsNew[i] = _agora.merchants[i];
    _agora.merchants = merchantsNew;
    _agora.spots*=2;
  }

  /**
   * Quand une agora déévolue, si elle possédait N magasins alors que la forme déévoluée ne peut en accepter que M (M<N), les N-M derniers magasins sont jetés.
   * (TODO : les enregistrer quand meme pour qu'en cas de déévolution-réévolution l'agora reprenne automatiquement les memes caractéristiques). 
   **/
  public override void Devolve()
  {
    base.Devolve();
    AgoraMerchant[] merchantsNew = new AgoraMerchant[_agora.spots/2];
    int j;
    int i;
    for(i = 0, j = 0; i < _agora.spots && j < _agora.spots/2; i++)
      if(_agora.merchants[i] != null)
        merchantsNew[j++] = _agora.merchants[i];
    _agora.merchants = merchantsNew;
    _agora.spots/=2;
    //TODO virer si nécessaire
  }
  
  public override bool MustEvolve()
  {
  	//Cette méthode manquait et faisait tout planter! (Stack overflow) Je la définit comme ça car je n'ai pas besoin d'agoras qui évoluent pour le test
    return false;
  }
  
  public override bool MustDevolve()
  {
    return false;
  }
}
