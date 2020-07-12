using UnityEngine;
using System.Collections;

/**
 * Classe qui représente une barre de progression qui sert à afficher l'état de santé d'un batiment quand on met la souris dessus.
 * Classe qui sera beaucoup appelée à changer. Pour l'instant elle affiche la barre dans le gui => pas propre du tout.
 * Il faudrait qu'elle soit dans la scene près du batiment comme un petit curseur, et qui ne bouge pas avec la caméra.
 * Plus tard encore, si on décide de mettre toute une série d'information, cela sera a intégrer dans une sorte de menu,
 * soit dans la scene, soit on gui, à voir.
 **/
public class ProgressBarBuilding : MonoBehaviour {

  public Texture2D progressBarEmpty;
  public Texture2D progressBarFull;

  public Vector2 pos = new Vector2(20, 40);
  public Vector2 size = new Vector2(20, 60);

  float barDisplay = 0;
  bool showBar = false;
  void OnGUI() {
    if(showBar)
    {
      // draw the background:
      GUI.BeginGroup(new Rect (pos.x, pos.y, size.x, size.y));
      GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);
    
      // draw the filled-in part:
      GUI.BeginGroup(new Rect (0, (size.y - (size.y  * barDisplay)), size.x, size.y  * barDisplay));
      GUI.Box(new Rect (0, -size.y + (size.y * barDisplay), size.x, size.y), progressBarFull);
      GUI.EndGroup();
      GUI.EndGroup ();
    }
  }
  void Update() { 
    if(showBar)
    {
      Building b = gameObject.GetComponent<Building>();
      if(b != null)
      {
        barDisplay = (float)b.health/(float)b.maxHealth;
      }
    }
  }
	// Use this for initialization
	void Start () {
	
	}

  void OnMouseEnter()
  {
    showBar = true;
  }

  void OnMouseExit()
  {
    showBar = false;
  }
}
