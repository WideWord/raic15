using System;
using System.Collections.Generic;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{

    public sealed class MyStrategy : IStrategy {

		private Vehicle vehicle = new Vehicle();
		private LinkedList<PathUtil.PathNode> path; 

		private RoadMap map;

        public void Move(Car self, World world, Game game, Move move) {
			Debug.beginPost();
			Constants.setConstants(game, world);
			vehicle.setCar(self);

			if (map == null) {
				map = new RoadMap(world.Width, world.Height);
				map.updateMap(world.TilesXY);
			}

			if (path == null) {
				path = PathUtil.buildSmoothPath(PathUtil.findPathFromWaypoints(world.Waypoints, map, vehicle.position));
			}
			PathUtil.drawPath(path, 0xFF0000);

			Debug.endPost();
        }
    }


}