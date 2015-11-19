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

		public Vector forward {
			get {
				return Vector.fromAngle(car.Angle);
			}
		}

		public Ray forwardRay {
			get {
				return new Ray(position, forward);
			}
		}
	}
}

