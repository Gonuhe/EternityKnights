using System.Collections;

/**
* Classe-mère de tous les modes de jeux.
* Les objets héritant de cette classe sont utilisés pour centraliser toute la 
* mise en place et le maintient d'état associé à un mode de jeu, qu'il
* s'agisse de l'écran de gestion de la ville, du mode-RPG, du menu principal,
* des écrans de chargement, ...
**/
public abstract class GameMode
{
  public GameMode parentMode;
	
  /**
  * Applique les préconditions nécessaires au démarrage de ce GameMode (en vérifiant
  * le mode ouvert actuellement, ...).
  * Idéalement, cette méthode aurait du être statique, mais... C# ne permet pas de faire 
  * des méthodes à la fois abstraites et statiques... LONG LIVE JAVA ! ><
  **/
  public abstract void ApplyPreconditions();
  
  /**
  * Fonction à implémenter pour définir le comportement de la focntion Update.
  **/
  public abstract void UpdateMode();
  
  /**
  * Démmarre ce GameMode, initialisant ce qui doit l'être après l'avoir défini 
  * comme mode actif. 
  **/
  public abstract void StartMode();
  
  /**
  * Arrête ce mode.
  **/
  public abstract void StopMode();
  
  /**
  * Effectue la transition entre ce mode et un autre. Cette méthode est appelée
  * par SwitchTo.
  **/
  protected abstract IEnumerator TransitionToMode(GameMode newMode);
  
  /**
  * Effectue la transition vers le mode donné en paramètre et arrête ce mode.
  **/
  public IEnumerator SwitchTo(GameMode newMode)//TODO voir si ce truc est bien utile, on ne l'appelle pas pour l'instant
  {
    yield return TransitionToMode(newMode);
    StopMode();
    GameManager.instance.activeGameMode = newMode;
  }
  
  /**
  * Fonctionne comme la fonction Update de Unity, est appelée par l'Update
  * du GameManager quand le mode est actif.
  * Cette fonction appelle d'abord l'Update du mode parent, s'il existe (et donc,
  * récursivement, l'Update de tous les autres parents).
  **/
  public void Update()
  {
  	if(parentMode!=null)
      parentMode.Update();
  
    UpdateMode();
  }
}