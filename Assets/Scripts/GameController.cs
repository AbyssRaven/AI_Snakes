using System.Collections;
using System.Collections.Generic;
using AI_Snakes.SnakeAI;
using AI_Snakes.Utility;
using UnityEngine;

namespace AI_Snakes.Main
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] Vector2Int _fieldSize = new Vector2Int(12, 12);

        [SerializeField] private int _maxSize = 3;
        [SerializeField] private int _size = 1;
        [SerializeField] private GameObject _snakePrefab;
        [SerializeField] private GameObject _foodPrefab;
        [SerializeField] private GameObject _currentFood;
        [SerializeField] private GameObject _head;
        [SerializeField] private GameObject _lastTail;
        [SerializeField] private List<GameObject> _tail = new List<GameObject>();

        [SerializeField] private bool _isTraining = true;
        [SerializeField] private int _currentGeneration = 0;
        [SerializeField] private float _qualityPointScore;
        [SerializeField] [Range(0, 1)] private float _movementPerSeconds;

        private GameObject[,] _gameField;
        private float _movementCounter;
        private Vector2 _nextPos;

        Direction dir = Direction.Right;

        private void OnEnable()
        {
            Snake.hit += Collision;
        }

        // Use this for initialization
        void Start()
        {
            CreateGameField();

            _movementCounter = _movementPerSeconds;
            StartNextGeneration();
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
                        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        wall.AddComponent<Rigidbody>().useGravity = false;
                        wall.transform.SetParent(gameObject.transform);
                        wall.GetComponent<Collider>().isTrigger = true;
                        wall.tag = "Wall";

                        wall.transform.position = new Vector3(i, j, 0);
                        wall.transform.GetComponent<Renderer>().material.color = new Color(0, 0, 0);

                        _gameField[i, j] = wall;

                    }
                    else if (j == 0 || j == _fieldSize.y - 1)
                    {
                        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        wall.AddComponent<Rigidbody>().useGravity = false;
                        wall.transform.SetParent(gameObject.transform);
                        wall.GetComponent<Collider>().isTrigger = true;
                        wall.tag = "Wall";

                        wall.transform.position = new Vector3(i, j, 0);
                        wall.transform.GetComponent<Renderer>().material.color = new Color(0, 0, 0);

                        _gameField[i, j] = wall;
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

            for (int i = 0; i < _tail.Count; i++)
            {
                if (_tail[i].transform.position.x == _currentFood.transform.position.x
                    && _tail[i].transform.position.y == _currentFood.transform.position.y)
                {
                    Destroy(_currentFood);
                    FoodGeneration();
                    return;
                }
            }

            RewardMatrix = SetRewardMatrixForFood(Food);
            
            print("Food coordinates:" + _currentFood.transform.position.x + "," + _currentFood.transform.position.y);
        }

        public void Movement()
        {
            GameObject snakeHead;
            _nextPos = _head.transform.position;

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
            _tail.Add(_head);
            _head.GetComponent<Snake>().SetNextHead(snakeHead);
            _head = snakeHead;

            _qualityPointScore -= 0.1f;
        }

        private void MovementRepeating()
        {
            Movement();
            if (_size >= _maxSize)
            {
                Tail();
            }
            else
            {
                _size++;
            }
            //ChooseDirection();
            print(dir);
        }

        public void Tail()
        {
            GameObject snakeTail = _lastTail;
            _lastTail = _lastTail.GetComponent<Snake>().GetNextHead();
            Destroy(_tail[0]);
            _tail.RemoveAt(0);
        }

        private void StartNextGeneration()
        {
            _currentGeneration++;
            _tail.Clear();
            _maxSize = 3;
            _size = 1;
            _qualityPointScore = 0;

            GameObject snakeHead;

            snakeHead = Instantiate(_snakePrefab, new Vector2(_fieldSize.x / 2, _fieldSize.y / 2), transform.rotation);
            _head = snakeHead;
            _lastTail = snakeHead;

            FoodGeneration();
        }

        private QMatrix SetRewardMatrixForFood(Vector2Int food) 
        {
            QMatrix matrix = new QMatrix(food);
            
            matrix.QualityMatrix[food.x, food.y - 1].SetValue(Direction.Up, 1);
            matrix.QualityMatrix[food.x -1, food.y].SetValue(Direction.Right, 1);
            matrix.QualityMatrix[food.x, food.y + 1].SetValue(Direction.Down, 1);
            matrix.QualityMatrix[food.x + 1, food.y].SetValue(Direction.Left, 1);

            return matrix;
        }

        private void Collision(string collidedObject)
        {
            if (collidedObject == "Food")
            {
                FoodGeneration();
                _maxSize++;
                _qualityPointScore += 1;
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
    }
}
