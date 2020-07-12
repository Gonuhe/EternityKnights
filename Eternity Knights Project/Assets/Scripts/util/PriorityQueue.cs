using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<K,T>: IEnumerable<T> where K:IComparable 
{
  private SortedList<K,Stack<T>> _content=new SortedList<K,Stack<T>>();
  
  public void Push(K key,T element)
  {
  	if(_content.ContainsKey(key))
  	{
  	  _content[key].Push(element);
  	}
  	else
  	{
  	  Stack<T> newStack=new Stack<T>();
  	  newStack.Push(element);
  	  _content[key]=newStack;
  	}
  }
  
  public T Pop()
  {
    K lowestKey=LowestKey();
    
    Stack<T> lowestElementStack=_content[lowestKey];
    T rslt=lowestElementStack.Pop();
    
    if(lowestElementStack.Count==0) _content.Remove(lowestKey);
    	
    return rslt;
  }
  
  public K LowestKey()
  {
    IList<K> keys=_content.Keys;
    return keys[0];
  }
  
  public bool IsEmpty()
  {
    return _content.Count==0;  
  }
  
  public IEnumerator<T> GetEnumerator()
  {
    foreach(Stack<T> stack in _content.Values)
    {
      foreach(T stackEntry in stack)
      {
        yield return stackEntry;  
      }
    }
  }
  
  IEnumerator IEnumerable.GetEnumerator()//Pas le choix, on doit l'implémenter... Bizarre...
  {
    return this.GetEnumerator();
  }
}