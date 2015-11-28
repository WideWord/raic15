using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class TilePath {

		public List<Tile> TileList { get; private set; }

		public TilePath(int[][] waypoints, RoadMap roadMap, Vehicle vehicle, int skipWaypoints = 1) {

			TileList = new List<Tile>();

			var from = roadMap.TileAt(vehicle.Position);

			var dir = vehicle.Forward;

			int waypointsCount = waypoints.Length;

			for (int waypointIndex = skipWaypoints, end = waypointsCount * 2 + 1; waypointIndex < end && TileList.Count < 10; ++waypointIndex) {


				Tile to = roadMap.TileAt(waypoints[waypointIndex % waypointsCount][0], waypoints[waypointIndex % waypointsCount][1]);

				dir = AddPathBetween(from, to, dir);

				from = to;

				if (waypointIndex == end - 1) {
					Tile preLast;
					if (TileList.Count >= 2) {
						preLast = TileList[TileList.Count - 2];
					} else {
						preLast = roadMap.TileAt(vehicle.Position);
					}

					Tile last = TileList[TileList.Count - 1];

					var finishDir = preLast.DirectionForTile(last);
					if (finishDir != null) {
						var tile = last;
						for (int i = 0; i < 4; ++i) {
							var next = tile.NextTileInDirection(finishDir.Value);
							if (next == null)
								break;
							TileList.Add(next);
							tile = next;
						}
					}
				}
			}
			

		}
			
		private Vector AddPathBetween(Tile from, Tile to, Vector speedDir) {

			var q = new Queue<Tile>();
			var map = from.RoadMap;

			var cost = new double[map.Width, map.Height];
			for (int x = 0; x < map.Width; ++x) {
				for (int y = 0; y < map.Height; ++y) {
					cost[x, y] = double.MaxValue;
				}
			}

			cost[from.posX, from.posY] = 0;

			var backDir = new AxisDirection[map.Width, map.Height];

			{
				Vector speed = speedDir;

				foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
					if (!from.CanGoInDirection(dir))
						continue;

					var next = from.NextTileInDirection(dir);

					if (next == null)
						continue;

					cost[next.posX, next.posY] = (speed * new Vector(dir)) * 0.5 + 1;
					backDir[next.posX, next.posY] = dir.Back();
					q.Enqueue(next);
				}
			}

			while (q.Count > 0) {
				var cur = q.Dequeue();

				Vector speed = new Vector(backDir[cur.posX, cur.posY].Back());
				{
					var prev = cur.NextTileInDirection(backDir[cur.posX, cur.posY]);
					if (prev == from) {
						speed += speedDir;
					} else {
						speed += new Vector(backDir[prev.posX, prev.posY].Back());
					}
				}
				speed = speed.Normalized;

				foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
					if (!cur.CanGoInDirection(dir))
						continue;

					var next = cur.NextTileInDirection(dir);

					if (next == null)
						continue;

					var nextCost = cost[cur.posX, cur.posY] + (speed * new Vector(dir)) * -0.7   + 1;

					if (nextCost < cost[next.posX, next.posY]) {
						cost[next.posX, next.posY] = nextCost;
						backDir[next.posX, next.posY] = dir.Back();
						q.Enqueue(next);
					}
				}
			}



			{
				var localPath = new LinkedList<Tile>();

				var last = to;

				while (last != from && last != null) {
					localPath.AddFirst(last);

					last = last.NextTileInDirection(backDir[last.posX, last.posY]);
				}

				foreach (var tile in localPath) {
					TileList.Add(tile);
				}

			}

			Vector retSpeed = new Vector(backDir[to.posX, to.posY].Back());
			{
				var prev = to.NextTileInDirection(backDir[to.posX, to.posY]);
				if (prev == from) {
					retSpeed += speedDir;
				} else if (prev != null) {
					retSpeed += new Vector(backDir[prev.posX, prev.posY].Back());
				} else {
					retSpeed += speedDir;
				}

				retSpeed = retSpeed.Normalized;
			}

			return retSpeed;
		}
	

		public void Draw(Color color) {
			Tile last = null;

			foreach (var tile in TileList) {
				if (last != null) {
					Debug.Line(last.Center, last.Center + (tile.Center - last.Center) * 0.4, color);
					Debug.Line(last.Center + (tile.Center - last.Center) * 0.4, last.Center + (tile.Center - last.Center) * 0.4 - (tile.Center - last.Center).Rotate(0.2) * 0.1, color);
					Debug.Line(last.Center + (tile.Center - last.Center) * 0.4, last.Center + (tile.Center - last.Center) * 0.4 - (tile.Center - last.Center).Rotate(-0.2) * 0.1, color);
				}
				last = tile;
			}
		}

	}
}

