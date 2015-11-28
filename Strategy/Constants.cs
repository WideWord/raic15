using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public static class Constants {

		public static readonly double RoadMargin = 80.0;

		public static double HalfTileSize { get { return TileSize * 0.5; } }

		public static readonly int PhysicsTicks = 10;
		public static readonly double PhysicsTickFactor = 1.0 / (double)PhysicsTicks;

		private static Game game;
		private static World world;

		public static double TileSize { get { return game.TrackTileSize; } }

		public static double BuggyEngineForwardPower { get { return game.BuggyEngineForwardPower; } }
		public static double JeepEngineForwardPower { get { return game.JeepEngineForwardPower; } }
		public static double BuggyEngineRearPower { get { return game.BuggyEngineRearPower; } }
		public static double JeepEngineRearPower { get { return game.JeepEngineRearPower; } }

		public static double GetEnginePower(CarType carType, AxisDirection direction) {
			if (carType == CarType.Buggy) {
				if (direction == AxisDirection.up) {
					return BuggyEngineForwardPower;
				} else {
					return BuggyEngineRearPower;
				}
			} else {
				if (direction == AxisDirection.up) {
					return JeepEngineForwardPower;
				} else {
					return JeepEngineForwardPower;
				}
			}
		}

		public static double BuggyMass { get { return game.BuggyMass; } }
		public static double JeepMass { get { return game.JeepMass; } }

		public static double GetMass(CarType carType) {
			if (carType == CarType.Buggy) {
				return BuggyMass;
			} else {
				return JeepMass;
			}
		}

		public static double GetAcceleration(CarType type, double enginePower) {
			return GetEnginePower(type, ((enginePower > 0)?AxisDirection.up:AxisDirection.down)) / GetMass(type) * enginePower;
		}

		public static double VehicleLength { get { return game.CarWidth; } }
		public static double VehicleWidth { get { return game.CarHeight; } }

		public static double VehicleMovementAirFriction {
			get {
				return game.CarMovementAirFrictionFactor;
			}
		}

		public static double VehicleRotationAirFriction { get { return game.CarRotationAirFrictionFactor; } }

		public static double VehicleLengthFriction { get { return game.CarLengthwiseMovementFrictionFactor; } }
		public static double VehicleCrossFriction { get { return game.CarCrosswiseMovementFrictionFactor; } }

		public static double MaxEnginePowerChange { get { return game.CarEnginePowerChangePerTick; } }
		public static double MaxSteeringAngleChange { get { return game.CarWheelTurnChangePerTick; } }

		public static double VehicleAngularSpeedFactor { get { return game.CarAngularSpeedFactor; } }

		public static void SetConstants(Game _game, World _world) {
			game = _game;
			world = _world;
		}
	}
}

