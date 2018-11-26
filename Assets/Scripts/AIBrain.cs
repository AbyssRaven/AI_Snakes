using System.Collections;
using System.Collections.Generic;
using AI_Snakes.Utility;
using UnityEngine;

namespace AI_Snakes.Snake
{
    public class AIBrain
    {
        private QMatrix _collectedMatrix;
        private List<QMatrix> _data = new List<QMatrix>();

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
            if (_collectedMatrix == null)
            {
                _collectedMatrix = qMatrix;
            }
        }

        public void SaveQMatrix()
        {
            if (_collectedMatrix == null)
            {
                return;
            }

            var food = new Vector2Int(_collectedMatrix.X, _collectedMatrix.Y);
            var data = new QMatrix(food);
            data.Generations = _collectedMatrix.Generations++;

            for (var i = 0; i < _collectedMatrix.QualityMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < _collectedMatrix.QualityMatrix.GetLength(1); j++)
                {
                    var up = _collectedMatrix.QualityMatrix[i, j].GetDirectionValue(Direction.Up);
                    var right = _collectedMatrix.QualityMatrix[i, j].GetDirectionValue(Direction.Right);
                    var down = _collectedMatrix.QualityMatrix[i, j].GetDirectionValue(Direction.Down);
                    var left = _collectedMatrix.QualityMatrix[i, j].GetDirectionValue(Direction.Left);

                    data.QualityMatrix[i, j].SetDirectionValue(Direction.Up, up);
                    data.QualityMatrix[i, j].SetDirectionValue(Direction.Right, right);
                    data.QualityMatrix[i, j].SetDirectionValue(Direction.Down, down);
                    data.QualityMatrix[i, j].SetDirectionValue(Direction.Left, left);

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
            _collectedMatrix = null;
            _data.Add(data);
        }
    }
}
