using System.Collections;
using System.Collections.Generic;
using AI_Snakes.Snake;
using AI_Snakes.Utility;
using UnityEngine;
using UnityEngine.XR.WSA.Persistence;

namespace AI_Snakes.Snake
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] Vector2Int _fieldSize = new Vector2Int(12, 12);

        [SerializeField] private int _maxSize = 3;
        [SerializeField] private int _size = 1;
        [SerializeField] private GameObject _snakePrefab;
        [SerializeField] private GameObject _foodPrefab;
        [SerializeField] private GameObject _currentFood;
        [SerializeField] public GameObject Head;
        [SerializeField] private GameObject _tail;

        [SerializeField] public bool IsTraining = true;
        [SerializeField] private int _currentGeneration = 0;
        [SerializeField] private float _qualityPointScore;
        [SerializeField] [Range(0, 1)] private float _movementPerSeconds;

        [SerializeField] private int _howManyFruitsGotten;

        private GameObject[,] _gameField;
        private bool[,] _blockedField;
        private float _movementCounter;
        private Vector2 _nextPos;
        private Vector2 _blockedPos;
        private Snake _snake;

        Direction dir = Direction.Right;

        private void OnEnable()
        {
            Snake.hit += Collision;
        }

        // Use this for initialization
        void Start() 
        {
            _howManyFruitsGotten = 0;
            
            CreateGameField();
            
            _movementCounter = _movementPerSeconds;
            StartNextGeneration();
            _snake = Snake.GetSnake();
        }

        private void OnDisable()
        {
            Snake.hit = Collision;
        }

        // Update is called once per frame
        void Update()
        {
            GameReset();

            _movementCounter -= Time.deltaTime;
            if(_movementCounter < 0)
            {
                MovementRepeating();
                _movementCounter = _movementPerSeconds;
            }
        }

        private void CreateGameField()
        {
            Camera.main.transform.position = new Vector3(_fieldSize.x / 2, _fieldSize.y / 2, Camera.main.transform.position.z);
            _gameField = new GameObject[_fieldSize.x, _fieldSize.y];

            for (int i = 0; i < _fieldSize.x; i++)
            {
                for (int j = 0; j < _fieldSize.y; j++)
                {
                    if (i == 0 || i == _fieldSize.x - 1)
                    {
                        _blockedField = new bool[_fieldSize.x,_fieldSize.y];
                        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        wall.AddComponent<Rigidbody>().useGravity = false;
                        wall.transform.SetParent(gameObject.transform);
                        wall.GetComponent<Collider>().isTrigger = true;
                        wall.tag = "Wall";

                        wall.transform.position = new Vector3(i, j, 0);
                        wall.transform.GetComponent<Renderer>().material.color = new Color(0, 0, 0);                       

                        _gameField[i, j] = wall;
                        _blockedField[i, j] = true;
                    }
                    else if (j == 0 || j == _fieldSize.y - 1)
                    {
                        _blockedField = new bool[_fieldSize.x,_fieldSize.y];
                        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        wall.AddComponent<Rigidbody>().useGravity = false;
                        wall.transform.SetParent(gameObject.transform);
                        wall.GetComponent<Collider>().isTrigger = true;
                        wall.tag = "Wall";

                        wall.transform.position = new Vector3(i, j, 0);
                        wall.transform.GetComponent<Renderer>().material.color = new Color(0, 0, 0);

                        _gameField[i, j] = wall;
                        _blockedField[i, j] = true;
                    }
                    else
                    {
                        var square = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        square.transform.SetParent(gameObject.transform);
                        square.transform.position = new Vector3(i, j, 1);
                        _gameField[i, j] = square;
                    }
                }
            }
        }

        private void FoodGeneration()
        {          
            int xPos = Random.Range(1, _fieldSize.x - 1);
            int yPos = Random.Range(1, _fieldSize.y - 1);

            _currentFood = Instantiate(_foodPrefab, new Vector2(xPos, yPos), transform.rotation);

//            for (int i = 0; i < _tail.Count; i++)
//            {
//                if (_tail[i].transform.position.x == _currentFood.transform.position.x
//                    && _tail[i].transform.position.y == _currentFood.transform.position.y)
//                {
//                    Destroy(_currentFood);
//                    FoodGeneration();
//                    return;
//                }
//            }

            RewardMatrix = SetRewardMatrixForFood(Food);
            
            print("Food coordinates:" + _currentFood.transform.position.x + "," + _currentFood.transform.position.y);
        }

        public void Movement()
        {
            GameObject snakeHead;
            _nextPos = Head.transform.position;

            switch (dir)
            {
                case Direction.Up:
                    _nextPos = new Vector2(_nextPos.x, _nextPos.y + 1);
                    break;
                case Direction.Down:
                    _nextPos = new Vector2(_nextPos.x, _nextPos.y - 1);
                    break;
                case Direction.Left:
                    _nextPos = new Vector2(_nextPos.x - 1, _nextPos.y);
                    break;
                default:
                    _nextPos = new Vector2(_nextPos.x + 1, _nextPos.y);
                    break;
            }

            snakeHead = Instantiate(_snakePrefab, _nextPos, transform.rotation);
            Head.GetComponent<Snake>().SetNextHead(snakeHead);
            Head = snakeHead;

            _qualityPointScore -= 0.1f;
        }

        public bool IsOppositeDirection(Direction chosenDir) 
        {
            var previousDir = dir;
            if(previousDir == Direction.Up && chosenDir == Direction.Down) 
            {
                return true;
            }
            if(previousDir == Direction.Right && chosenDir == Direction.Left) 
            {
                return true;
            }
            if(previousDir == Direction.Down && chosenDir == Direction.Up) 
            {
                return true;
            }
            if(previousDir == Direction.Left && chosenDir == Direction.Right) 
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        
        private void MovementRepeating() 
        {
            dir = _snake.ChooseDirection();
            Movement();
            if (_size >= _maxSize)
            {
                Tail();
            }
            else
            {
                _size++;
            }
            print(dir);
        }

        public void Tail()
        {
            GameObject snakeTail = _tail;
            _tail = _tail.GetComponent<Snake>().GetNextHead();
            Destroy(snakeTail);
        }

        private void StartNextGeneration()
        {
            _currentGeneration++;
            _maxSize = 3;
            _size = 1;
            _qualityPointScore = 0;

            GameObject snakeHead;

            snakeHead = Instantiate(_snakePrefab, new Vector2(_fieldSize.x / 2, _fieldSize.y / 2), transform.rotation);
            Head = snakeHead;
            _tail = snakeHead;

            FoodGeneration();
        }

        private QMatrix SetRewardMatrixForFood(Vector2Int food) 
        {
            QMatrix matrix = new QMatrix(food);
            
//            matrix.QualityMatrix[food.x, food.y - 1].SetValue(Direction.Up, 1);
//            matrix.QualityMatrix[food.x -1, food.y].SetValue(Direction.Right, 1);
//            matrix.QualityMatrix[food.x, food.y + 1].SetValue(Direction.Down, 1);
//            matrix.QualityMatrix[food.x + 1, food.y].SetValue(Direction.Left, 1);

            return matrix;
        }

        private bool IsBlocked(Vector2 pos) 
        {
            if(pos.x > _blockedField.GetLength(0) - 1 || pos.y > _blockedField.GetLength(1) - 1 || pos.x < 0 || pos.y < 0 ) 
            {
                return true;
            }

            return _blockedField[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];
        }

        public bool IsWayBlocked(Direction dir) 
        {
            _blockedPos = Head.transform.position;
            switch (dir) 
            {
                case Direction.Up:
                    _blockedPos = new Vector2(_blockedPos.x, _blockedPos.y + 1);
                    break;
                case Direction.Down:
                    _blockedPos = new Vector2(_blockedPos.x, _blockedPos.y - 1);
                    break;
                case Direction.Right:
                    _blockedPos = new Vector2(_blockedPos.x + 1, _blockedPos.y);
                    break;
                case Direction.Left:
                    _blockedPos = new Vector2(_blockedPos.x - 1, _blockedPos.y);
                    break;
            }
            if(IsBlocked(_blockedPos)) 
            {
                return true;
            }

            switch(dir) 
            {
                case Direction.Up:
                    return _blockedPos.y + 1 > _fieldSize.y - 1;
                case Direction.Down:
                    return _blockedPos.y - 1 < 0;
                case Direction.Right:
                    return _blockedPos.x + 1 > _fieldSize.x - 1;
                case Direction.Left:
                    return _blockedPos.x - 1 < 0;
                default:
                    return false;
            }
        }

        private void Collision(string collidedObject)
        {
            if (collidedObject == "Food") 
            {
                _howManyFruitsGotten++;
                _maxSize++;
                _qualityPointScore += 1;
                FoodGeneration();
            }
            if (collidedObject == "Snake" || collidedObject == "Wall")
            {
                _qualityPointScore = -1;
            }
        }

        private void WipeClean()
        {
            GameObject[] snakes = GameObject.FindGameObjectsWithTag("Snake");
            for (int i = 0; i < snakes.Length; i++)
            {
                
                Destroy(snakes[i]);
            }
            Destroy(_currentFood);
        }

        private void GameReset()
        {
            if (_qualityPointScore == -1)
            {
                WipeClean();
                StartNextGeneration();
            }
        }

        public static GameController GetController() 
        {
            return FindObjectOfType<GameController>();
        }

        public Vector2Int FieldSize 
        {
            get {return _fieldSize;}
        }
        
        public Vector2Int Food {get; private set;}
        
        public QMatrix RewardMatrix {get; private set;}
        
        public Value Value {get; private set;}
    }
}
