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

		public static bool isSameAxis(this AxisDirection a, AxisDirection b) {
			if (a == AxisDirection.left || a == AxisDirection.right) {
				return b == AxisDirection.right || b == AxisDirection.left;
			} else {
				return b == AxisDirection.up || b == AxisDirection.down;
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

		public static Vector fromAngle(double angle) {
			return new Vector(Math.Cos(angle), Math.Sin(angle));
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
			
		public static Vector operator * (double value, Vector a) {
			return a * value;
		}

		public static Vector operator / (Vector a, double value) {
			return new Vector(a.x / value, a.y / value);
		}

		public static Vector operator / (double value, Vector a) {
			return a / value;
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

		public double cross(Vector o) {
			return x * o.y - y * o.x;
		}
    }

	public class Ray {

		public Vector position { get; private set; }
		public Vector direction { get; private set; }

		public Ray(Vector position, Vector direction) {
			this.position = position;
			this.direction = direction;
		}

		public static Ray line(Vector p1, Vector p2) {
			return new Ray(p1, p2 - p1);
		}

		public Vector? intersect(Ray o) {
			var p = position;
			var r = direction;

			var q = o.position;
			var s = o.direction;

			if (r.cross (s) == 0) {
				return null;
			} else {
				var t = (q - p).cross(s) / r.cross(s);
				var u = (p - q).cross(r) / r.cross(s);

				if (t >= 0 && t <= 1 && u >= 0 && u <= 1) {
					return p + t * r;
				} else {
					return null;
				}
			}
		}
	}

}
