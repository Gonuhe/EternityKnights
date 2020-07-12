using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver : MonoBehaviour
{

  // Use this for initialization
  void Start ()
  {
    Over();
  }
  
  // Update is called once per frame
  void Update ()
  {
  
  }

  private IEnumerator GoToTitleScreen()
  {
    yield return new WaitForSeconds(5);
    SceneManager.LoadScene(0, LoadSceneMode.Single);
  }

  private void Over()
  {
    //Perform some actions

    StartCoroutine(GoToTitleScreen());
  }
}
