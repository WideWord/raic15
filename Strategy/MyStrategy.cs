using System;
using System.Collections.Generic;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{

    public sealed class MyStrategy : IStrategy {

		private ManagedVehicle currentVehicle;

		public static RoadMap Map { get; private set; }
		public static int[][] Waypoints;
		public static Tile TileAtWaypoint(int index) {
			index = index % Waypoints.Length;
			return Map.TileAt(Waypoints[index][0], Waypoints[index][1]);
		}

		public static int CurrentTick { get; private set; }


		public void Move(Car self, World world, Game game, Move move) {
			Constants.SetConstants(game, world);
			CurrentTick = world.Tick;


			if (Map == null) {
				Waypoints = world.Waypoints;
				Map = new RoadMap(world.Width, world.Height);
				Map.UpdateMap(world.TilesXY);
			}

			if (currentVehicle == null) {
				currentVehicle = new ManagedVehicle();
			}

			currentVehicle.SetCar(self);

			currentVehicle.Tick(move);

			Debug.Flush();
        }
    }


}