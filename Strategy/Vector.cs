using System;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	public enum AxisDirection {
		up, down, left, right
	}

	public static class AxisDirectionExtensions {
		public static AxisDirection back(this AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down:
				return AxisDirection.up;
			case AxisDirection.left:
				return AxisDirection.right;
			case AxisDirection.right:
				return AxisDirection.left;
			case AxisDirection.up:
				return AxisDirection.down;
			default:
				return AxisDirection.down;
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
				if (this * Vector.up >= 0.5)
					return AxisDirection.up;
				if (this * Vector.down >= 0.5)
					return AxisDirection.down;
				if (this * Vector.left >= 0.5)
					return AxisDirection.left;
				return AxisDirection.right;
			}
		}

		public static Vector operator +(Vector a, Vector b) {
			return new Vector(a.x + b.x, a.y + b.y);
		}

		public static Vector operator -(Vector a, Vector b) {
			return new Vector(a.x - b.x, a.y - b.y);
		}

		public static Vector operator *(Vector a, double value) {
			return new Vector(a.x * value, a.y * value);
		}

		public static Vector operator *(double value, Vector a) {
			return a * value;
		}

		public static Vector operator /(Vector a, double value) {
			return new Vector(a.x / value, a.y / value);
		}

		public static Vector operator /(double value, Vector a) {
			return a / value;
		}

		public static double operator *(Vector a, Vector b) {
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

		public static double cross(Vector a, Vector b) {
			return a.x * b.y - b.x * a.y;
		}
	}

	public struct Ray {
		public Vector position { get; private set; }
		public Vector direction { get; private set; }

		public Ray(Vector position, Vector direction) {
			this.position = position;
			this.direction = direction;
		}

		public static Vector? intersection(Ray a, Ray b) {
			var p = a.position;
			var r = a.direction;

			var q = b.position;
			var s = b.direction;

			if (Vector.cross(r, s) == 0)
				return null;

			var t = Vector.cross((q - p), s) / Vector.cross(r, s);
			var u = Vector.cross((p - q), r) / Vector.cross(s, r);

			if (0.0 <= t && t <= 1.0 && 0.0 <= u && u <= 1.0) {
				return p + t * r;
			} else {
				return null;
			}
		}

		public static Vector? intersection(Ray a, Arc b) {
			return null;
		}

		public static Vector? intersection(Arc b, Ray a) {
			return Ray.intersection(a, b);
		}

		public Line line {
			get {
				return new Line(position, position + direction);
			}
		}
	}

	public struct Line {
		public Vector p1 { get; private set; }
		public Vector p2 { get; private set; }

		public Line(Vector p1, Vector p2) {
			this.p1 = p1;
			this.p2 = p2;
		}

		public Ray ray {
			get {
				return new Ray(p1, p2 - p1);
			}
		}

		public static Vector? intersection(Line a, Line b) {
			return Ray.intersection(a.ray, b.ray);
		}
	}

	public struct Arc {

		public Vector position { get; private set; }
		public double fromAngle { get; private set; }
		public double toAngle { get; private set; }

		public Arc(Vector position, double from, double to) {
			fromAngle = from;
			toAngle = to;
			this.position = position;
		}


	}

}
