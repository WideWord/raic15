using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class TilePath {

		public List<Tile> tilePath { get; private set; }

		public TilePath(int[][] waypoints, RoadMap roadMap, Vehicle vehicle, int skipWaypoints = 1) {

			tilePath = new List<Tile>();

			var from = roadMap.tileAt(vehicle.position);

			var dir = vehicle.forward;

			int waypointsCount = waypoints.Length;

			for (int waypointIndex = skipWaypoints, end = waypointsCount * 2 + 1; waypointIndex < end && tilePath.Count < 10; ++waypointIndex) {


				Tile to = roadMap.tileAt(waypoints[waypointIndex % waypointsCount][0], waypoints[waypointIndex % waypointsCount][1]);

				dir = addPathBetween(from, to, dir);

				from = to;

			}

		}
			
		private Vector addPathBetween(Tile from, Tile to, Vector speedDir) {

			var q = new Queue<Tile>();
			var map = from.roadMap;

			var cost = new double[map.width, map.height];
			for (int x = 0; x < map.width; ++x) {
				for (int y = 0; y < map.height; ++y) {
					cost[x, y] = double.MaxValue;
				}
			}

			cost[from.posX, from.posY] = 0;

			var backDir = new AxisDirection[map.width, map.height];

			{
				Vector speed = speedDir;

				foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
					if (!from.canGoInDirection(dir))
						continue;

					var next = from.nextTileInDirection(dir);

					if (next == null)
						continue;

					cost[next.posX, next.posY] = (speed * new Vector(dir)) * 0.5 + 1;
					backDir[next.posX, next.posY] = dir.back();
					q.Enqueue(next);
				}
			}

			while (q.Count > 0) {
				var cur = q.Dequeue();

				Vector speed = new Vector(backDir[cur.posX, cur.posY].back());
				{
					var prev = cur.nextTileInDirection(backDir[cur.posX, cur.posY]);
					if (prev == from) {
						speed += speedDir;
					} else {
						speed += new Vector(backDir[prev.posX, prev.posY].back());
					}
				}
				speed = speed.normalized;

				foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
					if (!cur.canGoInDirection(dir))
						continue;

					var next = cur.nextTileInDirection(dir);

					if (next == null)
						continue;

					var nextCost = cost[cur.posX, cur.posY] + (speed * new Vector(dir)) * -0.7   + 1;

					if (nextCost < cost[next.posX, next.posY]) {
						cost[next.posX, next.posY] = nextCost;
						backDir[next.posX, next.posY] = dir.back();
						q.Enqueue(next);
					}
				}
			}



			{
				var localPath = new LinkedList<Tile>();

				var last = to;

				while (last != from && last != null) {
					localPath.AddFirst(last);

					last = last.nextTileInDirection(backDir[last.posX, last.posY]);
				}

				foreach (var tile in localPath) {
					tilePath.Add(tile);
				}

			}

			Vector retSpeed = new Vector(backDir[to.posX, to.posY].back());
			{
				var prev = to.nextTileInDirection(backDir[to.posX, to.posY]);
				if (prev == from) {
					retSpeed += speedDir;
				} else if (prev != null) {
					retSpeed += new Vector(backDir[prev.posX, prev.posY].back());
				} else {
					retSpeed += speedDir;
				}

				retSpeed = retSpeed.normalized;
			}

			return retSpeed;
		}
	

		public void draw(int color) {
			Tile last = null;

			foreach (var tile in tilePath) {
				if (last != null) {
					Debug.line(last.center, last.center + (tile.center - last.center) * 0.4, color);
					Debug.line(last.center + (tile.center - last.center) * 0.4, last.center + (tile.center - last.center) * 0.4 - (tile.center - last.center).rotate(0.2) * 0.1, color);
					Debug.line(last.center + (tile.center - last.center) * 0.4, last.center + (tile.center - last.center) * 0.4 - (tile.center - last.center).rotate(-0.2) * 0.1, color);
				}
				last = tile;
			}
		}

	}
}

