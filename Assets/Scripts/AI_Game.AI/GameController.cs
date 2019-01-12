using AI_Game.Utility;
using AI_Game.Menu;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI_Game.AI
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] public Vector2Int _fieldSize = new Vector2Int(12, 12);

        [SerializeField] private int _maxSize = 3;
        [SerializeField] private int _size = 1;
        [SerializeField] private GameObject _aiPrefab;
        [SerializeField] private GameObject _goalPrefab;
        [SerializeField] public GameObject CurrentGoal;
        [SerializeField] public GameObject Head;
        [SerializeField] private GameObject _tail;

        [SerializeField] public bool IsTraining = true;
        [SerializeField] private int _currentGeneration = 0;
        [SerializeField] [Range(0, 1)] private float _movementPerSeconds;
        [SerializeField] private int _howManyGoalsFound;

        private GameObject[,] _gameField;
        private bool[,] _blockedField;
        private float _movementCounter;
        private Vector2 _nextPos;
        private Vector2 _blockedPos;
        private AI _ai;
        private MainMenu _mainMenu;
        private bool _generationActive;
        public int SizeX;
        public int SizeY;

        [SerializeField] private bool _wipeNowPlease = false;

        Direction dir = Direction.None;
        Direction previousDir;

        private void OnEnable()
        {
            AI.hit += Collision;
        }

        // Use this for initialization
        void Start() 
        {
            _howManyGoalsFound = 0;
            print(SizeX);

            CreateGameField();
            AIBrain = new AIBrain();
            
            _movementCounter = _movementPerSeconds;
            FoodGeneration();
            StartNextGeneration();
            _ai = AI.GetAI();
        }

        private void OnDisable()
        {
            AI.hit = Collision;
        }

        // Update is called once per frame
        void Update() 
        {
            WipeNowPlease();
            //After a set time, do a movement
            _movementCounter -= Time.deltaTime;
            if(_generationActive) 
            {
                if(_movementCounter < 0) 
                {
                    MovementRepeating();
                    _movementCounter = _movementPerSeconds;
                }
            }
            
        }

        //Sets up the playing field of the simulation. Puts down field blocks and also wall blocks at the ends of the field
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

        public void ColorGameField(double value)
        {
            var aiHead = Head.transform.position;
            foreach (GameObject field in _gameField)
            {
                if(field.transform.position.x == aiHead.x && field.transform.position.y == aiHead.y)
                {
                    print("ColorGameField has been activated and value is: " + value);
                    if(value >= 0.90)
                    {
                        field.transform.GetComponent<Renderer>().material.color = new Color(1, 0.2f, 0.1f);
                    }
                    if (value < 0.90 && value >= 0.75)
                    {
                        field.transform.GetComponent<Renderer>().material.color = new Color(1, 0.4f, 0.2f);
                    }
                    if (value < 0.75 && value >= 0.50)
                    {
                        field.transform.GetComponent<Renderer>().material.color = new Color(1, 0.6f, 0.4f);
                    }
                    if (value < 0.50 && value >= 0.25)
                    {
                        field.transform.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
                    }
                    if (value < 0.25 && value > 0)
                    {
                        field.transform.GetComponent<Renderer>().material.color = new Color(0, 1, 1);
                    }
                }
            }
        }

        //Generates food 
        private void FoodGeneration()
        {
            var xPos = Random.Range(1, _fieldSize.x - 1);
            var yPos = Random.Range(1, _fieldSize.y - 1);

            CurrentGoal = Instantiate(_goalPrefab, new Vector2(yPos, xPos), transform.rotation);

            var goalLocation = CurrentGoal.transform.position;
            Goal = new Vector2Int(Mathf.RoundToInt(goalLocation.x), Mathf.RoundToInt(goalLocation.y));      
            
            print("Goal coordinates:" + CurrentGoal.transform.position.x + "," + CurrentGoal.transform.position.y);
        }

        //Perform a movemnt according to a the direction enum
        public void Movement()
        {
            GameObject goalHead;
            _nextPos = Head.transform.position;

            switch (dir)
            {
                case Direction.Up:
                    _nextPos = new Vector2(_nextPos.x, _nextPos.y + 1);
                    break;
                case Direction.Right:
                    _nextPos = new Vector2(_nextPos.x + 1, _nextPos.y);
                    break;    
                case Direction.Down:
                    _nextPos = new Vector2(_nextPos.x, _nextPos.y - 1);
                    break;
                case Direction.Left:
                    _nextPos = new Vector2(_nextPos.x - 1, _nextPos.y);
                    break;
            }
            Head.transform.position = _nextPos;
        }
        
        //Repeats the movement action, and all its functions
        private void MovementRepeating() 
        {
            dir = _ai.ChooseDirection();
            previousDir = dir;
            _ai.CalculateQValueOfNextAction(dir);
            _ai.SetRewardForAction(dir);
            Movement();
        }

        //Starts a new generation
        private void StartNextGeneration() 
        {
            _currentGeneration++;
            _maxSize = 1;
            _size = 1;
            
            var xPos = Random.Range(1, _fieldSize.x - 1);
            var yPos = Random.Range(1, _fieldSize.y - 1);

            GameObject snakeHead;
            AIBrain.SaveQMatrix();
            snakeHead = Instantiate(_aiPrefab, new Vector2(xPos, yPos), transform.rotation);
            Head = snakeHead;
            _tail = snakeHead;

            _generationActive = true;
        }

        //Looks if something is blocked
        private bool IsBlocked(Vector2 pos) 
        {
            if(pos.x > _blockedField.GetLength(0) - 1 || pos.y > _blockedField.GetLength(1) - 1 || pos.x < 0 || pos.y < 0 ) 
            {
                return true;
            }
            return _blockedField[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];
        }

        //Asks if the direction chosen, has a blocked field
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

            switch (dir) 
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

        //Collison
        private void Collision(string collidedObject)
        {
            if (collidedObject == "Goal") 
            {
                _howManyGoalsFound++;
                GameReset();
            }
            if (collidedObject == "AI" || collidedObject == "Wall")
            {
                GameReset();
            }
        }

        //Activate game reset now becasue of bool _wipeNowPlease
        private void WipeNowPlease()
        {
            if(_wipeNowPlease)
            {
                _wipeNowPlease = false;
                GameReset();
            }
        }

        //Removes all food and snake pieces on the playing field
        public void WipeClean()
        {
            _generationActive = false;
            GameObject[] ai = GameObject.FindGameObjectsWithTag("AI");
            _ai.CollectCurrentMatrixData();
            for (int i = 0; i < ai.Length; i++)
            {
                Destroy(ai[i]);
            }
        }

        //Game reset
        private void GameReset()
        {
            WipeClean();
            StartNextGeneration();
        }

        public static GameController GetController() 
        {
            return FindObjectOfType<GameController>();
        }

        public Vector2Int FieldSize 
        {
            get {return _fieldSize;}
        }
        
        public Vector2Int Goal {get; private set;}
        
        public AIBrain AIBrain { get; private set; }
    }
}
