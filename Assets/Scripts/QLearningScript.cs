using System.Collections;
using System.Collections.Generic;
using Ai_Snakes.Scripts.GameControllerScript;
using UnityEngine;

public class QLearningScript : MonoBehaviour {

    public FoodLocation[,] QMatrix { get; set; }

    public int XCoordinate { get; private set; }
    public int YCoordinate { get; private set; }

    public QMatrixCreation(Vector2Int food)
    {
        QMatrix = new FoodLocation[GameControllerScript.]
    }
}
