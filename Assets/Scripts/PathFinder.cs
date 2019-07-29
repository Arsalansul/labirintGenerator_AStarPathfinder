using UnityEngine;

namespace Assets.Scripts
{
    // https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
    public class PathFinder
    {
        private readonly CellManager cellManager;
        public PathFinder(CellManager cellManager)
        {
            this.cellManager = cellManager;
        }
        
        private void FindPath(int currentCellIndex, int endCellIndex)
        {
            cellManager.AddToOpenList(currentCellIndex);

            while (cellManager.OpenListCount() > 0)
            {
                currentCellIndex = cellManager.GetCellIndexWithLeastF(); // let the currentNode equal the node with the least f value

                cellManager.RemoveFromOpenList(currentCellIndex);
                cellManager.AddToCloseList(currentCellIndex);

                if (currentCellIndex == endCellIndex) //found the end
                {
                    break;
                }

                for (var i = 0; i < 4; i++)
                {
                    //find child
                    var maskWall = CellManager.maskWallTop << i;
                    if ((cellManager.cells[currentCellIndex] & maskWall) != 0)
                        continue;

                    var childIndex = currentCellIndex + cellManager.GetNeighbourRelativePosition(maskWall << 4);
                    
                    // Child is on the closedList
                    if (cellManager.CloseListContain(childIndex))
                        continue;
                    
                    // Create the f, g, and h values
                    //child.g = currentNode.g + distance between child and current
                    //child.h = distance from child to end
                    //child.f = child.g + child.h;

                    var g = cellManager.GetG(currentCellIndex) + 1;

                    if (cellManager.OpenListContain(childIndex) && g > cellManager.GetG(childIndex))
                        continue;

                    cellManager.RememberG(childIndex, g);

                    var h = cellManager.Distance(childIndex, endCellIndex);
                    cellManager.RememberH(childIndex, (ulong) h);

                    cellManager.RememberCameFromIndexPF(childIndex, currentCellIndex); //запоминаем откуда пришли

                    // Add the child to the openList
                    cellManager.AddToOpenList(childIndex);
                }
            }
        }

        public void FindCellsInDistance(int cellIndex, int distance)  //в бит маску G записывается длина пути, этим и будем пользоваться
        {
            cellManager.AddToOpenList(cellIndex);

            while (cellManager.OpenListCount() > 0)
            {
                cellIndex = cellManager.GetCellIndexFromOpenList();

                cellManager.RemoveFromOpenList(cellIndex);
                cellManager.AddToCloseList(cellIndex);

                for (var i = 0; i < 4; i++)
                {
                    //find child
                    var maskWall = CellManager.maskWallTop << i;
                    if ((cellManager.cells[cellIndex] & maskWall) != 0)
                        continue;

                    var childIndex = cellIndex + cellManager.GetNeighbourRelativePosition(maskWall << 4);

                    // Child is on the closedList
                    if (cellManager.CloseListContain(childIndex))
                        continue;
                    
                    var g = cellManager.GetG(cellIndex) + 1;

                    if (cellManager.OpenListContain(childIndex) && g > cellManager.GetG(childIndex))
                        continue;

                    cellManager.RememberG(childIndex, g);

                    // Add the child to the openList
                    if (g < (ulong) distance)
                        cellManager.AddToOpenList(childIndex);
                }
            }
        }

        public int GiveCellIndexToMove(int startCellIndex, int endCellIndex)
        {
            FindPath(startCellIndex, endCellIndex);

            //выстраиваем путь от конца к началу
            var currentCellIndex = endCellIndex;
            while (currentCellIndex != startCellIndex)
            {
                var nextIndex = cellManager.GetCameFromIndexPF(currentCellIndex);

                cellManager.RememberMoveToIndex(nextIndex, currentCellIndex); //записываем путь

                currentCellIndex = nextIndex;
            }

            var result = cellManager.GetMoveToIndexPF(startCellIndex);

            cellManager.ClearCellsAfterPF();

            return result;
        }

        public int GetRandomCellIndexInDistance(int cellIndex, int distance)
        {
            FindCellsInDistance(cellIndex, distance);

            var randomIndex = Random.Range(0, cellManager.cells.Length);
            while (cellManager.GetG(randomIndex) != 0)
            {
                randomIndex = Random.Range(0, cellManager.cells.Length);
            }

            cellManager.ClearCellsAfterPF();
            return randomIndex;
        }
    }
}
