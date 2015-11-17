using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
    public struct Vector
    {
        public double x;
        public double y;

        Vector(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
      
        public static Vector operator + (Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y);
        }

        public static Vector operator - (Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y);
        }

        public static Vector operator * (Vector a, double value)
        {
            return new Vector(a.x * value, a.y * value);
        }

        public static double operator * (Vector a, Vector b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }
    }
}
