using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

/**
 * Classe qui représente un dialogue déserialisé.
 **/
[XmlRoot("DialogXML")]
public class DialogXML
{

  public string postAction;

  [XmlArray("Dialog")]
  [XmlArrayItem("Sentence")]
  public List<Sentence> dialog = new List<Sentence>();
}

public class Sentence
{
  /*(Facultatif) Permet de donner un index à un ID à une certaine phrase, utile pour faire des jumps dans les choix multiple. NE DOIVENT PAS NECESSAIREMENT etre utilisé dans l'ordre
   cfr postAction pour comprendre comment choisir un bon id. Ne pas utiliser l'id 0 car c'est la valeur par défaut.*/
  [XmlAttribute("id")]
  public int id;


  /*
   * Texte de la sentence actuelle. Peut contenir des arguments qui seront sous la forme '%i%'. Dans ce cas, un script (et un seul, même si il y a plusieurs arguments) de type ArgumentProvider
   * doit être attaché au GameObject et doit donner les instructions pour obtenir le bon argument via la fonction GetArgument(i).
   */
  public string text;

  //(Facultatif) si on veut afficher un artwork du perso
  [XmlAttribute("author")]
  public string author;

  //(Facultatif) laisser un choix au joueur.
  [XmlArray("Choices")]
  [XmlArrayItem("Choice")]
  public List<Choice> choices = new List<Choice>();

  /*
   * (Facultatif) Voir postAction
   */
  public string preAction;

  /* (Facultatif) permet de donner le nom d'une fonction a éxécuter une fois que la ligne de dialogue a été lue. Permet de faire certaines opérations scriptée (comme se déplacer,
    lancer un effet spécial ou peu importe). Utilise la réflection.
    Peut etre utlisée de deux façons :
    - Soit "nomDuScript:nomDeLaFonction", dans ce cas si un script nomDuScript est attaché à l'objet (doit être un mono) la fonction nomDeLaFonction sera exécutée. S'il n'y a pas de scripts attaché,
    un objet nomDuScript est instancié et nomDeLaFonction exécutée.
    - Soit "nomDeLaFonction" d'un script de type DialogPostAction (ou qui en hérite) qui est attaché sur le gameObject qui a lancé le dialogue. Il faut dans ce cas veiller à ce que DialogManager connaisse
    le gameObject qui a lancé le dialogue, soit par setCaller, soit au moment de lancer le dialogue en utilisant la fonction qui renseigne le caller (c'est ce qui est fait par le 
    DefaultDialogLauncher). Dans ce cas, on appelle la fonction de nom postAction dans le Nieme script (N >= 1) de type postAction attaché sur le gameObject, où N est l'id de cette sentence.
    => La première solution est à privilégier. Si on veut juste éxécuter des choses simples, indépendante de l'objet qui lance le dialogue, on utilisera la première solution sans attacher le script
    La deuxième solution doit être utilisée si on souhaite exécuter une fonction sans connaître le nom exact du script (qui doit hériter de DialogPostAction). Peut aussi être utiliser pour appeler
    une même fonction avec des arguments différents, même si dans ce cas on privilégiera la première solution en changeant les arguments via des postAction.
   */
  public string postAction;

  /* Constante à utiliser en tant que text pour terminer une conversation. Par défaut, le dialogManager lit tout le fichier et considère qu'il constitue un seul dialogue continu, meme lorsqu'il y a des choix
     alternatif à faire. Cela peut etre utile par exemple si un choix alternatif débloque un dialogue supplémentaire mais que la conversation doit reprendre son cours normal ensuite.
     Si la conversation ne doit pas reprendre son cours normal ensuite mais se terminer, on veillera a placer ce choix alternatif à la fin du fichier, à le cloturer par cette constante,
     et surtout à cloturer l'embranchement principal par cette constante.
     Il est encore possible de faire une postAction avec END_OF_CONV (par exemple, faire le personnage s'en aller)*/
  public const string END_OF_CONV = "END_OF_CONV";
}

public class Choice
{
  public string text;

  [XmlAttribute("goToId")]
  public int goToId;
}