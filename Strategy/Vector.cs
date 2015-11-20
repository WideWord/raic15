using System;
using System.Collections.Generic;

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

		public static AxisDirection turnLeft(this AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down: return AxisDirection.right;
			case AxisDirection.left: return AxisDirection.down;
			case AxisDirection.right: return AxisDirection.up;
			case AxisDirection.up: return AxisDirection.left;
			default: return AxisDirection.down;
			}
		}

		public static AxisDirection turnRight(this AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down: return AxisDirection.left;
			case AxisDirection.left: return AxisDirection.up;
			case AxisDirection.right: return AxisDirection.down;
			case AxisDirection.up: return AxisDirection.right;
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

		public static double angle(this AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down: return Math.PI * 0.5;
			case AxisDirection.left: return Math.PI;
			case AxisDirection.right: return 0;
			case AxisDirection.up: return -Math.PI * 0.5;
			default: return 0;
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

		public double angle {
			get {
				return Math.Atan2(y, x);
			}
		}

		public double angleTo(Vector o) {
			double relAngle = o.angle - angle;
			while (relAngle > Math.PI)
				relAngle -= 2 * Math.PI;

			while (relAngle < -Math.PI)
				relAngle += 2 * Math.PI;

			return relAngle;
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

		public void draw(int color) {
			Debug.fillCircle(this, 15, color);
		}
    }

	public struct Ray {

		public Vector position { get; private set; }
		public Vector direction { get; private set; }

		public Vector p1 {
			get {
				return position;
			}
		}

		public Vector p2 {
			get {
				return position + direction;
			}
		}

		public Ray(Vector position, Vector direction) {
			this.position = position;
			this.direction = direction;
		}
		
		public static Ray line(Vector p1, Vector p2) {
			return new Ray(p1, p2 - p1);
		}

		public static Ray operator * (Ray a, double val) {
			return new Ray(a.position, a.direction * val);
		}

		public static Ray operator * (double val, Ray a) {
			return a * val;
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
				var u = -(p - q).cross(r) / r.cross(s);

				if (t >= 0 && t <= 1 && u >= 0 && u <= 1) {
					return p + t * r;
				} else {
					return null;
				}
			}
		}

		public double lineDistance(Vector p) {

			var p1 = this.p1;
			var p2 = this.p2;

			return Math.Abs((p2.y - p1.y) * p.x - (p2.x - p1.x) * p.y + p2.x * p1.y - p2.y * p1.x)
			/
			(p1 - p2).length;

		}

		public void draw(int color) {
			Debug.line(p1, p2, color);
		}

	}

	public struct Arc {

		public Vector position { get; private set; }
		public double radius { get; private set; }

		public double fromAngle { get; private set; }
		public double toAngle { get; private set; }

		public Arc(Vector position, double _raduis, double fromAngle, double toAngle) {
			while (fromAngle > Math.PI)
				fromAngle -= Math.PI * 2;
			while (fromAngle < -Math.PI)
				fromAngle += Math.PI * 2;

			while (toAngle > Math.PI)
				toAngle -= Math.PI * 2;
			while (toAngle < -Math.PI)
				toAngle += Math.PI * 2;

			this.position = position;
			this.radius = _raduis;
			this.fromAngle = fromAngle;
			this.toAngle = toAngle;
		}

		public bool isPointInArcSector(Vector point) {

			double angle = (point - position).angle;

			if (fromAngle > toAngle) {
				return angle > fromAngle || angle < toAngle;
			} else {
				return angle < toAngle && angle > fromAngle;
			}
		}

		public Vector pointAtAngle(double angle) {
			return Vector.fromAngle(angle) * radius + position;
		}

		public Vector[] multiIntersect(Ray ray) {

			var e = ray.position;
			var d = ray.direction;
			var f = e - position;

			var a = d * d;
			var b = 2 * (f * d);
			var c = (f * f) - radius * radius;

			var D = b * b - 4 * a * c;

			if (D < 0)
				return null;
			
			if (D == 0) { // impossible
				D = 0.00001;
			}

			var sD = Math.Sqrt(D);
			var a2 = a * 2;

			var t1 = (-b + sD) / a2;
			var t2 = (-b - sD) / a2;

			var p1 = ray.position + ray.direction * t1;
			var p2 = ray.position + ray.direction * t2;


			var havePoint1 = t1 >= 0 && t1 <= 1 && isPointInArcSector(p1);
			var havePoint2 = t2 >= 0 && t2 <= 1 && isPointInArcSector(p2);

			if (havePoint1 && havePoint2) {
				return new Vector[] { p1, p2 };
			} else if (havePoint1) {
				return new Vector[] { p1 };
			} else if (havePoint2) {
				return new Vector[] { p2 };
			} else {
				return null;
			}

		}

		public Vector? intersect(Ray ray) {
			var intersections = multiIntersect(ray);

			if (intersections == null)
				return null;

			Vector? vec = null;
			var dist = double.MaxValue;

			foreach (var intersection in intersections) {
				var curDist = (intersection - ray.position).length;
				if (curDist < dist) {
					dist = curDist;
					vec = intersection;
				}
			}
			return vec;
		}

		public void draw(int color) {
			Vector lastPoint = pointAtAngle(fromAngle);
			double angle = fromAngle;

			if (fromAngle > toAngle) {
				while (angle < Math.PI) {
					angle += 0.1;
					var point = pointAtAngle(angle);
					Debug.line(lastPoint, point, color);
					lastPoint = point;
				}

				angle -= Math.PI * 2;
			}

			while (angle < toAngle - 0.0001) {
				if (angle + 0.1 > toAngle)
					angle = toAngle;
				else
					angle += 0.1;
				
				var point = pointAtAngle(angle);
				Debug.line(lastPoint, point, color);
				lastPoint = point;
			}
		}

	}

	public struct Rect {

		public Vector min;
		public Vector max;

		public Rect(Vector a, Vector b) {
			min = new Vector(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
			max = new Vector(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
		}

		public bool contains(Vector point) {
			return point.x <= max.x && point.x >= min.x && point.y <= max.y && point.y >= min.y;
		}

		public Ray lineForDirection(AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down:
				return Ray.line(max, new Vector(min.x, max.y));
			case AxisDirection.left:
				return Ray.line(min, new Vector(min.x, max.y));
			case AxisDirection.right:
				return Ray.line(max, new Vector(max.x, min.y));
			case AxisDirection.up:
				return Ray.line(min, new Vector(max.x, min.y));
			default:
				return Ray.line(min, min);
			}
		}

		public AxisDirection? borderAnyIntersectionDirection(Ray ray, AxisDirection? exceptDir = null) {
			foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				if (dir == exceptDir)
					continue;

				var side = lineForDirection(dir);
				if (ray.intersect(side) != null) {
					//(ray.intersect(side) ?? new Vector(0, 0)).draw(0xFF0000);
					return dir;
				}

			}
			return null;
		}

		public void draw(int color) {
			/*foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				lineForDirection(dir).draw(color);

			}*/
			Debug.rect(min, max, color);				
		}

	}

}

