public class RetryData
{
  private int _triesCounter=0;
	
  public int triesCounter
  {
    get
    {
      return _triesCounter;	
    }
  }
  
  public RoadData lastKeyPoint;
  public RoadData goal;
  
  public RetryData(RoadData lastKeyPoint,RoadData goal)
  {
    this.lastKeyPoint=lastKeyPoint;
    this.goal=goal;
  }
  
  public void IncrementCounter()
  {
    _triesCounter++;  
  }
}