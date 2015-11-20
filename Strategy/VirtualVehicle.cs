using System;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public struct VirtualVehicle {

		public Vector position;
		public double angle;

		public Vector forward {
			get {
				return Vector.fromAngle(angle);
			}
		}

		public Ray forwardRay {
			get {
				return new Ray(position, forward);
			}
		}

		public VirtualVehicle(Vehicle vehicle) {
			position = vehicle.position;
			angle = vehicle.angle;
		}

	}
}

