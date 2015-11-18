using System;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
 
    public enum AxisDirection {
		up, down, left, right
	}

    public static class AxisDirectionExtensions {
        public static AxisDirection back(this AxisDirection dir) {
            switch (dir) {
                case AxisDirection.down: return AxisDirection.up;
                case AxisDirection.left: return AxisDirection.right;
                case AxisDirection.right: return AxisDirection.left;
                case AxisDirection.up: return AxisDirection.down;
                default: return AxisDirection.down;
            }
        }
    }

    public struct Vector {
		
		public double x { get; private set; }
		public double y { get; private set; }

		public Vector(double _x, double _y) {
            x = _x;
            y = _y;
        }

		public Vector(AxisDirection dir) {
            this.x = 0;
            this.y = 0;
			switch (dir) {
			case AxisDirection.down:
				this.x = Vector.down.x;
                this.y = Vector.down.y;
				break;
			case AxisDirection.left:
                this.x = Vector.left.x;
                this.y = Vector.left.y;
                break;
			case AxisDirection.right:
                this.x = Vector.right.x;
                this.y = Vector.right.y;
                break;
			case AxisDirection.up:
                this.x = Vector.up.x;
                this.y = Vector.up.y;
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

		public static bool Equals(Vector a, Vector b) {
			const double E = 0.0001;

			return Math.Abs(a.x - b.x) < E && Math.Abs(a.y - b.y) < E;
		}
    }
}
