using System;
using System.Collections.Generic;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{

    public sealed class MyStrategy : IStrategy {

		private ManagedVehicle currentVehicle;

		public static RoadMap map { get; private set; }
		public static int[][] waypoints;
		public static Tile tileAtWaypoint(int index) {
			index = index % waypoints.Length;
			return map.tileAt(waypoints[index][0], waypoints[index][1]);
		}

		public static int currentTick { get; private set; }


		public void Move(Car self, World world, Game game, Move move) {
			Debug.beginPost();
			Constants.setConstants(game, world);
			currentTick = world.Tick;


			if (map == null) {
				waypoints = world.Waypoints;
				map = new RoadMap(world.Width, world.Height);
				map.updateMap(world.TilesXY);
			}

			if (currentVehicle == null) {
				currentVehicle = new ManagedVehicle();
			}

			currentVehicle.setCar(self);

			currentVehicle.tick(move);

			Debug.endPost();
        }
    }


}