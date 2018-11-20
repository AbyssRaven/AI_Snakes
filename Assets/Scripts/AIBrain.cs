using System.Collections;
using System.Collections.Generic;
using AI_Snakes.Utility;
using UnityEngine;

namespace AI_Snakes.Snake
{
    public class AIBrain
    {
        private List<QMatrix> _collectedData;
        private List<QMatrix> _data = new List<QMatrix>();
        private QMatrix _qMatrix;

        public QMatrix FindQMatrixForFood(Vector2Int food)
        {
            foreach (var matrix in _data)
            {
                if (matrix.GetCoordinateEquals(food.x, food.y))
                {
                    return matrix;
                }
            }
            return new QMatrix(food);
        }

        public void DataCollection(QMatrix qMatrix)
        {
            if (_collectedData == null)
            {
                _collectedData = new List<QMatrix>();
            }
            _collectedData.Add(qMatrix);
        }

        private void SaveQMatrix()
        {
            if(_collectedData == null)
            {
                return;
            }

            var food = new Vector2Int(_collectedData[0].X, _collectedData[0].Y);
            var data = new QMatrix(food);
            data.Generations = _collectedData[0].Generations++;

            for(var i = 0; i < _collectedData[0].QualityMatrix.GetLength(0); i++)
            {
                for(var j = 0;j < _collectedData[1].QualityMatrix.GetLength(1); j++)
                {
                    double up = _qMatrix.QualityMatrix[i, j].GetValue(Direction.Up);
                    double right = _qMatrix.QualityMatrix[i, j].GetValue(Direction.Right); 
                    double down = _qMatrix.QualityMatrix[i, j].GetValue(Direction.Down);
                    double left = _qMatrix.QualityMatrix[i, j].GetValue(Direction.Left);

                    data.QualityMatrix[i, j].SetValue(Direction.Up, up);
                    data.QualityMatrix[i, j].SetValue(Direction.Right, right);
                    data.QualityMatrix[i, j].SetValue(Direction.Down, down);
                    data.QualityMatrix[i, j].SetValue(Direction.Left, left);

                }
            }

            foreach (var matrix in _data)
            {
                if (matrix.GetCoordinateEquals(data.X, data.Y))
                {
                    matrix.QualityMatrix = data.QualityMatrix;
                    return;
                }
            }
            _collectedData.Clear();
            _data.Add(data);
        }
    }
}
