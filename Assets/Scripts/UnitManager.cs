using UnityEngine;

namespace Assets.Scripts
{
    public class UnitManager
    {
        private GameObject enemyParent;
        private GameObject coinParent;

        public GameObject player;

        private GameObject Units;

        private PathFinder pathFinder;

        public void InstantiateUnits(Settings settings, CellManager cellManager)
        {
            Units = new GameObject("Units");

            player = Object.Instantiate(Resources.Load("Prefabs/Player"),Units.transform) as GameObject;

            var playerUnit = player.GetComponent<Unit>();
            playerUnit.Pos = settings.playerStartPosition;
            playerUnit.moveController = 1;
            playerUnit.cellManager = cellManager;
            playerUnit.settings = settings;
            playerUnit.speed = settings.playerSpeed;

            enemyParent = new GameObject("enemies");
            enemyParent.transform.SetParent(Units.transform);
            for (var i = 0; i < settings.enemyCount; i++)
            {
                var enemy = Object.Instantiate(Resources.Load("Prefabs/Enemy"), enemyParent.transform) as GameObject;

                var enemyUnit = enemy.GetComponent<Unit>();
                var enemyPos = new Vector2(Random.Range(0, settings.labirintSize), Random.Range(0, settings.labirintSize));
                enemyUnit.settings = settings;
                enemyUnit.Pos = enemyPos;
                enemyUnit.moveController = 2;
                enemyUnit.cellManager = cellManager;
                enemyUnit.target = player.transform;
                enemyUnit.speed = settings.enemySpeed;
            }

            pathFinder = new PathFinder(cellManager);
            coinParent = new GameObject("Coins");
            coinParent.transform.SetParent(Units.transform);

            for (int i = 0; i < settings.coinCount; i++)
            {
                GenerateCoinPosition(4, cellManager);
            }

            for (var i = 0; i < cellManager.cells.Length; i++)
            {
                if ((cellManager.cells[i] & CellManager.maskCoin) == 0)
                    continue;

                var coin = Object.Instantiate(Resources.Load("Prefabs/Coin"), coinParent.transform) as GameObject;

                var coinUnit = coin.GetComponent<Unit>();
                coin.transform.position = cellManager.GetPositionByCellIndex(i);
                coinUnit.moveController = 0;
                coinUnit.cellManager = cellManager;
                coinUnit.settings = settings;
            }
        }

        public void DestroyUnits()
        {
            Object.Destroy(Units);
        }

        private void GenerateCoinPosition(int distance, CellManager cellManager)
        {
            if (cellManager.CountCoinPositions() == 0)
            {
                Debug.LogError("Don't have free coin positions");
                return;
            }

            var cellIndex = Random.Range(0, cellManager.cells.Length);
            while ((cellManager.cells[cellIndex] & CellManager.maskCoinNegativePoint) != 0)
            {
                cellIndex = Random.Range(0, cellManager.cells.Length);
            }
            
            cellManager.SetCoinInCell(cellIndex);
            cellManager.CoinNegativePoint(cellIndex);
            SetCoinNegativePoints(cellIndex, distance, cellManager);
        }
        
        private void SetCoinNegativePoints (int cellIndex, int distance, CellManager cellManager)
        { 
            pathFinder.FindCellsInDistance(cellIndex, distance);

            for (int i = 0; i < cellManager.cells.Length; i++)
            {
                if ((cellManager.cells[i] & CellManager.maskGPF) != 0)
                    cellManager.CoinNegativePoint(i);
            }

            cellManager.ClearCellsAfterPF();
        }
    }
}
