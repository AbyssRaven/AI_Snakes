using System;
using System.Collections.Generic;
using AI_Snakes.Utility;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Snakes.Snake
{
    public class Snake : MonoBehaviour
    {
        private GameObject _nextHead;
        public static Action<String> hit;

        //private float _learningRateAlpha = 0.1f;
        private float _discountRateGamma = 0.9f;
        
        private AIBrain _brain;
        private GameController _gameController;
        public QMatrix CurrentMatrix {get; private set;}
        private Text _matrix;
        
        Direction dir = Direction.None;

        private void Start() 
        {
            _gameController = GameController.GetController();
            _brain = _gameController.AIBrain;
            //            _matrix = GetComponent<Text>();

            if (_gameController.Head == this.gameObject)
            {
                CurrentMatrix = _brain.FindQMatrixForFood(_gameController.Food);
            }

            for (int i = 0; i < CurrentMatrix.QualityMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < CurrentMatrix.QualityMatrix.GetLength(1); j++)
                {
                    Debug.Log(CurrentMatrix.QualityMatrix[i, j] + "\t");
                }
            }
        }
        private void Update()
        {
            if (_gameController.Head == this.gameObject)
            {
                CurrentMatrix = _brain.FindQMatrixForFood(_gameController.Food);
            }
            //            _matrix.text = CurrentMatrix.ToString();
        }

        public Direction ChooseDirection() 
        {
            var snakeHead = _gameController.Head.transform.position;

            if (_gameController.IsWayBlocked(Direction.Up) && _gameController.IsWayBlocked(Direction.Right)
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
            else
            {
                var up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetValue(Direction.Up);
                var right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetValue(Direction.Right);
                var down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetValue(Direction.Down);
                var left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetValue(Direction.Left);

                if (up > down && up > right && up > left)
                {
                    dir = Direction.Up;
                }
                if (right > up && right > down && right > left)
                {
                    dir = Direction.Right;
                }
                if (down > up && down > right && down > left)
                {
                    dir = Direction.Down;
                }
                if (left > up && left > down && left > right)
                {
                    dir = Direction.Left;
                }

                while (_gameController.IsWayBlocked(dir))
                {
                    dir = (Direction)Random.Range(0, 4);
                }
            }

            return dir;
        }

        public void SetBackwardsQValue(Direction dir)
        {
            var snakeHead = _gameController.Head.transform.position;

            switch (dir)
            {
                case Direction.Up:
                    CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y - 1)].SetValue(Direction.Down, -1);
                    break;
                case Direction.Right:
                    CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x - 1), Mathf.RoundToInt(snakeHead.y)].SetValue(Direction.Left, -1);
                    break;
                case Direction.Down:
                    CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y + 1)].SetValue(Direction.Up, -1);
                    break;
                case Direction.Left:
                    CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x + 1), Mathf.RoundToInt(snakeHead.y)].SetValue(Direction.Right, -1);
                    break;
            }
        }

        public void CalculateQValueOfNextAction(Direction dir)
        {
            var snakeHead = _gameController.Head.transform.position;
            Vector2 newStatus = new Vector2();

            //var r = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetValue(dir);

            switch (dir)
            {
                case Direction.Up:
                    newStatus = new Vector2(snakeHead.x, snakeHead.y + 1);
                    break;
                case Direction.Right:
                    newStatus = new Vector2(snakeHead.x + 1, snakeHead.y);
                    break;
                case Direction.Down:
                    newStatus = new Vector2(snakeHead.x, snakeHead.y - 1) ;
                    break;
                case Direction.Left:
                    newStatus = new Vector2(snakeHead.x - 1, snakeHead.y);
                    break;
            }

            var q = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetValue(dir) + _discountRateGamma * GetQValueForEachAction(newStatus);

            CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetValue(dir, q);
        }

        private double GetQValueForEachAction(Vector2 newStatus)
        {
            var snakeHead = _gameController.Head.transform.position;
            double up;
            double right;
            double down;
            double left;

            up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(newStatus.x), Mathf.RoundToInt(newStatus.y + 1)].GetValue(Direction.Up);
            right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(newStatus.x + 1), Mathf.RoundToInt(newStatus.y)].GetValue(Direction.Right);
            down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(newStatus.x), Mathf.RoundToInt(newStatus.x - 1)].GetValue(Direction.Down);
            left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(newStatus.x - 1), Mathf.RoundToInt(newStatus.x)].GetValue(Direction.Left);

            List<double> value = new List<double> { up, right, down, left };
            return value.Max();
        }

        public void SetRewardForAction(Direction dir)
        {
            var snakeHead = _gameController.Head.transform.position;
            var food = _gameController.CurrentFood.transform.position;

            switch (dir)
            {
                case Direction.Up:
                    if (snakeHead.x == food.x && snakeHead.y + 1 == food.y)
                    {
                        CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetValue(dir, 1);
                    }
                    break;
                case Direction.Right:
                    if (snakeHead.x + 1 == food.x && snakeHead.y == food.y)
                    {
                        CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetValue(dir, 1);
                    }
                    break;

                case Direction.Down:
                    if (snakeHead.x == food.x && snakeHead.y - 1 == food.y)
                    {
                        CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetValue(dir, 1);
                    }
                    break;
                case Direction.Left:
                    if (snakeHead.x - 1 == food.x && snakeHead.y == food.y)
                    {
                        CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetValue(dir, 1);
                    }
                    break;
            }
        }

        //private double GetValueOfAllActionWithReward()
        //{
        //    var snakeHead = _gameController.Head.transform.position;

        //    return _gameController.RewardMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetValue(dir) + _discountRateGamma * GetQValueForEachAction(dir);
        //}

        public void SetNextHead(GameObject newHead)
        {
            _nextHead = newHead;

        }

        public GameObject GetNextHead()
        {
            return _nextHead;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hit != null)
            {
                hit(other.transform.tag);
            }
            if (other.tag == "Food")
            {
                //Destroy(other.gameObject);
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
