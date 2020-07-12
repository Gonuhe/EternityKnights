using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlacementData))]
[RequireComponent(typeof(BuildingStock))]
public class Building : MonoBehaviour
{
  //Prefab correspondant au bâtiment tombé en ruines
  public GameObject ruinsPrefab;
  public GameObject smokePrefab;
	
  [HideInInspector]
  public PlacementData placementData;
	
  public int health = 100;
  public int maxHealth = 100;

  public int waterLevel = 100; 

  public float automaticHealthDegradationResistency = 1.0f;
  public float automaticWaterDegradationResistency = 1.0f;

  private int degradationRateInSeconds = 10;
  
  [HideInInspector]
  public BuildingStock buildingStock;
  
  [HideInInspector]
  public FreightAreaData freightAreaData;

  protected void Awake()
  {
    placementData=GetComponent<PlacementData>();
    buildingStock=GetComponent<BuildingStock>();
    freightAreaData=GetComponent<FreightAreaData>();
  }
  
  protected void Start()
  {
    StartCoroutine(DegradeHealth());//Obligé de le faire dans 2 coroutine pour pouvoir jouer sur les rate de dégradation. Obligé de faire cela avec le temps car ce sont des int
    StartCoroutine(DegradeWater());
    if(automaticWaterDegradationResistency == 0 || automaticHealthDegradationResistency == 0)
      Debug.Log("Warning : a building (name : "+ gameObject.name+" ) has automaticDegradationResistency at 0, which means that it won't degrade");
  }
	
  // Update is called once per frame
  protected virtual void Update ()
  {
  }

  private IEnumerator DegradeHealth ()
  {
    while(true)
    {
      health-=1;
      if(health < 0)
        health = 0;
	
	  if(health<=0)//Pas dans un else pour que ça se fasse tout de suite après la réudction de santé si c'est approprié
        StartCoroutine(CollapseBuilding());
	  
      yield return new WaitForSeconds((float)degradationRateInSeconds*(1.0f/automaticHealthDegradationResistency));
    }
  }

  private IEnumerator DegradeWater()//TODO: supprimer cette gestion de l'eau
  {
    while(true)
    {
      waterLevel-=1;
      if(waterLevel < 0)
        waterLevel = 0;
      yield return new WaitForSeconds((float)degradationRateInSeconds*(1.0f/automaticWaterDegradationResistency));
    }
  }

  private IEnumerator CollapseBuilding()
  {
  	GameManager gameManager=GameManager.instance;
     
    gameManager.InstantiatePrefab(smokePrefab, transform.position);

    yield return new WaitForSeconds(0.7f);
    
    GameObject ruins=gameManager.InstantiatePrefab(ruinsPrefab,transform.position);
    
    //TODO: les lignes suivantes ne sont là que parce que pour l'instant, les ruines sont un quad avec texture répétée
    Vector2 newScale=new Vector2(placementData.unitsWidth,placementData.unitsHeight);
    ruins.transform.localScale=newScale;
    Material ruinsMaterial=ruins.GetComponent<MeshRenderer>().material;
    ruinsMaterial.mainTextureScale=newScale;
    PlacementData ruinsPlacementData=ruins.GetComponent<PlacementData>();
    ruinsPlacementData.unitsHeight=placementData.unitsHeight;
    ruinsPlacementData.unitsWidth=placementData.unitsWidth;

    gameManager.DestroyGameObject(gameObject);
  }
  
  public void RestoreHealth()
  {
    health = maxHealth;
  }
}
