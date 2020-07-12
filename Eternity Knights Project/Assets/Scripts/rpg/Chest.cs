using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartLocalization;

public class Chest : MonoBehaviour
{
  public int rewardId;

  public int quantity = 1;

  private bool _opening = false;
  private bool _opened = false;

  private Animator _animator;

  private DefaultDialogLauncher _ddl;

  private Text _controlHint;


  void Start()
  {
    _animator = GetComponent<Animator>();
    _ddl = GetComponent<DefaultDialogLauncher>();
    _controlHint = GameObject.Find("ControlHint").GetComponent<Text>(); 
  }


  public void OpenChest()
  {
    if(!_opened)
    {
      _opened = true;
      StartCoroutine(OpenChestRoutine());
    }
    else if (_opening)
      _ddl.Talk();
  }

  public IEnumerator OpenChestRoutine()
  {
    _animator.SetBool("OpenChest",true);
    GameManager.instance.RPGData.inventory.AddItem(rewardId, quantity);
    yield return new WaitForSeconds(1.5f);
    _opening = true;
    _ddl.Talk();
    yield return new WaitUntil(() => _ddl.talking == false);
    DeactivateChest();
  }

  public void DeactivateChest()
  {
    _opening = false;
    GetComponent<DefaultDialogLauncher>().enabled = false;
    GetComponent<ChestArgumentProvider>().enabled = false;
    GetComponent<Animator>().enabled = false;
    GetComponent<Selectable>().enabled = false;
    BoxCollider2D[] bcs = GetComponents<BoxCollider2D>();
    foreach(BoxCollider2D bc in bcs)
      if(bc.isTrigger)
        bc.enabled = false;
    GetComponent<Chest>().enabled = false;
  }
}
