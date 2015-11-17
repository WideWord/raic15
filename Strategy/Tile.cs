using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;


namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	public class Tile {
		public TileType type = TileType.Unknown;
		private RoadMap roadMap;

		public int posX { get; private set; }
		public int posY { get; private set; }

		public Tile(RoadMap roadMap, int posX, int posY) {
			this.posX = posX;
			this.posY = posY;
			this.roadMap = roadMap;
		}

		public bool canGoInDirection(AxisDirection dir) {
			switch (dir) {
			case AxisDirection.down:
				switch (type) {
				case TileType.BottomHeadedT:
				case TileType.Empty:
				case TileType.Horizontal:
				case TileType.LeftTopCorner:
				case TileType.RightTopCorner:
				case TileType.Unknown:
					return false;
				default:
					return true;
				}
			case AxisDirection.up:
				switch (type) {
				case TileType.TopHeadedT:
				case TileType.Empty:
				case TileType.Unknown:
				case TileType.Horizontal:
				case TileType.LeftBottomCorner:
				case TileType.RightBottomCorner:
					return false;
				default:
					return true;
				}

			case AxisDirection.left:
				switch (type) {
				case TileType.Empty:
				case TileType.Unknown:
				case TileType.LeftHeadedT:
				case TileType.Vertical:
				case TileType.RightTopCorner:
				case TileType.RightBottomCorner:
					return false;
				default:
					return true;
				}
			case AxisDirection.right:
				switch (type) {
				case TileType.Empty:
				case TileType.Unknown:
				case TileType.RightHeadedT:
				case TileType.Vertical:
				case TileType.LeftTopCorner:
				case TileType.LeftBottomCorner:
					return false;
				default:
					return true;
				}
			}

			return false;
		}

		public Tile nextTileInDirection(AxisDirection dir) {
			switch (dir) {
			case AxisDirection.up:
				return roadMap.tileAt(posX, posY - 1);
			case AxisDirection.down:
				return roadMap.tileAt(posX, posY + 1);
			case AxisDirection.left:
				return roadMap.tileAt(posX - 1, posY);
			case AxisDirection.right:
				return roadMap.tileAt(posX + 1, posY);
			}
			return null;
		}

	}

}
