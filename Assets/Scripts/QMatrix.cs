using System.Collections.Generic;
using AI_Snakes.Snake;
using UnityEngine;

namespace AI_Snakes.Utility 
{
    public class QMatrix 
    {
        public Value[,] QualityMatrix {get; set;}       

        public int X { get; private set; }
        public int Y { get; private set; }
        private int _generationAmount;

        public QMatrix(Vector2Int food) 
        {
            QualityMatrix = new Value[GameController.GetController().FieldSize.x,GameController.GetController().FieldSize.y];

            X = food.x;
            Y = food.y;
            
            for(int i = 0; i < QualityMatrix.GetLength(0); i++)
            {
                for(int j = 0; j < QualityMatrix.GetLength(1); j++) 
                {
                    QualityMatrix[i, j] = new Value();;
                }
            }
        }
        
        public bool GetCoordinateEquals(int x, int y) 
        {
            return X == x && Y == y;
        }

        public int Generations
        {
            get { return _generationAmount; }
            set { _generationAmount = value; }
        }
    }

}
