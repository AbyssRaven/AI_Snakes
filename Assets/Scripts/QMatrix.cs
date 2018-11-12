using AI_Snakes.Main;
using UnityEngine;

namespace AI_Snakes.Utility 
{
    public class QMatrix 
    {
        public Value[,] QualityMatrix {get; private set;}

        public int X;
        public int Y;

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
        
    }

}
