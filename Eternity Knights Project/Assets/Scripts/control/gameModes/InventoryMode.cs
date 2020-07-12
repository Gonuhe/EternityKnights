using UnityEngine;
using System.Collections;

public class InventoryMode : GameMode
{
  private GameObject _player;

  private InventoryUI _UIpanel;

  public InventoryUI uIPanel
  {
    set
    {
      _UIpanel = value;//Sert au panel à s'enregistrer
    }
  }
  
  public InventoryMode()
  {
    _player = GameObject.Find("Player");
  }
  
  public override void ApplyPreconditions()
  {
  }
  
  public override void UpdateMode()
  {
  }

  /**
   * Ne peut être appelé avant start
   **/
  public override void StartMode()
  {
    if(_player != null)
      _player.GetComponent<PlayerMoveManager>().DisableMove();
    //TODO cacher le panneau latéral du citybuildercanvas SI on est en mode citybuilder

    _UIpanel.gameObject.SetActive(true);
    _UIpanel.Refresh();
  }
  
  public override void StopMode()
  {
    _UIpanel.gameObject.SetActive(false);

    if(_player != null)
      _player.GetComponent<PlayerMoveManager>().EnableMove();
  }
  
  protected override IEnumerator TransitionToMode(GameMode newMode)
  {
    return null;
  }
}
