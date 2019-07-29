using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Game
    {
        private CellManager cellManager;
        private LabirintManager labirintManager;
        private Settings settings;

        public WallsCreator wallsCreator;
        public UnitManager unitManager;
        private Camera mainCamera;
        private MainCamera mainCameraComponent;

        public Game(Settings settings)
        {
            this.settings = settings;
            cellManager = new CellManager(settings.labirintSize);
            labirintManager = new LabirintManager(cellManager);
            wallsCreator = new WallsCreator();
            unitManager = new UnitManager();
            mainCamera = Camera.main;
            mainCameraComponent = mainCamera.GetComponent<MainCamera>();
            mainCameraComponent.settings = settings;

            LoadSceneObjects();
        }

        private void LoadSceneObjects()
        {
            labirintManager.LabirintCreator(settings);
            wallsCreator.CreateWalls(cellManager, settings);
            
            unitManager.InstantiateUnits(settings, cellManager);
            
            mainCameraComponent.target = unitManager.player.transform;
        }
    }
}
