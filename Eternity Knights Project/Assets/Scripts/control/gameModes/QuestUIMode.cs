using UnityEngine;
using System.Collections;

public class QuestUIMode : GameMode
{
  private GameObject _player;

  private QuestUI _UIpanel;

  public QuestUI uIPanel
  {
    set
    {
      _UIpanel = value;//Sert au panel à s'enregistrer
    }
  }
  
  public QuestUIMode()
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
    _UIpanel.LoadScrollView();
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
