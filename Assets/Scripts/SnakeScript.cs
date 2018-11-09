using System;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{
    private GameObject _nextHead;

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

        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
        }
    }
}


