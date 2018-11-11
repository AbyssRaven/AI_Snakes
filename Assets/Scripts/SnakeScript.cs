//using System;
//using Ai_Snakes.Scripts.GameController;
//using Ai_Snakes.Scripts.Food;
//using Random = UnityEngine.Random;
//using UnityEngine;

//namespace Ai_Snakes.Scripts.Snake
//{
//    public class SnakeScript : MonoBehaviour
//    {
//        [SerializeField] private GameObject _snakePrefab;

//        private GameControllerScript _gameControllerScript;
//        private GameObject _nextHead;
//        static public Action<String> hit;
//        private Vector2 _nextPos;
//        private bool _isHead = true;
//        private GameObject[] _tailList;

//        Direction dir = Direction.Right;

//        private void Update()
//        {
//            InvokeRepeating("MovementRepeating", 0.5f, 0.5f);
//        }

//        private void MovementRepeating()
//        {
//            Movement();
//            if (_gameControllerScript._size >= _gameControllerScript._maxSize)
//            {
//                Tail();
//            }
//            else
//            {
//                _gameControllerScript._size++;
//            }
//            ChooseDirection();
//            print(dir);
//        }

//        public void Movement()
//        {
//            if (_isHead == true)
//            {
//                GameObject snakeHead;
//                _nextPos = gameObject.transform.position;

//                switch (dir)
//                {
//                    case Direction.Up:
//                        _nextPos = new Vector2(_nextPos.x, _nextPos.y + 1);
//                        break;
//                    case Direction.Down:
//                        _nextPos = new Vector2(_nextPos.x, _nextPos.y - 1);
//                        break;
//                    case Direction.Left:
//                        _nextPos = new Vector2(_nextPos.x - 1, _nextPos.y);
//                        break;
//                    default:
//                        _nextPos = new Vector2(_nextPos.x + 1, _nextPos.y);
//                        break;
//                }

//                snakeHead = Instantiate(_snakePrefab, _nextPos, transform.rotation);
//                //_gameControllerScript._head.GetComponent<SnakeScript>().SetNextHead(snakeHead);
//                //_gameControllerScript._head = snakeHead;
//                _isHead = false;
                
//                _gameControllerScript._qualityPointScore -= 0.1f;
//            }
//        }

//        private void ChooseDirection()
//        {
//            if (_gameControllerScript._isTraining)
//            {
//                if (dir == Direction.Up)
//                {
//                    var temp = (Direction)Random.Range(0, 4);
//                    while (temp == Direction.Down)
//                    {
//                        temp = (Direction)Random.Range(0, 4);
//                    }

//                    dir = temp;
//                    return;
//                }

//                if (dir == Direction.Right)
//                {
//                    var temp = (Direction)Random.Range(0, 4);
//                    while (temp == Direction.Left)
//                    {
//                        temp = (Direction)Random.Range(0, 4);
//                    }

//                    dir = temp;
//                    return;
//                }

//                if (dir == Direction.Down)
//                {
//                    var temp = (Direction)Random.Range(0, 4);
//                    while (temp == Direction.Up)
//                    {
//                        temp = (Direction)Random.Range(0, 4);
//                    }

//                    dir = temp;
//                    return;
//                }

//                if (dir == Direction.Left)
//                {
//                    var temp = (Direction)Random.Range(0, 4);
//                    while (temp == Direction.Right)
//                    {
//                        temp = (Direction)Random.Range(0, 4);
//                    }

//                    dir = temp;
//                }
//            }
//            else
//            {

//            }
//        }

//        public void Tail()
//        {
//            GameObject snakeTail = _gameControllerScript._tail;
//            _gameControllerScript._tail = _gameControllerScript._tail.GetComponent<SnakeScript>().GetNextHead();
//            Destroy(snakeTail);
//        }

//        public void SetNextHead(GameObject newHead)
//        {
//            _nextHead = newHead;

//        }

//        public GameObject GetNextHead()
//        {
//            return _nextHead;
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            if (hit != null)
//            {
//                hit(other.transform.tag);
//            }
//            if (other.tag == "Food")
//            {
//                Destroy(other.gameObject);
//            }
//        }
//    }
//}


