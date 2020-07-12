using UnityEngine;
using System.Collections;

public class DialogMode : GameMode
{
  private GameObject _player;

  public DialogMode()
  {
    _player = GameObject.Find("Player");
  }

  public override void ApplyPreconditions()
  {
  }
  
  public override void UpdateMode()
  {
  }

  public override void StartMode()
  {
    _player.GetComponent<PlayerMoveManager>().DisableMove();
    //TODO cacher le panneau latéral du citybuildercanvas SI on est en mode citybuilder

  }
  
  public override void StopMode()
  {
    _player.GetComponent<PlayerMoveManager>().EnableMove();
  }
  
  protected override IEnumerator TransitionToMode(GameMode newMode)
  {
    return null;
  }
}
