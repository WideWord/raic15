using System;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public struct VirtualVehicle {

		public Vector position { get; private set; }
		public double angle { get; private set; }
		public Vector speed { get; private set; }
		public double angularSpeed { get; private set; }
		public double enginePower { get; private set; }
		public double steeringAngle { get; private set; }
		public CarType type { get; private set; }

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
			type = vehicle.type;
		}

		public void simulateTick(double newEnginePower, double newSteeringAngle) {

			var acceleration = forward * Constants.getAcceleration(type, enginePower) * Constants.physicsTickFactor;

			var airFriction = Math.Pow(1 - Constants.vehicleMovementAirFriction, Constants.physicsTickFactor);

			for (int i = 0; i < Constants.physicsTicks; ++i) {
				position += speed * Constants.physicsTickFactor;
				speed += acceleration;
				speed *= airFriction;

				var lengthFriction =
					Math.Max(Math.Min(speed * forward, Constants.vehicleLengthFriction * Constants.physicsTickFactor), -Constants.vehicleLengthFriction * Constants.physicsTickFactor) * forward;

				var crossFriction = 
					Math.Max(Math.Min(speed % forward, Constants.vehicleCrossFriction * Constants.physicsTickFactor), -Constants.vehicleCrossFriction * Constants.physicsTickFactor) * forward.rotate(-Math.PI * 0.5);
			
				speed -= lengthFriction;
				speed -= crossFriction;
			}

			enginePower = MyMath.limit(enginePower + MyMath.limit(newEnginePower, Constants.maxEnginePowerChange), 1.0);
			steeringAngle = MyMath.limit(steeringAngle + MyMath.limit(steeringAngle, Constants.maxSteeringAngleChange), 1.0);

		}

	}
}

