using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameModeManager
    {
        public GameObject canvasGO;

        public Settings settings;

        private Game game;

        public void StartGame(int level)
        {
            SetSettingsValues(level);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
            game = new Game(settings);
            
            canvasGO.SetActive(false);
        }

        public void GameOver()
        {
            game.unitManager.DestroyUnits();
            canvasGO.SetActive(true);
            game.wallsCreator.DestoryWalls();
            game = null;
            //gameState = GameState.MainMenu;
            settings.GameOver = false;
        }

        public void Win()
        {
            game.unitManager.DestroyUnits();
            canvasGO.SetActive(true);
            game.wallsCreator.DestoryWalls();
            game = null;
            //gameState = GameState.MainMenu;
            settings.Win = false;
        }

        private void SetSettingsValues(int level)
        {
            settings.labirintSize = 15;
            settings.labirintDifficulty = 2;

            settings.playerStartPosition = new Vector2(settings.labirintSize / 2, settings.labirintSize / 2);

            settings.enemyCount = level % 20 + 3;
            settings.coinCount = level % 20 + 3;

            settings.enemySpeed = 0.9f;
            settings.playerSpeed = 1;

            settings.enemyDetectTargetDistance = 4 + level / 20;
            settings.enemyLostTargetDistance = 6 + level / 20;
            if (level == 40)
            {
                settings.enemyLostTargetDistance++;
            }

            settings.enemyPatrolDistance = 3;
        }
    }
}
