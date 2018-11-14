using System;
using AI_Snakes.Snake;
using AI_Snakes.Utility;
using Random = UnityEngine.Random;
using UnityEngine;

namespace AI_Snakes.Snake
{
    public class Snake : MonoBehaviour
    {
        private GameObject _nextHead;
        public static Action<String> hit;
        
        private Value _value;
        private Vector2 _nextPos;
        private GameController _gameController;
        public QMatrix CurrentMatrix {get; private set;}
        
        Direction dir = Direction.Right;

        private void Start() 
        {
            _gameController = GameController.GetController();
            _value = _gameController.Value;

            if(_gameController.Head == this) 
            {
                CurrentMatrix = _value.FindQMatrixForFood(_gameController.Food);
            }
        }
        
        public Direction ChooseDirection() 
        {
            if(_gameController.IsWayBlocked(Direction.Up) && _gameController.IsWayBlocked(Direction.Right)
              && _gameController.IsWayBlocked(Direction.Down) && _gameController.IsWayBlocked(Direction.Left)) 
            {
                return dir;
            }
            if(_gameController.IsTraining) 
            {
                dir = (Direction)Random.Range(0, 3);
                while(_gameController.IsOppositeDirection(dir) && _gameController.IsWayBlocked(dir)) 
                {
                    dir = (Direction)Random.Range(0, 3);
                }
            }
            
            _nextPos = _gameController.Head.transform.position;
            dir = CurrentMatrix.QualityMatrix[Mathf.RoundToInt(_nextPos.x), Mathf.RoundToInt(_nextPos.y)].ChooseDirectionWithHighestValue();
            return dir;
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
                Destroy(other.gameObject);
            }
        }
        
        public static Snake GetSnake() 
        {
            return FindObjectOfType<Snake>();
        }
    }
}
