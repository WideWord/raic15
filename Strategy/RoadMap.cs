using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
    class RoadMap {
		
        public class Tile {
            public TileType type = TileType.Unknown;
			private RoadMap roadMap;

			public int posX { get; private set; }
			public int posY { get; private set; }

			Tile(RoadMap roadMap, int posX, int posY) {
				this.posX = posX;
				this.posY = posY;
				this.roadMap = roadMap;
			}

			bool canGoInDirection(AxisDirection dir) {
				
			}

        }

		public int width { get; private set; }
		public int height { get; private set; }

		private Tile[,] tiles;

		RoadMap(int width, int height) {
			this.width = width;
			this.height = height;

			tiles = new Tile[this.width, this.height];
			for (int x = 0; x < width; ++x) {
				for (int y = 0; y < height; ++y) {
					tiles[x, y] = new Tile(x, y); 
				}
			}
		}

		void updateMap(TileType[,] map) {
			for (int x = 0; x < width; ++x) {
				for (int y = 0; y < height; ++y) {
					tiles[x, y].type = map[x, y];
				}
			}
		}

		Tile tileAt(int x, int y) {
			return tiles[x, y];
		}

    }
}
