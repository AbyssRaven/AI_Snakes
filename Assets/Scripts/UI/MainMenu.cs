using UnityEngine;
using UnityEngine.SceneManagement;

namespace AI_Game.Menu
{
    public class MainMenu : MonoBehaviour
    {
        // Use this for initialization
        public void StartGame()
        {
            SceneManager.LoadScene("Game_Scene");
        }
    }
}
