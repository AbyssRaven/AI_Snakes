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
        private double _discountRateGamma = 0.9;
        
        private AIBrain _brain;
        private GameController _gameController;
        private QMatrix CurrentMatrix {get; set;}
        private QMatrix RewardMatrix {get; set;}
        private Text _matrix;
        
        Direction dir = Direction.None;

        private void Start() 
        {
            _gameController = GameController.GetController();
            _brain = _gameController.AIBrain;
            CurrentMatrix = _brain.FindQMatrixForFood(_gameController.Food);
            RewardMatrix = InitRewardMatrix(_gameController.Food);
            //            _matrix = GetComponent<Text>();

            //for (int i = 0; i < CurrentMatrix.QualityMatrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < CurrentMatrix.QualityMatrix.GetLength(1); j++)
            //    {
            //        Debug.Log(CurrentMatrix.QualityMatrix[i, j] + "\t");
            //    }
            //}
        }
        private void Update()
        {
            //if (_gameController.Head == this.gameObject)
            //{
            //    CurrentMatrix = _brain.FindQMatrixForFood(_gameController.Food);
            //}

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
                    //CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetValue(dir, -1);
                    dir = (Direction)Random.Range(0, 4);
                }
            }
            else
            {
                double up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Up);
                double right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Right);
                double down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Down);
                double left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Left);

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

        //public void SetBackwardsQValue(Direction dir)
        //{
        //    var snakeHead = _gameController.Head.transform.position;

        //    switch (dir)
        //    {
        //        case Direction.Up:
        //            CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y + 1)].SetValue(Direction.Down, -1);
        //            break;
        //        case Direction.Right:
        //            CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x + 1), Mathf.RoundToInt(snakeHead.y)].SetValue(Direction.Left, -1);
        //            break;
        //        case Direction.Down:
        //            CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y - 1)].SetValue(Direction.Up, -1);
        //            break;
        //        case Direction.Left:
        //            CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x - 1), Mathf.RoundToInt(snakeHead.y)].SetValue(Direction.Right, -1);
        //            break;
        //    }
        //}

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

            double q = RewardMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(dir) + _discountRateGamma * GetQValueForEachAction(dir);

            print("R(status,action) is: " + RewardMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(dir));
            print("Gamma is: " + _discountRateGamma);
            print("Q(Status,action) is: " + q);
            print("Old Snake position is: " + snakeHead);
            print("New status position is: " + newStatus);

            if(CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(dir) <= 0) 
            {
                CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetDirectionValue(dir, q);
            }
        }

        private double GetQValueForEachAction(Direction dir)
        {
            var snakeHead = _gameController.Head.transform.position;
//            var up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(newStatus.x), Mathf.RoundToInt(newStatus.y)].GetDirectionValue(Direction.Up);;
//            var right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(newStatus.x), Mathf.RoundToInt(newStatus.y)].GetDirectionValue(Direction.Right);;
//            var down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(newStatus.x), Mathf.RoundToInt(newStatus.x)].GetDirectionValue(Direction.Down);;
//            var left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(newStatus.x), Mathf.RoundToInt(newStatus.x)].GetDirectionValue(Direction.Left);;
            double up = 0;
            double right = 0;
            double down = 0;
            double left = 0;
            
            switch(dir) 
            {
                case Direction.Up:
                    up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y + 1)].GetDirectionValue(Direction.Up);
                    right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y + 1)].GetDirectionValue(Direction.Right);
                    down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y + 1)].GetDirectionValue(Direction.Down);
                    left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y + 1)].GetDirectionValue(Direction.Left);
                    break;
                case Direction.Right:
                    up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x + 1), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Up);
                    right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x + 1), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Right);
                    down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x + 1), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Down);
                    left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x + 1), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Left);
                    break;
                case Direction.Down:
                    up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y - 1)].GetDirectionValue(Direction.Up);
                    right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y - 1)].GetDirectionValue(Direction.Right);
                    down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y - 1)].GetDirectionValue(Direction.Down);
                    left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y - 1)].GetDirectionValue(Direction.Left);
                    break;
                case Direction.Left:
                    up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x - 1), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Up);
                    right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x - 1), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Right);
                    down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x - 1), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Down);
                    left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x - 1), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Left);
                    break;
            
            }
//            var up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Up);
//            var right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Right);
//            var down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Down);
//            var left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].GetDirectionValue(Direction.Left);

            print("All action values of the new status: " + "Up:" + up + " | Right:" + right + " | Down:" + down + " | Left:" + left);
            List<double> value = new List<double> { up, right, down, left };
            print("The max of QMax: " + value.Max());
            
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
                        RewardMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetDirectionValue(dir, 1);
                    }
                    break;
                case Direction.Right:
                    if (snakeHead.x + 1 == food.x && snakeHead.y == food.y)
                    {
                        RewardMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetDirectionValue(dir, 1);
                    }
                    break;

                case Direction.Down:
                    if (snakeHead.x == food.x && snakeHead.y - 1 == food.y)
                    {
                        RewardMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetDirectionValue(dir, 1);
                    }
                    break;
                case Direction.Left:
                    if (snakeHead.x - 1 == food.x && snakeHead.y == food.y)
                    {
                        RewardMatrix.QualityMatrix[Mathf.RoundToInt(snakeHead.x), Mathf.RoundToInt(snakeHead.y)].SetDirectionValue(dir, 1);
                    }
                    break;
            }
        }

        private QMatrix InitRewardMatrix(Vector2Int food) {
            QMatrix reward = new QMatrix(food);

            return reward;
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
