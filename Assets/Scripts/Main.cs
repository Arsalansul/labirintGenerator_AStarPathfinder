using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public Button gameButton;
        public Button quitButton;
        public Dropdown levelsDropdown;

        private Settings settings = new Settings();
        private GameObject canvasGameObject;
        
        private GameModeManager gameModeManager;

        void Start()
        {
            levelsDropdown.options.Clear();
            for (var i = 0; i < 50; i++)
            {
                levelsDropdown.options.Add(new Dropdown.OptionData("level " + i));
            }

            levelsDropdown.value = 0;

            canvasGameObject = GameObject.Find("Canvas");

            SceneManager.LoadScene("Game", LoadSceneMode.Additive);

            gameModeManager = new GameModeManager()
            {
                canvasGO = canvasGameObject,
                settings = settings
            };

            gameButton.onClick.AddListener(() => gameModeManager.StartGame(levelsDropdown.value));
            quitButton.onClick.AddListener(Quit);
        }

        void Update()
        {
            if (settings.GameOver)
            {
                gameModeManager.GameOver();
            }

            if (settings.Win)
            {
                gameModeManager.Win();
            }
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}
