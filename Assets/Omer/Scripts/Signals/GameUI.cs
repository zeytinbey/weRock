using UnityEngine;
using Zenject;
using TMPro;
using DI.Signals;

namespace DI
{
    public class GameUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [Inject] private ScoreService scoreService;

        [Inject] private SignalBus _signalBus;

        private void OnEnable()
        {
            scoreService.OnScoreChanged += UpdateScoreText;
             _signalBus.Subscribe<ScoreChangedSignal>(OnScoreChanged); 
        }
        private void OnDisable()
        {
            scoreService.OnScoreChanged -= UpdateScoreText;
            _signalBus.TryUnsubscribe<ScoreChangedSignal>(OnScoreChanged);
        }


        private void OnScoreChanged(ScoreChangedSignal args)
         {
             UpdateScoreText(args.Score);
         }
        private void UpdateScoreText(int score)
        {
            scoreText.text =$"Score: {score}";
        }


    }
}