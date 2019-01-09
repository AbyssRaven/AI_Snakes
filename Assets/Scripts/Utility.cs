using System;
using System.Collections.Generic;
using AI_Game.Utility;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Game.Utility
{
    public class Utility 
    {
    }

    public static class MoreMath
    {
        // This method only exists for ease of usage, because there doesnt exist an easier way to find the max of a group of doubles
        public static double Max(double x, double y)
        {
            return Math.Max(x, y);
        }

        public static double Max(double x, double y, double z)
        {
            return Math.Max(x, Math.Max(y, z));
        }

        public static double Max(double w, double x, double y, double z)
        {
            return Math.Max(w, Math.Max(x, Math.Max(y, z)));
        }
    }

    public struct pos 
    {
        public int x;
        public int y;
    }
    
   

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left,
        None,
    }
}

