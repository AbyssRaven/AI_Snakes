using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai_Snakes.Scripts.Food
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left,
        None,
    }

    public class FoodFoodScript
    {
        private double _up = 0;
        private double _right = 0;
        private double _down = 0;
        private double _left = 0;
        private double _none = 0;

        public void SetQValue(Direction dir, double qValue)
        {
            if (double.IsNaN(qValue))
            {
                qValue = 0;
            }

            switch (dir)
            {

            }
        }

        public Direction GetBestDirections(Direction dir)
        {
            return dir;
        }
    }
}
