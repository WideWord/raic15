using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	public class DistanceMap {

		private double[,] distances;

		public DistanceMap(Tile target) {

			var map = target.RoadMap;

			distances = new double[map.Width, map.Height];

			for (int x = 0; x < map.Width; ++x) {
				for (int y = 0; y < map.Height; ++y) {
					distances[x, y] = double.PositiveInfinity;
				}
			}

			{
				var q = new Queue<Tile>();
				distances[target.posX, target.posY] = 0;

				q.Enqueue(target);

				while (q.Count > 0) {
					var tile = q.Dequeue();

					foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
						if (!tile.CanGoInDirection(dir))
							continue;

						var next = tile.NextTileInDirection(dir);

						if (next == null)
							continue;

						var nextDist = distances[tile.posX, tile.posY] + 1;

						if (distances[next.posX, next.posY] > nextDist) {
							distances[next.posX, next.posY] = nextDist;
							q.Enqueue(next);
						}
					}
				}
			}

		}

		public double DistanceFor(Tile tile) {
			return distances[tile.posX, tile.posY];
		}

	}
}
