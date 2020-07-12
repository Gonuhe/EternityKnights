using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using SmartLocalization;

/**
* Classe coordonnant tout le jeu.
**/
public class GameManager : MonoBehaviour 
{
  public static GameManager instance;//Pour faciliter le référencement
  
  public CityBuilderData cityBuilderData;
  public RPGData RPGData;
  
  /*
  Sommet de la formée par les GameModes actifs (à savoir, le parent de celui-ci
  et tous ses parents, par structure chaînée).
  */
  public GameMode activeGameMode;
  public Camera mainCamera;
  //public static DialogManager dialogManager;

  /**
   * Pour l'instant, c'est tout ce qui est nécessaire de mettre en place pour l'accélérateur de temps. Ce sera à nous de gérer quels sont les éléments précis qui y sont sensibles
   * et d'employer ce facteur pour accélérer leurs actions. Un système générique permettant d'agir directement sur la fonction update n'est pas une bonne idée car alors tout
   * le jeu est accéléré. Peut-etre que nous pourrions faire quelque chose de similaire en mettant tous les objets concernés sur un layer et en accélérant que ce layer.
   **/
  public int accelerator = 1;
  
  /*
  Booléen indiquant si la souris se trouve actuellement dans de l'interface 
  (utile pour désactiver certaines actions lorsque le joueur se trouve dans les
  menus).
  */
  public bool mouseInUI;
  
  /*
  A la suite de ce commentaire : liste des canvas contenant les différentes UI 
  du jeu. L'activation des Canvas est gérée par les modes de jeu.
  */
  public Canvas cityBuilderCanvas;
  public Canvas RPGCanvas;
  public Canvas allModesCanvas;
  
  //Le personnage du joueur dans le mode RPG
  public Player player;
  
  private EventsManager _eventsManager;//Pour permettre la persistence de l'EventManager entre les scènes (TODO: voir si c'est une bonne idée en fait... Pas sûr) 
  
  protected void Awake()
  {
  	instance=this;//TODO: ça fait que si on en créée un deuxième, il remplace l'autre. Voir si c'est une bonne idée  
    DontDestroyOnLoad(this);
    _eventsManager=EventsManager.instance;
    mainCamera=(Camera)FindObjectOfType(typeof(Camera));
    cityBuilderData=new CityBuilderData();
    RPGData=new RPGData();
    DialogManager.instance = new DialogManager();//Doit être initialisé ici car pas un monobehavior
    activeGameMode=GameModes.CITY_BUILDER_MAIN;//TODO c'est pour le test, ce sera à changer
    mainCamera.orthographicSize=CameraSize.CITY_BUILDER_CAMERA_SIZE;
    
    LanguageManager.Instance.ChangeLanguage("fr");//TODO: changer ça quand on intègrera mieux la localisation
  }
  
  /**
  * On initialise ici les différentes données de jeu.
  **/
  protected void Start()
  {
    cityBuilderData.treasury=10000;//TODO TEST, normalement, 0
	
    activeGameMode.StartMode();

  }
  
  void FixedUpdate()
  {  
    activeGameMode.Update();

  } 
  
  /**
  * Instancie un prefab situé dans le dossier Resources ou un de ses sous-dossiers
  * (en donnant le chemin vers celui-ci à partir du dossier Resources).
  **/
  public GameObject InstantiatePrefab(string prefabReference)
  {
    GameObject placerPrototype=Resources.Load(prefabReference) as GameObject;
    return (Instantiate(placerPrototype) as GameObject);  
  }
  
  /**
  * Instancie un prefab situé dans le dossier Resources ou un de ses sous-dossiers
  * (en donnant le chemin vers celui-ci à partir du dossier Resources) en lui 
  * donnant un nouveau nom, permettant par exemple de le retrouver via script.
  **/
  public GameObject InstantiatePrefab(string prefabReference,string newName)
  {
    GameObject placerPrototype=Resources.Load(prefabReference) as GameObject;
    GameObject rslt= (Instantiate(placerPrototype) as GameObject);
    rslt.name=newName;
    
    return rslt;
  }
  
  /**
  * Instantie un prefab en réalisant une copie d'un prototype.
  **/
  public GameObject InstantiatePrefab(GameObject prototype,Vector3 position)
  {
    return Instantiate(prototype,position,Quaternion.identity) as GameObject;	  
  }
  
  /**
  * Instantie un prefab comme enfant d'un objet. 
  **/
  public GameObject InstantiatePrefabAsChild(GameObject parentObject,GameObject prototype,Vector3 position)
  {
  	/*
  	Il est ici nécessaire de jouer sur les SetActive, dans le cas où les fonctions
  	de démarrage (Start et Awake) de l'objet que l'on créée feraient appel à son
  	parent, qui ne serait pas encore défini (puisqu'il n'est lié qu'APRES l'appel
  	à Instantiate), ce qui causerait un bon gros bug (testé et approuvé !)
  	*/
  	prototype.SetActive(false);
    GameObject childObject= Instantiate(prototype,position,Quaternion.identity) as GameObject;
    prototype.SetActive(true);
    
    childObject.transform.parent=parentObject.transform;
    childObject.SetActive(true);
    
    return childObject;
  }
  
