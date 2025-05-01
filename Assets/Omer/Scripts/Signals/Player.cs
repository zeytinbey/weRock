using UnityEngine;
using Zenject;

namespace DI
{
    public class Player: MonoBehaviour
    {
        [Inject] private ScoreService scoreService;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("GainScore called");
                scoreService.GainScore(amount:1);
            }
            


        }


    }
}