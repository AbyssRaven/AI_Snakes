using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Ai_Snakes.Scripts.GameControllerScript
{
    public class GameControllerScript : MonoBehaviour
    {

        enum Direction
        {
            Up,
            Right,
            Down,
            Left,
        }

        [SerializeField] Vector2Int _fieldSize = new Vector2Int(12, 12);

        [SerializeField] private int _maxSize = 3;
        [SerializeField] private int _size = 1;
        [SerializeField] private GameObject _snakePrefab;
        [SerializeField] private GameObject _foodPrefab;
        [SerializeField] private GameObject _currentFood;

        [SerializeField] private GameObject _head;
        [SerializeField] private GameObject _tail;

        [SerializeField] private int _currentGeneration = 0;

        private GameObject[,] _gameField;
        private Vector2 _nextPos;
        private bool _isTraining;
        private float _qualityPointScore;
        public static Action<String> collision;

        //	private int populationSize = 50;
        //	private int generationNumber;
        //	private int[] layers = new int[] { 1, 10, 10, 1 };
        //	private List<NNScript> networks;	
        //	private RaycastHit hit;

        Direction dir = Direction.Right;

        // Use this for initialization
        void Start()
        {
            _qualityPointScore = 0;
            
            CreateGameField();

            StartNextGeneration();
            InvokeRepeating("MovementRepeating", 0.5f, 0.5f);
        }

        // Update is called once per frame
        void Update()
        {
            //		BeginTraining();
            GameReset();
        }

        private void CreateGameField()
        {
            Camera.main.transform.position = new Vector3(_fieldSize.x / 2, _fieldSize.y / 2, Camera.main.transform.position.z);
            _gameField = new GameObject[_fieldSize.x, _fieldSize.y];

            for(int i = 0; i < _fieldSize.x; i++)
            {
                for(int j = 0; j < _fieldSize.y; j++)
                {
                    var square = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    square.transform.SetParent(gameObject.transform);
                    square.transform.position = new Vector3(i, j, 1);
                    _gameField[i, j] = square;
                }
            }
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
            RandomDirection();
            print(dir);
        }

        private void Timer()
        {
            _isTraining = false;
        }

        private void Movement()
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
            _head.GetComponent<SnakeScript>().SetNextHead(snakeHead);
            _head = snakeHead;
            _qualityPointScore -= 0.1f;
        }

        private void Tail()
        {
            GameObject snakeTail = _tail;
            _tail = _tail.GetComponent<SnakeScript>().GetNextHead();
            Destroy(snakeTail);
        }

        private void RandomDirection()
        {
            if (dir == Direction.Up)
            {
                var temp = (Direction)Random.Range(0, 4);
                while (temp == Direction.Down)
                {
                    temp = (Direction)Random.Range(0, 4);
                }

                dir = temp;
                return;
            }

            if (dir == Direction.Right)
            {
                var temp = (Direction)Random.Range(0, 4);
                while (temp == Direction.Left)
                {
                    temp = (Direction)Random.Range(0, 4);
                }

                dir = temp;
                return;
            }

            if (dir == Direction.Down)
            {
                var temp = (Direction)Random.Range(0, 4);
                while (temp == Direction.Up)
                {
                    temp = (Direction)Random.Range(0, 4);
                }

                dir = temp;
                return;
            }

            if (dir == Direction.Left)
            {
                var temp = (Direction)Random.Range(0, 4);
                while (temp == Direction.Right)
                {
                    temp = (Direction)Random.Range(0, 4);
                }

                dir = temp;
            }
        }

        void Food()
        {
            int xPos = Random.Range(0, _fieldSize.x);
            int yPos = Random.Range(0, _fieldSize.y);

            _currentFood = Instantiate(_foodPrefab, new Vector2(xPos, yPos), transform.rotation);
            StartCoroutine(CheckRender(_currentFood));
        }

        void Collision(string collidedObject)
        {
            if (collidedObject == "Food")
            {
                Food();
                _maxSize++;
                _qualityPointScore += 1;
            }

            if (collidedObject == "Snake" || collidedObject == "Wall")
            {

                _qualityPointScore = -1;
            }
        }

        private void StartNextGeneration()
        {
            _currentGeneration++;
            _qualityPointScore = 0;

            GameObject snakeHead;

            snakeHead = Instantiate(_snakePrefab, new Vector2(_fieldSize.x / 2, _fieldSize.y / 2), transform.rotation);
            _head = snakeHead;
            _tail = snakeHead;

            Food();
        }

        private void WipeClean()
        {
            Destroy(_head);
            Destroy(_tail);
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

        IEnumerator CheckRender(GameObject foodObject)
        {
            yield return new WaitForEndOfFrame();
            if (foodObject.GetComponent<Renderer>().isVisible == false)
            {
                if (foodObject.CompareTag("Food"))
                {
                    Destroy(foodObject);
                    Food();
                }
            }
        }

        //	void SnakeEyes() 
        //	{
        //
        //		if(dir == Direction.North) 
        //		{
        //			Debug.DrawRay(head.transform.position, Vector3.right);
        //			Debug.DrawRay(head.transform.position, Vector3.up);
        //			Debug.DrawRay(head.transform.position, Vector3.left);
        //			
        //			Ray RightRay = new Ray(transform.position, Vector3.right);
        //			Ray ForwardRay = new Ray(transform.position, Vector3.up);
        //			Ray LeftRay = new Ray(transform.position, Vector3.left);
        //
        //			if(Physics.Raycast(RightRay, out hit)) 
        //			{
        //				if(hit.collider.CompareTag("Wall")) 
        //				{
        //					
        //				}
        //			}
        //			return;
        //		}
        //		
        //		if(dir == Direction.East) 
        //		{
        //			Debug.DrawRay(head.transform.position, Vector3.down);
        //			Debug.DrawRay(head.transform.position, Vector3.up);
        //			Debug.DrawRay(head.transform.position, Vector3.right);
        //			
        //			Ray RightRay = new Ray(transform.position, Vector3.down);
        //			Ray ForwardRay = new Ray(transform.position, Vector3.right);
        //			Ray LeftRay = new Ray(transform.position, Vector3.up);
        //			return;
        //		}
        //		
        //		if(dir == Direction.South) 
        //		{
        //			Debug.DrawRay(head.transform.position, Vector3.left);
        //			Debug.DrawRay(head.transform.position, Vector3.right);
        //			Debug.DrawRay(head.transform.position, Vector3.down);
        //			
        //			Ray RightRay = new Ray(transform.position, Vector3.right);
        //			Ray ForwardRay = new Ray(transform.position, Vector3.down);
        //			Ray LeftRay = new Ray(transform.position, Vector3.left);
        //			return;
        //		}
        //		
        //		if(dir == Direction.West) 
        //		{
        //			Debug.DrawRay(head.transform.position, Vector3.up);
        //			Debug.DrawRay(head.transform.position, Vector3.left);
        //			Debug.DrawRay(head.transform.position, Vector3.down);
        //			
        //			Ray RightRay = new Ray(transform.position, Vector3.up);
        //			Ray ForwardRay = new Ray(transform.position, Vector3.left);
        //			Ray LeftRay = new Ray(transform.position, Vector3.down);
        //		}
        //	}
    }
}
