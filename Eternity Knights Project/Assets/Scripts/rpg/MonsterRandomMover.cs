using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MoveManager))]
[RequireComponent(typeof(Monster))]
public class MonsterRandomMover : MonoBehaviour
{
  private Monster _monster;
  private MoveManager _moveManager;
  
  protected void Awake()
  {
    _moveManager=GetComponent<MoveManager>();
    _monster=GetComponent<Monster>();
  }
  
  protected void Start()
  {
    _moveManager.Move(RandomMove());  
  }

  private IEnumerator<MoveDescriptor> RandomMove()
  {
  	while(true)
  	{
      if(!_monster.playerInDetectionArea)
      {
      	if(Random.Range(1,50)==42)//Bouge dans un cas sur 50 si pas en mouvement
      	{
          Vector2 newPosition=transform.position;
          float xMultiplier=(float)Random.Range(-1,2);
          float yMultiplier=(float)Random.Range(-1,2);
      
          newPosition.x+=_monster.moveStat*xMultiplier;
          newPosition.y+=_monster.moveStat*yMultiplier;
        
          yield return new MoveDescriptor(newPosition,transform.position,1.0f,0.00001f);
        }
        else yield return null;
      }
      else
        yield return new MoveDescriptor(GameManager.instance.player.transform.position,transform.position,1.0f,0.00001f);
    }
  }
}