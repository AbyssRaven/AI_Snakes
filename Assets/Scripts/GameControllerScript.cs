using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameControllerScript : MonoBehaviour {	
		
	enum Direction 
	{
		North,
		East,
		South,
		West,
	}

	[SerializeField]private int xBound;
	[SerializeField]private int yBound;
	
	[SerializeField] private int _maxSize;
	[SerializeField] private int _size;
	[SerializeField] private GameObject _snakePrefab;		
	[SerializeField] private GameObject _foodPrefab;
	[SerializeField] private GameObject _currentFood;
	
	[SerializeField] private SnakeScript _head;
	[SerializeField] private SnakeScript _tail;
	
	[SerializeField] private GameObject _squarePrefab;
	[SerializeField] private GameObject _field;
	
	private GameObject[,] _matrixCells = new GameObject[16,16];
	private Vector2 _nextPos;
	private bool _isTraining;
	private float _qualityPointScore;
	
//	private int populationSize = 50;
//	private int generationNumber;
//	private int[] layers = new int[] { 1, 10, 10, 1 };
//	private List<NNScript> networks;	
//	private RaycastHit hit;
	
	Direction dir = Direction.East;

	// Use this for initialization
	void Start () 
	{
		InvokeRepeating("CreateMatrix", 0.5f, 0.5f);
		InvokeRepeating("MovementRepeating", 0.5f, 0.5f);
		Food();
	}
	
	// Update is called once per frame
	void Update () 
	{
//		BeginTraining();
		GameReset();
	}
	
	void CreateMatrix() 
	{
		foreach(Transform _field in transform) 
		{
			
		}
		for(int i = 0; i > _matrixCells.GetLength(0); i++) 
		{
			for(int j = 0; j > _matrixCells.GetLength(1); j++) 
			{
				
				_matrixCells[i, j] = Instantiate(_squarePrefab, new Vector2((i * 1) + 0.5f, (j * 1) + 0.5f), Quaternion.identity) as GameObject;

//				matrixCells[x, y] = matrixCell.GetComponent<int>();
			}
		}
	}
	
	void MovementRepeating() 
	{
		Movement();
		if(_size >= _maxSize) 
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
	
	void Timer()
	{
		_isTraining = false;
	}
	
	private void Movement() {
		GameObject temp;
		_nextPos = _head.transform.position;
		
		switch(dir) 
		{
			case Direction.North:
				_nextPos = new Vector2(_nextPos.x, _nextPos.y + 1);
				break;
			case Direction.South:
				_nextPos = new Vector2(_nextPos.x, _nextPos.y - 1);
				break;
			case Direction.West:
				_nextPos = new Vector2(_nextPos.x - 1, _nextPos.y);
				break;
			default:
				_nextPos = new Vector2(_nextPos.x + 1, _nextPos.y);
				break;
		}
		
		temp = Instantiate(_snakePrefab, _nextPos, transform.rotation);
		_head.SetNextHead(temp.GetComponent<SnakeScript>());
		_head = temp.GetComponent<SnakeScript>();
		_qualityPointScore -= 0.1f;
	}
	
	private void Tail() 
	{
		SnakeScript temp = _tail;
		_tail = _tail.GetNextHead();
		temp.RemoveTail();
	}

	private void RandomDirection() {
		if(dir == Direction.North) 
		{
			var temp = (Direction)Random.Range(0, 4);
			while(temp == Direction.South) 
			{
				temp = (Direction)Random.Range(0, 4);
			}

			dir = temp;
			return;
		}
		
		if(dir == Direction.East) 
		{
			var temp = (Direction)Random.Range(0, 4);
			while(temp == Direction.West) 
			{
				temp = (Direction)Random.Range(0, 4);
			}
			
			dir = temp;
			return;
		}
		
		if(dir == Direction.South) 
		{
			var temp = (Direction)Random.Range(0, 4);
			while(temp == Direction.North) 
			{
				temp = (Direction)Random.Range(0, 4);
			}
			
			dir = temp;
			return;
		}
		
		if(dir == Direction.West) 
		{
			var temp = (Direction)Random.Range(0, 4);
			while(temp == Direction.East) 
			{
				temp = (Direction)Random.Range(0, 4);
			}
			
			dir = temp;
		}
	}
	
	void Food() {
		int xPos = Random.Range(-xBound, xBound);
		int yPos = Random.Range(-yBound, yBound);

		_currentFood = Instantiate(_foodPrefab, new Vector2(xPos, yPos), transform.rotation);
		StartCoroutine(CheckRender(_currentFood));
	}
	
	void Collision(string collidedObject) 
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
	
	
	IEnumerator CheckRender(GameObject foodObject) 
	{
		yield return new WaitForEndOfFrame();
		if(foodObject.GetComponent<Renderer>().isVisible == false) 
		{
			if(foodObject.CompareTag("Food")) 
			{
				Destroy(foodObject);
				Food();
			}
		}
	}

	private void OnEnable() {
		SnakeScript.collision += Collision;
	}
	
	private void OnDisable() {
		SnakeScript.collision -= Collision;
	}

	void GameReset() 
	{
		if(_qualityPointScore == -1) 
		{
			CancelInvoke("MovementRepeating");
			SceneManager.LoadScene("Snake_Level");
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
