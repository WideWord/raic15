using System.Collections.Generic;
using System;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
    public class RoadMap {
		
		public int width { get; private set; }
		public int height { get; private set; }

		private Tile[,] tiles;

		public RoadMap(int width, int height) {
			this.width = width;
			this.height = height;

			tiles = new Tile[this.width, this.height];
			for (int x = 0; x < width; ++x) {
				for (int y = 0; y < height; ++y) {
					tiles[x, y] = new Tile(this, x, y); 
				}
			}
		}

		public void updateMap(TileType[,] map) {
			for (int x = 0; x < width; ++x) {
				for (int y = 0; y < height; ++y) {
					tiles[x, y].type = map[x, y];
				}
			}
		}

		public Tile tileAt(int x, int y) {
			if (x < 0 || x >= width)
				return null;
			if (y < 0 || y >= height)
				return null;

			return tiles[x, y];
		}

		public LinkedList<Tile> findPathBetween(Tile from, Tile to) {

			var cost = new double[width, height];
			for (int x = 0; x < width; ++x) {
				for (int y = 0; y < height; ++y) {
					cost[x, y] = double.MaxValue;
				}
			}

			var backDir = new AxisDirection[width, height];

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

			var list = new LinkedList<Tile>();

			{
				var current = to;

				while (current != from) {
					list.AddFirst(current);
					current = current.nextTileInDirection(backDir[current.posX, current.posY]);
				}

				list.AddFirst(from);
			}

			return list;
		}

    }
}
