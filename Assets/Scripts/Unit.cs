using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        public int moveController; //0 - none, 1- player, 2 - AI
        public CellManager cellManager;
        public Transform target;

        public float speed;
        private bool detectTarget;

        private PathFinder pathFinder;
        private Vector3 nextPosition;
        private Vector3 startPosition;

        public Settings settings;

        private bool changeAxis;
        public Vector2 Pos
        {
            get
            {
                var pos = transform.position;

                return new Vector2(pos.x, pos.z);
            }
            set => transform.position = new Vector3(value.x + 0.5f, 0, value.y + 0.5f);
        }

        void Start()
        {
            pathFinder = new PathFinder(cellManager);
            nextPosition = transform.position;
            startPosition = transform.position;
        }

        void Update()
        {
            if (moveController > 0)
            {
                GenerateNextPosition();
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime * speed);
            }
        }

        private void GenerateNextPosition()
        {
            if ((transform.position - nextPosition).magnitude > 0.05f) return;

            if (moveController == 1 && InputKeyPressed())
            {
                var currentCellIndex = cellManager.GetCellIndexByPosition(transform.position);
                var inputVector = GetInputVector().normalized;
                if (!cellManager.CheckWall(currentCellIndex, inputVector))
                {
                    var nextCellIndex = cellManager.GetCellIndexByPosition(transform.position + inputVector);
                    nextPosition = cellManager.GetPositionByCellIndex(nextCellIndex);
                }
                else if (settings.coinCount==0 && cellManager.CheckExit(currentCellIndex, inputVector))
                {
                    settings.Win = true;
                }
            }
            else if (moveController == 2 && transform.position != target.position)
            {
                Radar();
                var currentCell = cellManager.GetCellIndexByPosition(transform.position);
                int cellIndexToMove;
                if (detectTarget)
                {
                    cellIndexToMove = pathFinder.GiveCellIndexToMove(currentCell, cellManager.GetCellIndexByPosition(target.position));
                }
                else
                {
                    var randomCellInDistance = pathFinder.GetRandomCellIndexInDistance(cellManager.GetCellIndexByPosition(startPosition), settings.enemyPatrolDistance);
                    
                    cellIndexToMove = pathFinder.GiveCellIndexToMove(currentCell, randomCellInDistance);
                }
                nextPosition = cellManager.GetPositionByCellIndex(cellIndexToMove);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "enemy" && gameObject.tag == "Player")
            {
                settings.GameOver = true;
            }

            if (other.tag == "Player" && gameObject.tag == "coin")
            {
                settings.coinCount--;
                Destroy(gameObject);
            }
        }

        private Vector3 GetInputVector()
        {
            var inputVector = new Vector3(0, 0, 0);

            if (changeAxis)
            {
                if (Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal") > 0.05f)
                {
                    inputVector.x = Input.GetAxis("Horizontal");
                    inputVector.z = 0;
                }
                if (Input.GetAxis("Vertical") * Input.GetAxis("Vertical") > 0.05f)
                {
                    inputVector.x = 0;
                    inputVector.z = Input.GetAxis("Vertical");
                }

                changeAxis = false;
            }
            else
            {
                if (Input.GetAxis("Vertical") * Input.GetAxis("Vertical") > 0.05f)
                {
                    inputVector.x = 0;
                    inputVector.z = Input.GetAxis("Vertical");
                }
                if (Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal") > 0.05f)
                {
                    inputVector.x = Input.GetAxis("Horizontal");
                    inputVector.z = 0;
                }

                changeAxis = true;
            }

            return inputVector;
        }

        private bool InputKeyPressed()
        {
            return Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal") +
                   Input.GetAxis("Vertical") * Input.GetAxis("Vertical") > 0.05f;
        }

        private void Radar()
        {
            var distance = (target.position - transform.position).magnitude;
            if (distance < settings.enemyDetectTargetDistance)
                detectTarget = true;

            if (distance > settings.enemyLostTargetDistance)
            {
                detectTarget = false;
                startPosition = transform.position;
            }
        }
    }
}
