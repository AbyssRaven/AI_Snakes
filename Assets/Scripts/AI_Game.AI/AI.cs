using System;
using System.Collections.Generic;
using AI_Game.Utility;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Game.AI
{
    public class AI : MonoBehaviour
    {
        private GameObject _nextHead;
        public static Action<String> hit;

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
            CurrentMatrix = _brain.FindQMatrixForFood(_gameController.Goal);
            RewardMatrix = InitRewardMatrix(_gameController.Goal);
        }

        //Selects a new direction according to the bool _isTraining
        public Direction ChooseDirection() 
        {
            var aiHead = _gameController.Head.transform.position;

            //If all directions are block, do a game reset. Safty measure
            if (_gameController.IsWayBlocked(Direction.Up) && _gameController.IsWayBlocked(Direction.Right)
              && _gameController.IsWayBlocked(Direction.Down) && _gameController.IsWayBlocked(Direction.Left)) 
            {
                _gameController.WipeClean();
            }
            //random direction 
            if(_gameController.IsTraining) 
            {
                dir = (Direction)Random.Range(0, 4);
                while(_gameController.IsWayBlocked(dir)) 
                {
                    dir = (Direction)Random.Range(0, 4);
                }
            }
            //if _isTraining is false, select a direction with the highest QValue
            else
            {
                double up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Up);
                double right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Right);
                double down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Down);
                double left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Left);

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

        //Starts the Q algorithmus in here. Which means big math. Sets the new Q value of certain direction of a matrix field
        public void CalculateQValueOfNextAction(Direction dir)
        {
            var aiHead = _gameController.Head.transform.position;

            //Q algorithmus. q is the value that will be put into the "Action" part of the formula. Q(Status, q) is how it should be. 
            //This means that the formula is calculating the value of what the new q should be, so it can set it later into the Q matrix.           
            double q = RewardMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(dir) + _discountRateGamma * GetQValueForEachAction(dir);

            //If the Q value is set, dont set another value. 
            if(CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(dir) <= q) 
            {
                if(q < 1.1)
                {
                    //The q that we calculated before is now set as the Q value of the direction we are going to
                    CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].SetDirectionValue(dir, q);
                }
            }
            GetSurroundingQValues();
        }

        //Checks up the Q value of the field the player is on. Sends the value to GameController to color the field with a color
        private void GetSurroundingQValues()
        {
            var aiHead = _gameController.Head.transform.position;

            double up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Up);
            double right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Right);
            double down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Down);
            double left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Left);

            var value = MoreMath.Max(up, right, down, left);
            _gameController.ColorGameField(value);
        }

        //Calculation of Q(newStatus,action). Goes through all possible direction value, of the future field
        private double GetQValueForEachAction(Direction dir)
        {
            var aiHead = _gameController.Head.transform.position;
            double up = 0;
            double right = 0;
            double down = 0;
            double left = 0;
            
            switch(dir) 
            {
                case Direction.Up:
                    up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y + 1)].GetDirectionValue(Direction.Up);
                    right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y + 1)].GetDirectionValue(Direction.Right);
                    down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y + 1)].GetDirectionValue(Direction.Down);
                    left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y + 1)].GetDirectionValue(Direction.Left);
                    break;
                case Direction.Right:
                    up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x + 1), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Up);
                    right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x + 1), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Right);
                    down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x + 1), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Down);
                    left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x + 1), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Left);
                    break;
                case Direction.Down:
                    up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y - 1)].GetDirectionValue(Direction.Up);
                    right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y - 1)].GetDirectionValue(Direction.Right);
                    down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y - 1)].GetDirectionValue(Direction.Down);
                    left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y - 1)].GetDirectionValue(Direction.Left);
                    break;
                case Direction.Left:
                    up = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x - 1), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Up);
                    right = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x - 1), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Right);
                    down = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x - 1), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Down);
                    left = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x - 1), Mathf.RoundToInt(aiHead.y)].GetDirectionValue(Direction.Left);
                    break;
            
            }

            //print("All action values of the new status: " + "Up:" + up + " | Right:" + right + " | Down:" + down + " | Left:" + left);
            List<double> value = new List<double> { up, right, down, left };
            //print("The max of QMax: " + value.Max());
            
            return value.Max();
        }

        //Sets the reward matrix, if a reward is found
        public void SetRewardForAction(Direction dir)
        {
            var aiHead = _gameController.Head.transform.position;
            var goal = _gameController.CurrentGoal.transform.position;

            switch (dir)
            {
                case Direction.Up:
                    if (aiHead.x == goal.x && aiHead.y + 1 == goal.y)
                    {
                        RewardMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].SetDirectionValue(dir, 1);
                    }
                    break;
                case Direction.Right:
                    if (aiHead.x + 1 == goal.x && aiHead.y == goal.y)
                    {
                        RewardMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].SetDirectionValue(dir, 1);
                    }
                    break;

                case Direction.Down:
                    if (aiHead.x == goal.x && aiHead.y - 1 == goal.y)
                    {
                        RewardMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].SetDirectionValue(dir, 1);
                    }
                    break;
                case Direction.Left:
                    if (aiHead.x - 1 == goal.x && aiHead.y == goal.y)
                    {
                        RewardMatrix.QualityMatrix[Mathf.RoundToInt(aiHead.x), Mathf.RoundToInt(aiHead.y)].SetDirectionValue(dir, 1);
                    }
                    break;
            }
        }

        private QMatrix InitRewardMatrix(Vector2Int goal) {
            QMatrix reward = new QMatrix(goal);

            return reward;
        }

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
        
        public static AI GetAI() 
        {
            return FindObjectOfType<AI>();
        }

        public void CollectCurrentMatrixData()
        {
            _brain.DataCollection(CurrentMatrix);
        }
    }
}
