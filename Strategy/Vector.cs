using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public enum AxisDirection {
		up, down, left, right
	}

    public struct Vector {
		
		public double x { get; private set; }
		public double y { get; private set; }

        Vector(double _x, double _y) {
            x = _x;
            y = _y;
        }

		Vector(AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down:
				this = Vector.down;
				break;
			case AxisDirection.left:
				this = Vector.left;
				break;
			case AxisDirection.right:
				this = Vector.right;
				break;
			case AxisDirection.up:
				this = Vector.up;
				break;
			}
		}

		public AxisDirection direction {
			get {
				if (this * Vector.up >= 0.5) return AxisDirection.up;
				if (this * Vector.down >= 0.5) return AxisDirection.down;
				if (this * Vector.left >= 0.5) return AxisDirection.left;
				return AxisDirection.right;
			}
		}
      
        public static Vector operator + (Vector a, Vector b) {
            return new Vector(a.x + b.x, a.y + b.y);
        }

        public static Vector operator - (Vector a, Vector b) {
            return new Vector(a.x - b.x, a.y - b.y);
        }

        public static Vector operator * (Vector a, double value) {
            return new Vector(a.x * value, a.y * value);
        }

		public static Vector operator / (Vector a, double value) {
			return new Vector(a.x / value, a.y / value);
		}

        public static double operator * (Vector a, Vector b) {
            return (a.x * b.x) + (a.y * b.y);
        }

		public double length {
			get {
				return Math.Sqrt(x * x + y * y);
			}
		}

		public Vector normalized {
			get {
				return this / this.length;
			}
		}

		public static Vector up = new Vector(0, -1);
		public static Vector down = new Vector(0, 1);
		public static Vector left = new Vector(-1, 0);
		public static Vector right = new Vector(1, 0);

		public static bool operator == (Vector a, Vector b) {
			const double E = 0.0001;

			return Math.Abs(a.x - b.x) < E && Math.Abs(a.y - b.y) < E;
		}
    }
}
