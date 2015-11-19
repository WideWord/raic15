using System;
using System.Collections.Generic;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{

    public sealed class MyStrategy : IStrategy {

		private Vehicle vehicle = new Vehicle();
		private PathNavigator navigator;

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
				navigator = new PathNavigator(path);
			}
				
			PathUtil.drawPath(path, 0xFF0000);

			var nextPoint = navigator.nextPoint(vehicle);
			Debug.fillCircle(nextPoint, 25, 0xFFFF00);

			move.EnginePower = 0.7;

			move.WheelTurn = self.GetAngleTo(nextPoint.x, nextPoint.y);

			{
				map.intersect(vehicle.forwardRay * 2000, true);
				(vehicle.forwardRay * 2000).draw(0x0000FF);

			}

			Debug.endPost();
        }
    }


}