  /**
  * Détruit un objet sur la scène.
  **/
  public void DestroyGameObject(GameObject target)
  {
    Destroy(target);  
  }
  
  /**
  * Empile un GameMode au-dessus du GameMode courant (le définissant donc comme un
  * sous-mode de celui-ci) et le démarre.
  **/
  public void StackGameMode(GameMode subMode)
  {
  	subMode.ApplyPreconditions();
    subMode.parentMode=activeGameMode;
    activeGameMode=subMode;
    activeGameMode.StartMode();
  }
  
  /**
  * Dépile tous les modes jusqu'à ce que la pile des modes de jeu soit vide ou que
  * le mode passé en paramètre ait été retiré.
  **/
  public void UnstackGameMode(GameMode toUnstack)
  {
    UnstackGameModesTo(toUnstack);
    
    if(activeGameMode!=null)
    {
      activeGameMode.StopMode();
      activeGameMode=activeGameMode.parentMode;
    }
  }
  
  /**
  * Dépile tous les modes jusqu'à ce que la pile des modes de jeu soit vide ou que
  * le mode passé en paramètre se trouve à son sommet.
  **/
  public void UnstackGameModesTo(GameMode mode)
  {
    while(activeGameMode!=null && activeGameMode!=mode)
    {
      activeGameMode.StopMode();
      activeGameMode=activeGameMode.parentMode;	
    }
  }
  
  /**
  * Retourne true ssi le mode passé en paramètre se trouve dans la pile des
  * modes de jeu.
  **/
  public bool isModeActive(GameMode mode)
  {
  	GameMode examinedMode=activeGameMode;
  	while(examinedMode!=null)
    {
      if(examinedMode==mode) return true;
      examinedMode=examinedMode.parentMode;	
    }  
    
    return false;
  }
  
  public void SetGameMode(GameMode gameMode)
  {
    GameMode examinedMode=activeGameMode;
  	while(examinedMode!=null)
    {
      examinedMode.StopMode();
      examinedMode=examinedMode.parentMode;	
    }  
    
    activeGameMode=gameMode;
    activeGameMode.StartMode();
  }

  public void GameOver()
  {
    SceneManager.LoadScene(1, LoadSceneMode.Single);

  }
  
  /**
  * Indique que la souris vient de pénétrer dans de l'interface.
  **/
  public void MouseEnteredUI()
  {
    mouseInUI=true;
  }
  
  /**
  * Indique que la souris vient sortir de l'interface.
  **/
  public void MouseExitedUI()
  {
    mouseInUI=false;  
  }
  
  public Coroutine TransitionCameraSize(float finalSize)
  {
    return StartCoroutine(CameraSizeCoroutine(mainCamera.orthographicSize,finalSize));
  }
  
  private IEnumerator CameraSizeCoroutine(float startSize,float finalSize)
  {
    for(float i=0.0f;i<=1.0f;i+=Time.deltaTime*2.0f)//Bref, on fait le zoom en 30 secondes, peu importe le framerate
    {
      yield return null;
      mainCamera.orthographicSize=Mathf.Lerp(startSize,finalSize,i);
    }
    
    mainCamera.orthographicSize=finalSize;//ligne placée ici pour gérer comme il faut les erreurs d'arrondis
  }
  
  public Coroutine TransitionCameraMove(Vector2 destination)
  {
  	return StartCoroutine(CameraMoveCoroutine(destination));  
  }
  
  private IEnumerator CameraMoveCoroutine(Vector2 destination)
  {
    float startX=mainCamera.transform.position.x;
  	float startY=mainCamera.transform.position.y;
  	float startZ=mainCamera.transform.position.z;
  	  
    for(float i=0.0f;i<=1.0f;i+=Time.deltaTime*2.0f)//Bref, on fait le mouvement en 30 secondes, peu importe le framerate
    {
      yield return null;
      mainCamera.transform.position=new Vector3(Mathf.Lerp(startX,destination.x,i),Mathf.Lerp(startY,destination.y,i),startZ);
    }
    
    mainCamera.transform.position=new Vector3(destination.x,destination.y,startZ); //Pour éviter les erreurs d'arrondis  
  }
  
  public Coroutine InitiateRPGCameraView()
  {
    return StartCoroutine(InitiateRPGCameraViewCoroutine());  
  }
  
  private IEnumerator InitiateRPGCameraViewCoroutine()
  {  	  
  	if(!player.isInsideBuilding)  
      yield return TransitionCameraMove(player.transform.position);
    else
      yield return TransitionCameraMove(player.container.transform.position);
    
    if(activeGameMode is RPGMainMode)//NECESSAIRE pour éviter les bugs dans de rares cas en transitionnant la position de la caméra
      mainCamera.GetComponent<RPGCameraMotionManager>().enabled=true;
  }
}
