using UnityEngine;
using UnityEngine.SceneManagement;

namespace AI_Game.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public int SizeX = 10;
        public int SizeY = 10;

        void Start()
        {
        }
        // Use this for initialization
        public void StartGame()
        {
            SceneManager.LoadScene("Game_Scene");
        }

        public void FieldSizeXInput(string newText)
        {
            var value = Mathf.Clamp(int.Parse(newText), 10, 50);
            SizeX = value;
            newText = value.ToString();
        }

        public void FieldSizeYInput(string newText)
        {
            var value = Mathf.Clamp(int.Parse(newText), 10, 50);
            SizeY = value;
            newText = value.ToString();
        }

        public static MainMenu GetMenu()
        {
            return FindObjectOfType<MainMenu>();
        }
    }
}
