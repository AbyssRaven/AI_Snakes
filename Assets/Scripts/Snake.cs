using System;
using System.Collections.Generic;
using AI_Snakes.Utility;
using Random = UnityEngine.Random;
using UnityEngine;

namespace AI_Snakes.Snake
{
    public class Snake : MonoBehaviour
    {
        private GameObject _nextHead;
        public static Action<String> hit;

        private float _alpha = 0.1f;
        private float _gamme = 0.95f;
        
        private AIBrain _brain;
        private Vector2 _nextPos;
        private GameController _gameController;
        public QMatrix CurrentMatrix {get; private set;}
        public List<bool>[,] _blockedSnakeField;
        public bool[,] _snakeField;
        
        Direction dir = Direction.None;

        private void Start() 
        {
            _gameController = GameController.GetController();
            _brain = _gameController.AIBrain;
        }
        private void Update()
        {
            if (_gameController.Head == this.gameObject)
            {
                CurrentMatrix = _brain.FindQMatrixForFood(_gameController.Food);
            }
        }

        public Direction ChooseDirection() 
        {
            if(_gameController.IsWayBlocked(Direction.Up) && _gameController.IsWayBlocked(Direction.Right)
              && _gameController.IsWayBlocked(Direction.Down) && _gameController.IsWayBlocked(Direction.Left)) 
            {
                _gameController.WipeClean();
            }
            if(_gameController.IsTraining) 
            {
                dir = (Direction)Random.Range(0, 4);
                while(_gameController.IsWayBlocked(dir)) 
                {
                    dir = (Direction)Random.Range(0, 4);
                }
            }

            //_nextPos = _gameController.Head.transform.position;
            //dir = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(_nextPos.x), Mathf.RoundToInt(_nextPos.y)].ChooseDirectionWithHighestValue();
            return dir;
        }

        private void CalculateQValueOfNextAction(Direction dir)
        {

        }

        private double GetBestQValueForAction(Direction dir)
        {
            var snakeHead = _gameController.Head.transform.position;
            Value qValue;

            switch (dir)
            {
                case Direction.Up:
                    qValue = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x),
                        Mathf.RoundToInt(snakeHead.y + 1)];
                    break;
                case Direction.Right:
                    qValue = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x + 1),
                        Mathf.RoundToInt(snakeHead.y)];
                    break;
                case Direction.Down:
                    qValue = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x),
                        Mathf.RoundToInt(snakeHead.y - 1)];
                    break;
                case Direction.Left:
                    qValue = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x - 1),
                        Mathf.RoundToInt(snakeHead.y)];
                    break;
                default:
                    return 0;
            }

            return qValue.GetBestValue();
        }

        public void SetNextHead(GameObject newHead)
        {
            _nextHead = newHead;

        }

        public GameObject GetNextHead()
        {
            return _nextHead;
        }

        public void ClearSnakeFieldToBlock()
        {
            _snakeField = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hit != null)
            {
                hit(other.transform.tag);
            }
            if (other.tag == "Food")
            {
                Destroy(other.gameObject);
            }
        }
        
        public static Snake GetSnake() 
        {
            return FindObjectOfType<Snake>();
        }

        public void CollectCurrentMatrixData()
        {
            _brain.DataCollection(CurrentMatrix);
        }
    }
}
