using System;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public struct VirtualVehicle {

		public Vector position { get; private set; }
		public double angle { get; private set; }
		public Vector speed { get; private set; }
		public double angularSpeed { get; private set; }
		public double enginePower { get; private set; }
		public double steeringAngle { get; private set; }

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

		public FreeRect Rect {
			get {
				return new FreeRect(position, Constants.vehicleLength, Constants.vehicleWidth, angle);
			}
		}

		public VirtualVehicle(Vehicle vehicle) {
			position = vehicle.position;
			angle = vehicle.angle;
			speed = vehicle.speed;
			angularSpeed = vehicle.angularSpeed;
			enginePower = vehicle.enginePower;
			steeringAngle = vehicle.steeringAngle;
		}

		void simulate(float enginePower, float steeringAngle) {
			
		}

	}
}

