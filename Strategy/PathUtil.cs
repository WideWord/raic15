using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {


	public struct PathUtil {

		public struct TilePathNode {
			public int waypointIndex;
			public Tile tile;
			public TilePathNode(Tile _tile) {
				tile = _tile;
				waypointIndex = -1;
			}
		}

		public static LinkedList<TilePathNode> findPathBetween(Tile from, Tile to) {

			var roadMap = from.roadMap;

			var cost = new double[roadMap.width, roadMap.height];
			for (int x = 0; x < roadMap.width; ++x) {
				for (int y = 0; y < roadMap.height; ++y) {
					cost[x, y] = double.MaxValue;
				}
			}

			var backDir = new AxisDirection[roadMap.width, roadMap.height];

			var queue = new Queue<Tile>();
			queue.Enqueue(from);

			cost[from.posX, from.posY] = 0;

			while (queue.Count > 0) {
				var current = queue.Dequeue();

				foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
					if (current.canGoInDirection(dir)) {
						var next = current.nextTileInDirection(dir);
						if (next != null) {

							if (cost[next.posX, next.posY] > cost[current.posX, current.posY] + 1) {
								cost[next.posX, next.posY] = cost[current.posX, current.posY] + 1;
								backDir[next.posX, next.posY] = dir.back();

								if (!queue.Contains(next))
									queue.Enqueue(next);
							}
						}
					}
				}

			}

			var list = new LinkedList<TilePathNode>();

			{
				var current = to;

				while (current != from) {
					list.AddFirst(new TilePathNode(current));
					current = current.nextTileInDirection(backDir[current.posX, current.posY]);
				}
			}

			return list;
		}

		public static LinkedList<TilePathNode> findPathFromWaypoints(int[][] waypoints, RoadMap roadMap, Vector startPosition) {

			var path = new LinkedList<TilePathNode>();

			var lastTile = roadMap.tileAt(startPosition);

			for (int i = 1, iend = waypoints.Length; i < iend; ++i) {
				var currentTile = roadMap.tileAt(waypoints[i][0], waypoints[i][1]);

				var mpath = findPathBetween(lastTile, currentTile); 

				foreach (TilePathNode node in mpath) {
					path.AddLast(node);
				}

				var lastNode = path.Last.Value;
				lastNode.waypointIndex = i;
				path.Last.Value = lastNode;

				lastTile = currentTile;
			}

			return path;
		}
			
		public struct PathNode {
			public int waypointIndex;
			public Vector position;
			public PathNode(Vector pos) {
				position = pos;
				waypointIndex = -1;
			}
			public PathNode(Vector pos, int waypointIndex) {
				position = pos;
				this.waypointIndex = waypointIndex;
			}
		}

		public static LinkedList<PathNode> buildSmoothPath(LinkedList<TilePathNode> tilePath) {

			var path = new LinkedList<PathNode>();

			foreach (var tilePathNode in tilePath) {
				path.AddLast(new PathNode(tilePathNode.tile.center, tilePathNode.waypointIndex));
			}

			return path;
		}

		public static void drawPath(LinkedList<PathNode> path, int color) {
			PathNode lastPoint = path.First.Value;

			foreach (var point in path) {
				Debug.fillCircle(point.position, 25, 0xFF0000);
				if (point.waypointIndex != -1) {
					Debug.print(point.position + new Vector (50, 50), "" + point.waypointIndex, 0xFF0000);
				}

				Debug.line(lastPoint.position, point.position, 0xFF0000);
				lastPoint = point;
			}
		}

	}
}

