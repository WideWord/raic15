using System;
using System.Collections.Generic;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{

    public sealed class MyStrategy : IStrategy {

		private ManagedVehicle currentVehicle;

		private static LinkedList<PathUtil.TilePathNode> path; 

		private static RoadMap map;

        public void Move(Car self, World world, Game game, Move move) {
			Debug.beginPost();
			Constants.setConstants(game, world);

			if (currentVehicle == null) {
				map = new RoadMap(world.Width, world.Height);
				map.updateMap(world.TilesXY);

				path = PathUtil.findPathFromWaypoints(world.Waypoints, map, new Vector(self.X, self.Y));

				currentVehicle = new ManagedVehicle(path);
			}

			currentVehicle.setCar(self);

			currentVehicle.tick(move);

			Debug.endPost();
        }
    }


}