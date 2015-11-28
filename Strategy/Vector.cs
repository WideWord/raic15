using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	public enum AxisDirection {
		up, down, left, right
	}

	public static class AxisDirectionExtensions {
		public static AxisDirection Back(this AxisDirection dir) {
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

		public static AxisDirection TurnLeft(this AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down: return AxisDirection.right;
			case AxisDirection.left: return AxisDirection.down;
			case AxisDirection.right: return AxisDirection.up;
			case AxisDirection.up: return AxisDirection.left;
			default: return AxisDirection.down;
			}
		}

		public static AxisDirection TurnRight(this AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down: return AxisDirection.left;
			case AxisDirection.left: return AxisDirection.up;
			case AxisDirection.right: return AxisDirection.down;
			case AxisDirection.up: return AxisDirection.right;
			default: return AxisDirection.down;
			}
		}

		public static bool IsSameAxis(this AxisDirection a, AxisDirection b) {
			if (a == AxisDirection.left || a == AxisDirection.right) {
				return b == AxisDirection.right || b == AxisDirection.left;
			} else {
				return b == AxisDirection.up || b == AxisDirection.down;
			}
		}

		public static double Angle(this AxisDirection dir) {
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

		public Vector(double _x, double _y) : this() {
			x = _x;
			y = _y;
		}

		public Vector(AxisDirection dir) : this() {
			x = 0;
			y = 0;
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

		public static Vector FromAngle(double angle) {
			return new Vector(Math.Cos(angle), Math.Sin(angle));
		}

		public AxisDirection Direction {
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

		public static Vector operator - (Vector a) {
			return new Vector(-a.x, -a.y);
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

		public static double operator % (Vector a, Vector b) {
			return a.Cross(b);
		}

		public double Length {
			get {
				return Math.Sqrt(x * x + y * y);
			}
		}

		public Vector Normalized {
			get {
				return this / this.Length;
			}
		}

		public double Angle {
			get {
				return Math.Atan2(y, x);
			}
		}

		public double AngleTo(Vector o) {
			double relAngle = o.Angle - Angle;
			while (relAngle > Math.PI)
				relAngle -= 2 * Math.PI;

			while (relAngle < -Math.PI)
				relAngle += 2 * Math.PI;

			return relAngle;
		}

		public Vector Rotate(double deltaAngle) {
			return Vector.FromAngle(deltaAngle + this.Angle) * this.Length;
		}

		public static Vector up = new Vector(0, -1);
		public static Vector down = new Vector(0, 1);
		public static Vector left = new Vector(-1, 0);
		public static Vector right = new Vector(1, 0);

		public static bool Equals(Vector a, Vector b) {
			const double E = 0.0001;

			return Math.Abs(a.x - b.x) < E && Math.Abs(a.y - b.y) < E;
		}


		public double Cross(Vector o) {
			return x * o.y - y * o.x;
		}

		public void Draw(int color) {
			Debug.FillCircle(this, 15, color);
		}
    }

	public struct Ray {

		public Vector Position { get; private set; }
		public Vector Direction { get; private set; }

		public Vector StartPoint {
			get {
				return Position;
			}
		}

		public Vector EndPoint {
			get {
				return Position + Direction;
			}
		}

		public Ray(Vector position, Vector direction) : this() {
			this.Position = position;
			this.Direction = direction;
		}
		
		public static Ray Line(Vector p1, Vector p2) {
			return new Ray(p1, p2 - p1);
		}

		public static Ray operator * (Ray a, double val) {
			return new Ray(a.Position, a.Direction * val);
		}

		public static Ray operator * (double val, Ray a) {
			return a * val;
		}

		public static Ray operator - (Ray r) {
			return new Ray(r.Position, -r.Direction);
		}

		public Vector? Intersect(Ray o) {
			var p = Position;
			var r = Direction;

			var q = o.Position;
			var s = o.Direction;

			if (r.Cross (s) == 0) {
				return null;
			} else {
				var t = (q - p).Cross(s) / r.Cross(s);
				var u = -(p - q).Cross(r) / r.Cross(s);

				if (t >= 0 && t <= 1 && u >= 0 && u <= 1) {
					return p + t * r;
				} else {
					return null;
				}
			}
		}

		public double LineDistance(Vector p) {

			var p1 = this.StartPoint;
			var p2 = this.EndPoint;

			return Math.Abs((p2.y - p1.y) * p.x - (p2.x - p1.x) * p.y + p2.x * p1.y - p2.y * p1.x)
			/
			(p1 - p2).Length;

		}

		public void Draw(int color) {
			Debug.Line(StartPoint, EndPoint, color);
		}

	}

	public struct Arc {

		public Vector Position { get; private set; }
		public double Radius { get; private set; }

		public double FromAngle { get; private set; }
		public double ToAngle { get; private set; }

		public Arc(Vector position, double _raduis, double fromAngle, double toAngle) : this() {
			while (fromAngle > Math.PI)
				fromAngle -= Math.PI * 2;
			while (fromAngle < -Math.PI)
				fromAngle += Math.PI * 2;

			while (toAngle > Math.PI)
				toAngle -= Math.PI * 2;
			while (toAngle < -Math.PI)
				toAngle += Math.PI * 2;

			this.Position = position;
			this.Radius = _raduis;
			this.FromAngle = fromAngle;
			this.ToAngle = toAngle;
		}

		public bool IsPointInArcSector(Vector point) {

			double angle = (point - Position).Angle;

			if (FromAngle > ToAngle) {
				return angle > FromAngle || angle < ToAngle;
			} else {
				return angle < ToAngle && angle > FromAngle;
			}
		}

		public Vector PointAtAngle(double angle) {
			return Vector.FromAngle(angle) * Radius + Position;
		}

		public Vector[] MultiIntersect(Ray ray) {

			var e = ray.Position;
			var d = ray.Direction;
			var f = e - Position;

			var a = d * d;
			var b = 2 * (f * d);
			var c = (f * f) - Radius * Radius;

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

			var p1 = ray.Position + ray.Direction * t1;
			var p2 = ray.Position + ray.Direction * t2;


			var havePoint1 = t1 >= 0 && t1 <= 1 && IsPointInArcSector(p1);
			var havePoint2 = t2 >= 0 && t2 <= 1 && IsPointInArcSector(p2);

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

		public Vector? Intersect(Ray ray) {
			var intersections = MultiIntersect(ray);

			if (intersections == null)
				return null;

			Vector? vec = null;
			var dist = double.MaxValue;

			foreach (var intersection in intersections) {
				var curDist = (intersection - ray.Position).Length;
				if (curDist < dist) {
					dist = curDist;
					vec = intersection;
				}
			}
			return vec;
		}

		public void Draw(int color) {
			Vector lastPoint = PointAtAngle(FromAngle);
			double angle = FromAngle;

			if (FromAngle > ToAngle) {
				while (angle < Math.PI) {
					angle += 0.1;
					var point = PointAtAngle(angle);
					Debug.Line(lastPoint, point, color);
					lastPoint = point;
				}

				angle -= Math.PI * 2;
			}

			while (angle < ToAngle - 0.0001) {
				if (angle + 0.1 > ToAngle)
					angle = ToAngle;
				else
					angle += 0.1;
				
				var point = PointAtAngle(angle);
				Debug.Line(lastPoint, point, color);
				lastPoint = point;
			}
		}

	}

	public struct Circle {

		public Vector Position { get; private set; }
		public double Radius { get; private set; }

		public Circle(Vector pos, double rad) : this() {
			Position = pos;
			Radius = rad;
		}

		public bool IsIntersect(Ray ray) {

			var e = ray.Position;
			var d = ray.Direction;
			var f = e - Position;

			var a = d * d;
			var b = 2 * (f * d);
			var c = (f * f) - Radius * Radius;

			var D = b * b - 4 * a * c;

			if (D < 0)
				return false;

			if (D == 0) { // impossible
				D = 0.00001;
			}

			var sD = Math.Sqrt(D);
			var a2 = a * 2;

			var t1 = (-b + sD) / a2;
			var t2 = (-b - sD) / a2;

			var p1 = ray.Position + ray.Direction * t1;
			var p2 = ray.Position + ray.Direction * t2;


			var havePoint1 = t1 >= 0 && t1 <= 1;
			var havePoint2 = t2 >= 0 && t2 <= 1;

			return havePoint1 || havePoint2;
		}

		public void Draw(int color) {
			Debug.Circle(Position, Radius, color);
		}

	}

	public struct Rect {

		public Vector Min;
		public Vector Max;

		public Rect(Vector a, Vector b) {
			Min = new Vector(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
			Max = new Vector(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
		}

		public bool contains(Vector point) {
			return point.x <= Max.x && point.x >= Min.x && point.y <= Max.y && point.y >= Min.y;
		}

		public Ray lineForDirection(AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down:
				return Ray.Line(Max, new Vector(Min.x, Max.y));
			case AxisDirection.left:
				return Ray.Line(Min, new Vector(Min.x, Max.y));
			case AxisDirection.right:
				return Ray.Line(Max, new Vector(Max.x, Min.y));
			case AxisDirection.up:
				return Ray.Line(Min, new Vector(Max.x, Min.y));
			default:
				return Ray.Line(Min, Min);
			}
		}

		public AxisDirection? BorderAnyIntersectionDirection(Ray ray, AxisDirection? exceptDir = null) {
			foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				if (dir == exceptDir)
					continue;

				var side = lineForDirection(dir);
				if (ray.Intersect(side) != null) {
					//(ray.intersect(side) ?? new Vector(0, 0)).draw(0xFF0000);
					return dir;
				}

			}
			return null;
		}

		public void Draw(int color) {
			/*foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				lineForDirection(dir).draw(color);

			}*/
			Debug.Rect(Min, Max, color);				
		}

	}

	public struct FreeRect {

		public Vector Position { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public double Angle { get; private set; }

		public Vector Forward { get { return Vector.FromAngle(Angle); } }

		public FreeRect(Vector pos, double width, double height, double angle) : this() {
			Position = pos;
			this.Width = width / 2;
			this.Height = height / 2;
			this.Angle = angle;
		}

		public Ray EdgeInDirection(AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down:
				return Ray.Line(
					(Position + new Vector(-Width, Height).Rotate(Angle)), 
					(Position + new Vector(Width, Height).Rotate(Angle))
				);
			case AxisDirection.left:
				return Ray.Line(
					(Position + new Vector(-Width, Height).Rotate(Angle)), 
					(Position + new Vector(-Width, -Height).Rotate(Angle))
				);
			case AxisDirection.right:
				return Ray.Line(
					(Position + new Vector(Width, Height).Rotate(Angle)), 
					(Position + new Vector(Width, -Height).Rotate(Angle))
				);
			case AxisDirection.up:
				return Ray.Line(
					(Position + new Vector(-Width, -Height).Rotate(Angle)), 
					(Position + new Vector(Width, -Height).Rotate(Angle))
				);
			}
			return new Ray(Vector.up, Vector.up);
		}

		public bool IsIntersect(Ray ray) {
			
			foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				var edge = EdgeInDirection(dir);
				if (ray.Intersect(edge) != null) {
					return true;
				}
			}

			return false;
		}

		public bool IsIntersect(Arc arc) {

			foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				var edge = EdgeInDirection(dir);
				if (arc.Intersect(edge) != null) {
					return true;
				}
			}

			return false;
		}

		public bool IsIntersect(Circle circle) {
			foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				var edge = EdgeInDirection(dir);
				if (circle.IsIntersect(edge)) {
					return true;
				}
			}

			return false;
		}

		public void Draw(int color) {
			foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				var edge = EdgeInDirection(dir);
				edge.Draw(color);
			}
		}

	}

	public struct MyMath {

		public static double Limit(double val, double limit) {
			return Math.Max(Math.Min(val, limit), -limit);
		}

	}

}

