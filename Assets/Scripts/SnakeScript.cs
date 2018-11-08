using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnakeScript : MonoBehaviour {

	private SnakeScript _nextHead;
	public static Action<String> collision;

	public void SetNextHead(SnakeScript newHead) {
		_nextHead = newHead;
	}
	
	public SnakeScript GetNextHead() {
		return _nextHead;
	}

	public void RemoveTail() 
	{
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other) {
		if(collision != null) {
			collision(other.transform.tag);
		}

		if(other.CompareTag("Food")) 
		{
			Destroy(other.gameObject);
		}
	}

}
