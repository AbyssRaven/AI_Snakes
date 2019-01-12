using AI_Game.AI;
using UnityEngine.UI;
using UnityEngine;

namespace AI_Game.Menu
{
    public class GameUI : MonoBehaviour
    {
        private GameController _gameController;

        public Text generationText;

        // Use this for initialization
        void Start()
        {
            _gameController = GameController.GetController();
            generationText.text = "Generation: " + _gameController.CurrentGeneration.ToString();
        }

        void Update()
        {
            generationText.text = "Generation: " + _gameController.CurrentGeneration.ToString();
        }

        public void IsTrainingToggle(bool IsTraining)
        {
            _gameController.IsTraining = IsTraining;
        } 

        public void speedSlider(float MovementPerSeconds)
        {
            _gameController.MovementPerSeconds = MovementPerSeconds;
        }

        public void WipeNowPleaseButton()
        {
            _gameController.WipeNowPlease = true;
        }
    }
}
