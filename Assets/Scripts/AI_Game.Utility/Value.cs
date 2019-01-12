namespace AI_Game.Utility
{
    public class Value
    {
        private double _up;
        private double _right;
        private double _down;
        private double _left;

        private double _fieldValue;

        //Sets the Q value for a direction for a specific fieldSize position in the matrix
        public void SetDirectionValue(Direction dir, double value)
        {
            switch (dir)
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

        //Calls a q Value fro a direction for a specific fieldSize position in the matrix
        public double GetDirectionValue(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return _up;
                case Direction.Right:
                    return _right;
                case Direction.Down:
                    return _down;
                case Direction.Left:
                    return _left;
                default:
                    return 0;
            }
        }

        public void SetFieldValue(double value) {
            _fieldValue = value;

        }

        public double GetFieldValue() {
            return _fieldValue;
        }
    }
}
