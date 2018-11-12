using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Ai_Snakes.Scripts.Snake;
using Random = UnityEngine.Random;

namespace Ai_Snakes.Scripts.GameController
{
    public class GameControllerScript : MonoBehaviour
    {

        [SerializeField] Vector2Int _fieldSize = new Vector2Int(12, 12);

        [SerializeField] private int _maxSize = 3;
        [SerializeField] private int _size = 1;
        [SerializeField] private GameObject _snakePrefab;
        [SerializeField] private GameObject _foodPrefab;
        [SerializeField] private GameObject _currentFood;

        [SerializeField] private GameObject _head;
        [SerializeField] private GameObject _tail;

        [SerializeField] private bool _isTraining = true;
        [SerializeField] private int _currentGeneration = 0;

        [SerializeField] private float _qualityPointScore;

        private GameObject[,] _gameField;
        //private SnakeScript _snakeScript;

        //	private int populationSize = 50;
        //	private int generationNumber;
        //	private int[] layers = new int[] { 1, 10, 10, 1 };
        //	private List<NNScript> networks;	
        //	private RaycastHit hit;

        //private void OnEnable()
        //{
        //    SnakeScript.hit += Collision;
        //}

        void Start()
        {           
            CreateGameField();

            StartNextGeneration();
            
        }

        //private void OnDisable()
        //{
        //    SnakeScript.hit = Collision;
        //}

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
                    else if(j == 0 || j == _fieldSize.y - 1)
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

        private void Timer()
        {
            _isTraining = false;
        }

        private void Food()
        {
            int xPos = Random.Range(1, _fieldSize.x -1);
            int yPos = Random.Range(1, _fieldSize.y -1);

            _currentFood = Instantiate(_foodPrefab, new Vector2(xPos, yPos), transform.rotation);
            //FoodLocation = new Vector2Int(xPos, yPos);
            StartCoroutine(CheckRender(_currentFood));
        }

        //public void CheckForFood()
        //{
        //    if( Math.Abs(_head.transform.position.x - FoodLocation.x) < 0.1 && Math.Abs(_head.transform.position.y - FoodLocation.y) < 0.1)
        //    {
        //        Food();
        //        _maxSize++;
        //        _qualityPointScore += 1;
        //    }
                
        //}

        private void Collision(string collidedObject)
        {
            if(collidedObject == "Food")
            {
                Food();
                _maxSize++;
                _qualityPointScore += 1;
            }
            if(collidedObject == "Snake" || collidedObject == "Wall")
            {
                _qualityPointScore = -1;
            }
        }

        private void StartNextGeneration()
        {
            _currentGeneration++;
            _maxSize = 3;
            _size = 1;
            _qualityPointScore = 0;

            GameObject snakeHead;

            snakeHead = Instantiate(_snakePrefab, new Vector2(_fieldSize.x / 2, _fieldSize.y / 2), transform.rotation);
            _head = snakeHead;
            _tail = snakeHead;

            Food();
        }

        private void WipeClean()
        {
            GameObject[] snakes = GameObject.FindGameObjectsWithTag("Snake");
            for(int i = 0; i < snakes.Length; i++)
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

        public Vector2Int FoodLocation { get; private set; }

        public bool Training
        {
            get { return _isTraining; }
        }

        public static GameControllerScript GetScript()
        {
            return FindObjectOfType<GameControllerScript>();
        }

        public Vector2Int FieldSize
        {
            get { return _fieldSize; }
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
