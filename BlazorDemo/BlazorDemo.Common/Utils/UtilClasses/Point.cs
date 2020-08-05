using System;

namespace BlazorDemo.Common.Utils.UtilClasses
{
    public struct Point : IEquatable<Point>
    {
        private const double _tolerance = 0.0001;

        public static readonly Point Origin = new Point(0, 0);

        public double X { get; set; }
        public double Y { get; set; }

        public Point(long x, long y)
        {
            X = x;
            Y = y;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Distance(Point that)
        {
            var dX = X - that.X;
            var dY = Y - that.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public double DistanceSquared(Point that)
        {
            var dX = X - that.X;
            var dY = Y - that.Y;
            return dX * dX + dY * dY;
        }

        public double DistanceXY(Point that)
        {
            var dX = Math.Abs(X - that.X);
            var dY = Math.Abs(Y - that.Y);
            return dX + dY;
        }

        public override bool Equals(object o)
        {
            if (o == null || GetType() != o.GetType())
                return false;
            var that = (Point)o;
            return Equals(that);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() * 23) ^ (Y.GetHashCode() * 17);
        }

        public bool IsAbove(Point that)
        {
            return X > that.Y;
        }

        public bool IsBelow(Point that)
        {
            return X < that.Y;
        }

        public bool IsLeftOf(Point that)
        {
            return X < that.X;
        }

        public bool IsRightOf(Point that)
        {
            return X > that.X;
        }

        public static Point Midpoint(Point a, Point b)
        {
            return new Point(
                (a.X + b.X) / 2,
                (a.Y + b.Y) / 2
            );
        }

        public static double Slope(Point from, Point to)
        {
            return (to.Y - from.Y) / (to.X - from.X);
        }

        public Point Translate(long dx, long dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public Point Translate(int dx, int dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public Point Translate(double dx, double dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public override string ToString()
        {
            return $"({X:0.00}, {Y:0.00})";
        }

        public static double Angle(Point p1, Point p2, Point refp)
        {
            return Math.Atan2(p1.Y - refp.Y, p1.X - refp.X) - Math.Atan2(p2.Y - refp.Y, p2.X - refp.X) * 180 / Math.PI;
        }

        public bool Equals(Point that)
        {
            return Math.Abs(X - that.X) < _tolerance && Math.Abs(Y - that.Y) < _tolerance;
        }
    }
}
