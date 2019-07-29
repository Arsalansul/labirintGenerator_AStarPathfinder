using UnityEngine;

namespace Assets.Scripts
{
    public class Settings
    {
        public int labirintSize;  //labirintSize*labirintSize ограничено 8 битами
        public int labirintDifficulty;  //от 1.

        public bool GameOver;
        public bool Win;

        public Vector2 playerStartPosition;
        public int enemyCount;
        public int coinCount;

        public float enemySpeed;
        public float playerSpeed;
        public float enemyDetectTargetDistance;
        public float enemyLostTargetDistance;
        public int enemyPatrolDistance;
    }
}
