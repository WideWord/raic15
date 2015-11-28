using System;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
    public class RoadMap {
		
		public int Width { get; private set; }
		public int Height { get; private set; }

		private Tile[,] tiles;

		public RoadMap(int width, int height) {
			Width = width;
			Height = height;

			tiles = new Tile[this.Width, this.Height];
			for (int x = 0; x < width; ++x) {
				for (int y = 0; y < height; ++y) {
					tiles[x, y] = new Tile(this, x, y); 
				}
			}
		}

		public void UpdateMap(TileType[][] map) {
			for (int x = 0; x < Width; ++x) {
				for (int y = 0; y < Height; ++y) {
					tiles[x, y].Type = map[x][y];
				}
			}
		}

		public Tile TileAt(int x, int y) {
			if (x < 0 || x >= Width)
				return null;
			if (y < 0 || y >= Height)
				return null;

			return tiles[x, y];
		}

		public Tile TileAt(Vector position) {
			int x = (int)Math.Floor(position.x / Constants.TileSize);
			int y = (int)Math.Floor(position.y / Constants.TileSize);
			return TileAt(x, y);
		}

		public Vector? Intersect(Ray ray, bool debugDraw = false) {

			var tile = TileAt(ray.Position);
			AxisDirection? lastDir = null;

			while (true) {
				

				var intersection = tile.Intersect(ray);

				if (intersection != null) {
					return intersection;
				}
					

				var dir = tile.Rect.BorderAnyIntersectionDirection(ray, (lastDir == null)?(null):(lastDir.Value.Back() as AxisDirection?));

				if (dir == null)
					return null;

				tile = tile.NextTileInDirection(dir ?? AxisDirection.Up);
				if (tile == null)
					return null;

				lastDir = dir;

			}
		}

    }
}
