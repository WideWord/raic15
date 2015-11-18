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

    }
}
