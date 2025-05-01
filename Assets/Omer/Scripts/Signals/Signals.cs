namespace DI.Signals
{
    public class GameStartedSignal
    {
      
    }
    public class GameOverSignal
    {

    }
public class ScoreChangedSignal
    {
        public int Score { get; }
    
    public ScoreChangedSignal(int score)
        {
            Score = score;
        }
    }


}