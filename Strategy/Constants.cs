using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public static class Constants {

		public static readonly double turningSmoothCoef = 0.5;
		public static readonly double roadMargin = 80.0;

		private static Game game;
		private static World world;

		public static double tileSize { get { return game.TrackTileSize; } }

		public static double buggyEngineForwardPower { get { return game.BuggyEngineForwardPower; } }
		public static double jeepEngineForwardPower { get { return game.JeepEngineForwardPower; } }
		public static double buggyEngineRearPower { get { return game.BuggyEngineRearPower; } }
		public static double jeepEngineRearPower { get { return game.JeepEngineRearPower; } }

		public static double getEnginePower(CarType carType, AxisDirection direction) {
			if (carType == CarType.Buggy) {
				if (direction == AxisDirection.up) {
					return buggyEngineForwardPower;
				} else {
					return buggyEngineRearPower;
				}
			} else {
				if (direction == AxisDirection.up) {
					return jeepEngineForwardPower;
				} else {
					return jeepEngineForwardPower;
				}
			}
		}

		public static double buggyMass { get { return game.BuggyMass; } }
		public static double jeepMass { get { return game.JeepMass; } }

		public static double getMass(CarType carType) {
			if (carType == CarType.Buggy) {
				return buggyMass;
			} else {
				return jeepMass;
			}
		}

		public static double getCarAcceleration(CarType type, AxisDirection direction) {
			return getEnginePower(type, direction) / getMass(type);
		}

		public static void setConstants(Game _game, World _world) {
			game = _game;
			world = _world;
		}

	}
}

