using UnityEngine;
using System.Collections;


/**
 * Classe mère servant à représenter une postAction à faire éxécuter une certaine action après qu'une Sentence d'un Dialogue ait été affiché, ou le cas échéant après qu'un choix
 * ait été fait.
 * Pour l'instant cette classe ne contient rien. Les scripts PostAction doivent cependant hériter de cette classe, par facilité et par solidité du modèle. Nous pourrons en effet
 * ajouter à cette classe certaines règles à faire respecter aux postAction.
 * Cette classe pourra aussi etre utilisée pour implémenter certaines fonctions de base qui se retrouveront souvent dans les postActions.
 **/
public class DialogPostAction : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
