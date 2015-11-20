using System;
using System.Collections.Generic;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{

    public sealed class MyStrategy : IStrategy {

		private ManagedVehicle currentVehicle;

		public static RoadMap map { get; private set; }
		public static int[][] waypoints;

        public void Move(Car self, World world, Game game, Move move) {
			Debug.beginPost();
			Constants.setConstants(game, world);

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