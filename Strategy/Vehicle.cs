using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class Vehicle {

		private Car car;

		public void SetCar(Car car) {
			this.car = car;
		}

		public Vector Position {
			get {
				return new Vector(car.X, car.Y);
			}
		}

		public double Angle {
			get {
				return car.Angle;
			}
		}

		public double AngularSpeed {
			get {
				return car.AngularSpeed;
			}
		}

		public Vector Forward {
			get {
				return Vector.FromAngle(Angle);
			}
		}

		public Vector Speed {
			get {
				return new Vector(car.SpeedX, car.SpeedY);
			}
		}

		public double EnginePower {
			get {
				return car.EnginePower;
			}
		}

		public double SteeringAngle {
			get {
				return car.WheelTurn;
			}
		}

		public Ray ForwardRay {
			get {
				return new Ray(Position, Forward);
			}
		}

		public double SteeringAngleForDirection(Vector direction) {
			return Forward.AngleTo(direction) * 4 * Math.Sign(Speed * Forward);
		}

		public Vector StabilizationDir(Vector baseDir, Vector sideCenter, Vector sideDir, double sidePosition) {
			var dist = (Position - sideCenter) * sideDir + sidePosition;
			return baseDir - sideDir * dist / Constants.TileSize;
		}

		public CarType Type {
			get {
				return car.Type;
			}
		}
	}
}

