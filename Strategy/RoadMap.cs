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

		public void updateMap(TileType[][] map) {
			for (int x = 0; x < width; ++x) {
				for (int y = 0; y < height; ++y) {
					tiles[x, y].type = map[x][y];
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

		public Tile tileAt(Vector position) {
			int x = (int)Math.Floor(position.x / Constants.tileSize);
			int y = (int)Math.Floor(position.y / Constants.tileSize);
			return tileAt(x, y);
		}

		public Vector? intersect(Ray ray, bool debugDraw = false) {

			var tile = tileAt(ray.position);
			AxisDirection? lastDir = null;

			while (true) {

				if (debugDraw)
					tile.draw(0x00FF00);

				var intersection = tile.intersect(ray);

				if (intersection != null) {
					//zintersection?.draw(0xFF0000);
					return intersection;
				}

				var dir = tile.rect.borderAnyIntersectionDirection(ray, lastDir?.back());

				if (dir == null)
					return null;

				tile = tile.nextTileInDirection(dir ?? AxisDirection.up);
				if (tile == null)
					return null;

				lastDir = dir;

			}
		}

		public bool isIntersectRoughly(Ray ray) {
			var tile = tileAt(ray.position);
			AxisDirection? lastDir = null;

			while (true) {

				var dir = tile.rect.borderAnyIntersectionDirection(ray, lastDir?.back());



				if (dir == null)
					return false;
				else if (!tile.canGoInDirection(dir.Value)) {
					return true;
				}

				tile = tile.nextTileInDirection(dir.Value);
				if (tile == null)
					return false;

				lastDir = dir;
			}
		}

    }
}
