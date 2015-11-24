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

		public VirtualVehicle(Vehicle vehicle) : this() {
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
			//var rotFriction = Math.Pow(1 - Constants.vehicleRotationAirFriction, Constants.physicsTickFactor);

			angularSpeed = Constants.vehicleAngularSpeedFactor * steeringAngle * (speed * forward);

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

				angle += angularSpeed * Constants.physicsTickFactor;
			}

			enginePower = MyMath.limit(enginePower + MyMath.limit(newEnginePower - enginePower, Constants.maxEnginePowerChange), 1.0);
			steeringAngle = MyMath.limit(steeringAngle + MyMath.limit(newSteeringAngle - steeringAngle, Constants.maxSteeringAngleChange), 1.0);
		} 

		public void simulateTickRoughly(double newEnginePower, double newSteeringAngle) {

			double physicsTicks = 1;

			double physicsTickFactor = 1 / physicsTicks;

			var acceleration = forward * Constants.getAcceleration(type, enginePower) * physicsTickFactor;

			var airFriction = Math.Pow(1 - Constants.vehicleMovementAirFriction, physicsTickFactor);

			angularSpeed = Constants.vehicleAngularSpeedFactor * steeringAngle * (speed * forward) * physicsTickFactor;
			
			position += speed * physicsTickFactor;
			speed += acceleration;
			speed *= airFriction;

			var lengthFriction =
				Math.Max(Math.Min(speed * forward, Constants.vehicleLengthFriction * physicsTickFactor), -Constants.vehicleLengthFriction * physicsTickFactor) * forward;

			var crossFriction =
				Math.Max(Math.Min(speed % forward, Constants.vehicleCrossFriction * physicsTickFactor), -Constants.vehicleCrossFriction * physicsTickFactor) * forward.rotate(-Math.PI * 0.5);

			speed -= lengthFriction;
			speed -= crossFriction;

			angle += angularSpeed * physicsTickFactor;
			

			enginePower = MyMath.limit(enginePower + MyMath.limit(newEnginePower - enginePower, Constants.maxEnginePowerChange * physicsTickFactor), 1.0);
			steeringAngle = MyMath.limit(steeringAngle + MyMath.limit(newSteeringAngle - steeringAngle, Constants.maxSteeringAngleChange * physicsTickFactor), 1.0);
		}
	}
}

