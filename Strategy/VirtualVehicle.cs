using System;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public struct VirtualVehicle {

		public Vector Position { get; private set; }
		public double Angle { get; private set; }
		public Vector Speed { get; private set; }
		public double AngularSpeed { get; private set; }
		public double EnginePower { get; private set; }
		public double SteeringAngle { get; private set; }
		public CarType Type { get; private set; }

		public Vector Forward {
			get {
				return Vector.FromAngle(Angle);
			}
		}

		public Ray ForwardRay {
			get {
				return new Ray(Position, Forward);
			}
		}

		public FreeRect Rect {
			get {
				return new FreeRect(Position, Constants.VehicleLength, Constants.VehicleWidth, Angle);
			}
		}

		public VirtualVehicle(Vehicle vehicle) : this() {
			Position = vehicle.Position;
			Angle = vehicle.Angle;
			Speed = vehicle.Speed;
			AngularSpeed = vehicle.AngularSpeed;
			EnginePower = vehicle.EnginePower;
			SteeringAngle = vehicle.SteeringAngle;
			Type = vehicle.Type;
		}

		public void SimulateTick(double newEnginePower, double newSteeringAngle) {

			var acceleration = Forward * Constants.GetAcceleration(Type, EnginePower) * Constants.PhysicsTickFactor;

			var airFriction = Math.Pow(1 - Constants.VehicleMovementAirFriction, Constants.PhysicsTickFactor);
			//var rotFriction = Math.Pow(1 - Constants.vehicleRotationAirFriction, Constants.physicsTickFactor);

			AngularSpeed = Constants.VehicleAngularSpeedFactor * SteeringAngle * (Speed * Forward);

			for (int i = 0; i < Constants.PhysicsTicks; ++i) {
				Position += Speed * Constants.PhysicsTickFactor;
				Speed += acceleration;
				Speed *= airFriction;

				var lengthFriction =
					Math.Max(Math.Min(Speed * Forward, Constants.VehicleLengthFriction * Constants.PhysicsTickFactor), -Constants.VehicleLengthFriction * Constants.PhysicsTickFactor) * Forward;

				var crossFriction = 
					Math.Max(Math.Min(Speed % Forward, Constants.VehicleCrossFriction * Constants.PhysicsTickFactor), -Constants.VehicleCrossFriction * Constants.PhysicsTickFactor) * Forward.Rotate(-Math.PI * 0.5);
			
				Speed -= lengthFriction;
				Speed -= crossFriction;

				Angle += AngularSpeed * Constants.PhysicsTickFactor;
			}

			EnginePower = MyMath.Limit(EnginePower + MyMath.Limit(newEnginePower - EnginePower, Constants.MaxEnginePowerChange), 1.0);
			SteeringAngle = MyMath.Limit(SteeringAngle + MyMath.Limit(newSteeringAngle - SteeringAngle, Constants.MaxSteeringAngleChange), 1.0);
		} 

		public void SimulateTickRoughly(double newEnginePower, double newSteeringAngle) {

			double physicsTicks = 1;

			double physicsTickFactor = 1 / physicsTicks;

			var acceleration = Forward * Constants.GetAcceleration(Type, EnginePower) * physicsTickFactor;

			var airFriction = Math.Pow(1 - Constants.VehicleMovementAirFriction, physicsTickFactor);

			AngularSpeed = Constants.VehicleAngularSpeedFactor * SteeringAngle * (Speed * Forward) * physicsTickFactor;
			
			Position += Speed * physicsTickFactor;
			Speed += acceleration;
			Speed *= airFriction;

			var lengthFriction =
				Math.Max(Math.Min(Speed * Forward, Constants.VehicleLengthFriction * physicsTickFactor), -Constants.VehicleLengthFriction * physicsTickFactor) * Forward;

			var crossFriction =
				Math.Max(Math.Min(Speed % Forward, Constants.VehicleCrossFriction * physicsTickFactor), -Constants.VehicleCrossFriction * physicsTickFactor) * Forward.Rotate(-Math.PI * 0.5);

			Speed -= lengthFriction;
			Speed -= crossFriction;

			Angle += AngularSpeed * physicsTickFactor;
			

			EnginePower = MyMath.Limit(EnginePower + MyMath.Limit(newEnginePower - EnginePower, Constants.MaxEnginePowerChange * physicsTickFactor), 1.0);
			SteeringAngle = MyMath.Limit(SteeringAngle + MyMath.Limit(newSteeringAngle - SteeringAngle, Constants.MaxSteeringAngleChange * physicsTickFactor), 1.0);
		}
	}
}

