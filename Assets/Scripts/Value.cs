using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Snakes.Utility
{
    public class Value
    {
        private double _up = 0;
        private double _right = 0;
        private double _down = 0;
        private double _left = 0;


        public void SetValue(Direction dir, double value)
        {
            switch(dir) 
            {
                    case Direction.Up:
                        _up = value;
                        break;
                    case Direction.Right:
                        _right = value;
                        break;
                    case Direction.Down:
                        _down = value;
                        break;
                    case Direction.Left:
                        _left = value;
                        break;
            }
        }
        
        public double GetValue(Direction dir) 
        {
//            switch (dir) 
//            {
//                case Direction.Up:
//                    return _up;
//                case Direction.Down:
//                    return _down;
//                case Direction.Right:
//                    return _right;
//                case Direction.Left:
//                    return _left;
//            }
        }
    }
}
