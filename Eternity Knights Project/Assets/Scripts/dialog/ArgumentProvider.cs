using UnityEngine;
using System.Collections;


/**
 * Classe mère de tous les ArgumentProvider qui fournissent les bons arguments aux dialogues.
 **/
public class ArgumentProvider : MonoBehaviour
{

  public virtual string GetArgument(int i)
  {
    return "";
  }
}
