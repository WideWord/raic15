using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class Vehicle {

		private Car car;

		public void setCar(Car car) {
			this.car = car;
		}

		public Vector position {
			get {
				return new Vector(car.X, car.Y);
			}
		}

		public double angle {
			get {
				return car.Angle;
			}
		}

		public double angularSpeed {
			get {
				return car.AngularSpeed;
			}
		}

		public Vector forward {
			get {
				return Vector.fromAngle(angle);
			}
		}

		public Vector speed {
			get {
				return new Vector(car.SpeedX, car.SpeedY);
			}
		}

		public double enginePower {
			get {
				return car.EnginePower;
			}
		}

		public double steeringAngle {
			get {
				return car.WheelTurn;
			}
		}

		public Ray forwardRay {
			get {
				return new Ray(position, forward);
			}
		}

		public double steeringAngleForDirection(Vector direction) {
			return forward.angleTo(direction) * 4 * Math.Sign(speed * forward);
		}

		public CarType type {
			get {
				return car.Type;
			}
		}
	}
}

