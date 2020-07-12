using UnityEngine;
using System.Collections;

public class TestCollision : MonoBehaviour 
{
  void OnTriggerEnter2D(Collider2D other)
  {
    Debug.Log("Papa a vu le fifi de lolo");  
  }
}
