using System;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
    public class CellManager
    {
        // 1 - 4 bits - walls LBRT
        // 5 - 8 - unvisited neighbours LBRT (for LabirintCreator)
        // 9 - visited  (for LabirintCreator)
        // 10 - 17 - came from cell index (for LabirintCreator)

        // 18 - 25 - came from cell index (for PathFinder)
        // 26 - in open list (A Star терминология) (for PathFinder)
        // 27 - in close list (A Star терминология) (for PathFinder)
        // 28 - 35 - g value (A Star терминология) (for PathFinder)
        // 36 - 43 - h value (A Star терминология) (for PathFinder)
        // 44 - 51 - move to cell index (for PathFinder)

        // 52 - coin in this cell
        // 53 - can't spawn coin in this cell

        // 54 - 57 - exit LBRT (for LabirintCreator)

        public const ulong maskWallTop = 1;
        public const ulong maskWallRight = 1UL << 1;
        public const ulong maskWallBottom = 1UL << 2;
        public const ulong maskWallLeft = 1UL << 3;
                     
        public const ulong maskAllWalls = maskWallTop | maskWallRight | maskWallBottom | maskWallLeft;
                     
        public const ulong maskNeighbourTop = 1UL << 4;
        public const ulong maskNeighbourRight = 1UL << 5;
        public const ulong maskNeighbourBottom = 1UL << 6;
        public const ulong maskNeighbourLeft = 1UL << 7;

        public const ulong maskAllNeighbours = maskNeighbourTop | maskNeighbourRight | maskNeighbourBottom | maskNeighbourLeft;

        public const ulong maskVisited = 1UL << 8;
                    
        public const ulong maskCameFromLC = ((1UL << 8) - 1) << 9; //for labirint creator
        public const int CameFromFirstBitLC = 9; //for labirint creator

        public const ulong maskCameFromPF = ((1UL << 8) - 1) << 17; //for path finder
        public const int CameFromFirstBitPF = 17; //for path finder
        public const ulong maskOpenListPF = 1UL << 25; //for path finder
        public const int OpenListFirstBitPF = 25; //for path finder
        public const ulong maskCloseListPF = 1UL << 26; //for path finder
        public const ulong maskGPF = ((1UL << 8) - 1) << 27; //for path finder
        public const int GFirstBitPF = 27; //for path finder
        public const ulong maskHPF = ((1UL << 8) - 1) << 35; //for path finder
        public const int HFirstBitPF = 35; //for path finder
        public const ulong maskMoveTo = ((1UL << 8) - 1) << 43; //for path finder
        public const int MoveToIndexFirstBitPF = 43; //for path finder

        public const ulong maskAllPF = ((1UL << 34) - 1) << 17; //for path finder

        public const ulong maskCoin = 1UL << 51; //coin in this cell
        public const ulong maskCoinNegativePoint = 1UL << 52; //can't spawn coin in this cell

        public const ulong maskExitTop = 1UL << 53;
        public const ulong maskExitRight = 1UL << 54;
        public const ulong maskExitBottom = 1UL << 55;
        public const ulong maskExitLeft = 1UL << 56;

        public const ulong maskExitAll = maskExitTop | maskExitRight | maskExitBottom | maskExitLeft;

        public ulong[] cells;

        private readonly int labirintSize;

        public CellManager(int labirintSize)
        {
            this.labirintSize = labirintSize;
            cells = new ulong[labirintSize * labirintSize];
            DefaultCell();
        }

        private void DefaultCell()
        {
            for (var i = 0; i < cells.Length; i++)
            {
                cells[i] |= maskAllWalls; //set walls
            }

            SetUnvisitedNeighbours();
        }

        public void SetUnvisitedNeighbours()
        {
            for (var i = 0; i < cells.Length; i++)
            {
                if (i < cells.Length - labirintSize)
                {
                    cells[i] |= maskNeighbourTop;
                }

                if ((i + 1) % labirintSize != 0)
                {
                    cells[i] |= maskNeighbourRight;
                }

                if (i >= labirintSize)
                {
                    cells[i] |= maskNeighbourBottom;
                }

                if (i % labirintSize != 0)
                {
                    cells[i] |= maskNeighbourLeft;
                }
            }
        }

        public void Visited(int cellIndex)
        {
            cells[cellIndex] |= maskVisited;
            if (cellIndex < cells.Length - labirintSize)
                cells[cellIndex + labirintSize] &= ~maskNeighbourBottom; //говорим верхнему соседу, что соседа снизу уже посетили

            if((cellIndex +1)%labirintSize !=0)
                cells[cellIndex + 1] &= ~maskNeighbourLeft; //говорим соседу справа, что соседа слева уже посетили

            if(cellIndex >= labirintSize)
                cells[cellIndex - labirintSize] &= ~maskNeighbourTop; //говорим соседу снизу, что соседа сверху уже посетили

            if(cellIndex % labirintSize != 0)
                cells[cellIndex - 1] &= ~maskNeighbourRight; //говорим соседу слева, что соседа справа уже посетили
        }

        public int GetNeighbourRelativePosition(ulong mask)
        {
            switch (mask)
            {
                case maskNeighbourTop:
                    return labirintSize;
                case maskNeighbourRight:
                    return 1;
                case maskNeighbourBottom:
                    return -labirintSize;
                case maskNeighbourLeft:
                    return -1;
                default:
                    return 0;
            }
        }

        public void RemoveWall(int cellIndex, int neighbourIndex)
        {
            if (neighbourIndex < 0 || neighbourIndex >= labirintSize * labirintSize) return;
            var indexDif = cellIndex - neighbourIndex;
            if (indexDif == -labirintSize)
            {
                cells[cellIndex] &= ~maskWallTop; //remove top wall
                cells[neighbourIndex] &= ~maskWallBottom; //remove bottom wall
            }
            else if (indexDif == -1)
            {
                cells[cellIndex] &= ~maskWallRight; //remove right wall
                cells[neighbourIndex] &= ~maskWallLeft; //remove left wall
            }
            else if (indexDif == labirintSize)
            {
                cells[cellIndex] &= ~maskWallBottom; //remove bottom wall
                cells[neighbourIndex] &= ~maskWallTop; //remove top wall
            }
            else if (indexDif == 1)
            {
                cells[cellIndex] &= ~maskWallLeft; //remove left wall
                cells[neighbourIndex] &= ~maskWallRight; //remove right wall
            }
        }

        public int GetRandomUnvisitedNeghbourRelativePosition(ulong cell)
        {
            ulong neighbourPositionMask = 0; 
            if ((cell & maskAllNeighbours) != 0)
            {
                while (neighbourPositionMask == 0)
                {
                    var rd = Random.Range(0, 4);
                    //берем маску с самым правым битом из масок соседей и смещаем на случайное число
                    neighbourPositionMask = cell & (maskNeighbourTop << rd);
                }
                return GetNeighbourRelativePosition(neighbourPositionMask);
            }
            return 0;
        }

        public ulong GetCellByPosition(Vector3 position)
        {
            var x = (int) position.x;
            var y = (int) position.z;

            return cells[x + y * labirintSize];
        }

        public int GetCellIndexByPosition(Vector3 position)
        {
            var x = (int)position.x;
            var y = (int)position.z;

            return x + y * labirintSize;
        }

        public Vector3 GetPositionByCellIndex(int cellIndex)
        {
            var x = cellIndex % labirintSize + 0.5f;
            var y = 0;
            var z = (cellIndex / labirintSize) + 0.5f;

            return new Vector3(x, y, z);
        }

        public int CellWallsCount(ulong cell)
        {
            var result = 0;
            for (var i = 0; i < 4; i++)
            {
                result += ((int)cell & (1 << i)) >> i;
            }

            return result;
        }

        public int Distance(int cellIndex_1, int cellIndex_2) //минимальное расстояние без диагоналей
        {
            var result = 0;
            result += Mathf.Abs(cellIndex_1 % labirintSize - cellIndex_2 % labirintSize);
            result += Mathf.Abs(cellIndex_1 / labirintSize - cellIndex_2 / labirintSize);
            return result;
        }

        public int OpenListCount()
        {
            var result = 0;
            for (var i = 0; i < cells.Length; i++)
            {
                result += (int)((cells[i] & maskOpenListPF) >> OpenListFirstBitPF);
            }
            return result;
        }

        public int GetCellIndexWithLeastF()
        {
            var result = -1;
            uint f_Old = 300;
            for (var i = 0; i < cells.Length; i++)
            {
                if ((cells[i] & maskOpenListPF) == 0) continue;
                //f = g + h
                var f = (uint)(((cells[i] & maskGPF) >> GFirstBitPF) + ((cells[i] & maskHPF) >> HFirstBitPF));

                if (f >= f_Old) continue;
                result = i;
                f_Old = f;
            }
            if (result == -1)
                Debug.LogError("f_Old " + f_Old);
            return result;
        }

        public int GetCellIndexFromOpenList()
        {
            var result = -1;
            for (var i = 0; i < cells.Length; i++)
            {
                if ((cells[i] & maskOpenListPF) != 0)
                    result = i;
            }
            if (result == -1)
                Debug.LogError("open list is empty");
            return result;
        }

        public ulong GetG(int cellIndex)
        {
            return ((cells[cellIndex] & maskGPF) >> GFirstBitPF);
        }

        public ulong GetH(int cellIndex)
        {
            return ((cells[cellIndex] & maskHPF) >> HFirstBitPF);
        }

        public void RememberCameFromIndexPF(int cellIndex, int cameFromIndex)
        {
            cells[cellIndex] &= ~maskCameFromPF;
            cells[cellIndex] |= (ulong)cameFromIndex << CameFromFirstBitPF;
        }

        public void RememberCameFromIndexLC(int cellIndex, int cameFromIndex)
        {
            cells[cellIndex] &= ~maskCameFromLC;
            cells[cellIndex] |= (ulong)cameFromIndex << CameFromFirstBitLC;
        }

        public void RememberG(int cellIndex, ulong g)
        {
            cells[cellIndex] &= ~maskGPF;
            cells[cellIndex] |= g << GFirstBitPF;
        }

        public void RememberH(int cellIndex, ulong h)
        {
            cells[cellIndex] &= ~maskHPF;
            cells[cellIndex] |= h << HFirstBitPF;
        }

        public void RememberMoveToIndex(int cellIndex, int moveToIndex)
        {
            cells[cellIndex] &= ~maskMoveTo;
            cells[cellIndex] |= (ulong)moveToIndex << MoveToIndexFirstBitPF;
        }

        public void ClearCellsAfterPF()
        {
            for (var i = 0; i < cells.Length; i++)
            {
                cells[i] &= ~maskAllPF;
            }
        }

        public int GetCameFromIndexPF(int cellIndex)
        {
            return (int)((cells[cellIndex] & maskCameFromPF) >> CameFromFirstBitPF);
        }

        public int GetMoveToIndexPF(int cellIndex)
        {
            return (int)((cells[cellIndex] & maskMoveTo) >> MoveToIndexFirstBitPF);
        }

        public bool CheckWall(int cellIndex, Vector3 dir)
        {
            ulong mask = maskWallTop;
            if (dir.z > 0)
            {
                mask = maskWallTop;
            }
            else if (dir.x > 0)
            {
                mask = maskWallRight;
            }
            else if (dir.z < 0)
            {
                mask = maskWallBottom;
            }
            else if (dir.x < 0)
            {
                mask = maskWallLeft;
            }
            return (cells[cellIndex] & mask) != 0;
        }

        public void SetCoinInCell(int cellIndex)
        {
            cells[cellIndex] |= maskCoin;
        }

        public void CoinNegativePoint(int cellIndex)
        {
            cells[cellIndex] |= maskCoinNegativePoint;
        }

        public int CountCoinPositions()
        {
            var result = 0;
            for (int i = 0; i < cells.Length; i++)
            {
                if ((cells[i] & maskCoinNegativePoint) == 0)
                {
                    result++;
                }
            }
            return result;
        }

        public void AddToOpenList(int cellIndex)
        {
            cells[cellIndex] |= maskOpenListPF;
        }

        public void RemoveFromOpenList(int cellIndex)
        {
            cells[cellIndex] &= ~maskOpenListPF;
        }

        public void AddToCloseList(int cellIndex)
        {
            cells[cellIndex] |= maskCloseListPF;
        }

        public bool CloseListContain(int cellIndex)
        {
            return (cells[cellIndex] & maskCloseListPF) != 0;
        }

        public bool OpenListContain(int cellIndex)
        {
            return (cells[cellIndex] & maskOpenListPF) != 0;
        }

        public void SetExit(int side, int number)
        {
            var cellIndex = 0;
            var mask = maskExitTop << side;

            switch (side)
            {
                case 0:
                    cellIndex = number + labirintSize * (labirintSize - 1);
                    break;
                case 1:
                    cellIndex = labirintSize * (labirintSize - number) - 1;
                    break;
                case 2:
                    cellIndex = number;
                    break;
                case 3:
                    cellIndex = labirintSize * (labirintSize - 1 - number);
                    break;
            }
            cells[cellIndex] &= ~maskExitAll;
            cells[cellIndex] |= mask;
        }

        public bool CheckExit(int cellIndex, Vector3 dir)
        {
            ulong mask = maskExitTop;
            if (dir.z > 0)
            {
                mask = maskExitTop;
            }
            else if (dir.x > 0)
            {
                mask = maskExitRight;
            }
            else if (dir.z < 0)
            {
                mask = maskExitBottom;
            }
            else if (dir.x < 0)
            {
                mask = maskExitLeft;
            }
            return (cells[cellIndex] & mask) != 0;
        }
    }
}
