/**
* Liste des différents GameMode définis.
**/
public abstract class GameModes
{
  //Modes principaux
  public static readonly CityBuilderMainMode CITY_BUILDER_MAIN=new CityBuilderMainMode();
  public static readonly RPGMainMode RPG_MAIN=new RPGMainMode();
  
  //Sous-modes
  public static readonly CityBuilderPlacingMode CITY_BUILDER_PLACING=new CityBuilderPlacingMode();
  public static readonly CityBuilderDestroyMode CITY_BUILDER_DESTROY=new CityBuilderDestroyMode();
  public static readonly DialogMode DIALOG_MODE=new DialogMode();
  public static readonly QuestUIMode QUEST_UI_MODE=new QuestUIMode();
  public static readonly InventoryMode INVENTORY_MODE=new InventoryMode();

  public static readonly RPGCombatMode RPG_COMBAT_MODE=new RPGCombatMode();
}