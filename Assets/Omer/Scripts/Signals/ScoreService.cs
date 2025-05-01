
using System;
using DI.Signals;
using Zenject;

public class ScoreService 
{
    private readonly SignalBus _signalBus;
    
    public event Action<int> OnScoreChanged;


    public ScoreService(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }
    public int Score
    {
        get => score;
        private set
        {
            if(score == value)
                return;

            score = value;
           _signalBus.TryFire<ScoreChangedSignal>(new ScoreChangedSignal(score));
            OnScoreChanged?.Invoke(score);
        }
    }

    private int score;

    public void GainScore(int amount)
    {
        Score += amount;
    }

}
