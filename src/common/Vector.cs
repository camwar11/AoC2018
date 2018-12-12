namespace common
{
    public class Vector : Point
    {
        private long _xVelocity;
        private long _yVelocity;

        public Vector(long x, long y, long xVelocity, long yVelocity) : base(x, y)
        {
            _xVelocity = xVelocity;
            _yVelocity = yVelocity;
        }

        public Vector(string x, string y, string xVelocity, string yVelocity) : 
            this(long.Parse(x), long.Parse(y), long.Parse(xVelocity), long.Parse(yVelocity))
        {

        }

        public Vector Move(long moves = 1)
        {
            long xChange = moves * _xVelocity;
            long yChange = moves * _yVelocity;
            return new Vector(this.X + xChange, this.Y + yChange, this._xVelocity, this._yVelocity);
        }
    }
}