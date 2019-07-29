using UnityEngine;

namespace Assets.Scripts
{
    public class WallsCreator
    {
        private GameObject WallGameObject = Resources.Load<GameObject>("Prefabs/Wall");

        private GameObject walls;

        public void CreateWalls(CellManager cellManager, Settings settings)
        {
            walls = new GameObject("Walls");
            for (int i = 0; i < cellManager.cells.Length; i++)
            {
                CreateWall(cellManager,i,CellManager.maskWallBottom,new Vector3(0,0,-0.5f),0);

                CreateWall(cellManager, i, CellManager.maskWallLeft, new Vector3(-0.5f, 0, 0), 90);

                if (i >= settings.labirintSize * (settings.labirintSize - 1))
                {
                    CreateWall(cellManager, i, CellManager.maskWallTop, new Vector3(0, 0, 0.5f), 0);
                }

                if (i % settings.labirintSize == 14)
                {
                    CreateWall(cellManager, i, CellManager.maskWallRight, new Vector3(0.5f, 0, 0), 90);
                }
            }
        }

        private void CreateWall(CellManager cellManager, int cellIndex, ulong maskWall, Vector3 deltaWallPosition, int rotationY)
        {
            if ((cellManager.cells[cellIndex] & maskWall) != 0)
            {
                var position = cellManager.GetPositionByCellIndex(cellIndex);

                var instance = Object.Instantiate(WallGameObject, position + deltaWallPosition, Quaternion.Euler(0, rotationY, 0), walls.transform);
                if (cellManager.CheckExit(cellIndex, deltaWallPosition))
                {
                    instance.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
        }

        public void DestoryWalls()
        {
            Object.Destroy(walls);
        }
    }
}
