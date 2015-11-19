using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	public class Tile {
		public TileType type = TileType.Unknown;
		public RoadMap roadMap { get; private set; }

		public int posX { get; private set; }
		public int posY { get; private set; }

		public Vector center {
			get {
				return new Vector (
					posX * Constants.tileSize + Constants.tileSize * 0.5,
					posY * Constants.tileSize + Constants.tileSize * 0.5
				);
			}
		}

		public Rect rect {
			get {
				var half = new Vector(Constants.tileSize / 2, Constants.tileSize / 2);
				return new Rect(
					center - half,
					center + half
				);
			}
		}

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
				case TileType.LeftBottomCorner:
				case TileType.RightBottomCorner:
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
				case TileType.LeftTopCorner:
				case TileType.RightTopCorner:
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
				case TileType.LeftTopCorner:
				case TileType.LeftBottomCorner:
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
				case TileType.RightTopCorner:
				case TileType.RightBottomCorner:
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

		public AxisDirection? directionForTile(Tile tile) {
			if (tile.posX == posX) {
				if (tile.posY > posY)
					return AxisDirection.down;
				else if (tile.posY < posY)
					return AxisDirection.up;
				else
					return null;
			} else if (tile.posY == posY) {
				if (tile.posX > posX)
					return AxisDirection.right;
				else if (tile.posX < posX)
					return AxisDirection.left;
				else
					return null;
			} else
				return null;
		}

		public LinkedList<Vector> multiIntersect(Ray ray, bool debugDrawWalls = false) {

			var intersections = new LinkedList<Vector>();

			foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				if (!canGoInDirection(dir)) {

					var sideCenter = center + new Vector(dir) * (Constants.tileSize * 0.5 - Constants.roadMargin);
					var sideHalfLength = (Constants.tileSize * 0.5 - Constants.roadMargin * 2);
					var side = Ray.line(
						sideCenter + new Vector(dir.turnLeft()) * sideHalfLength, 
						sideCenter + new Vector(dir.turnRight()) * sideHalfLength
					);
						
					var sideIntersection = ray.intersect(side);
					if (debugDrawWalls)
						side.draw(0x00FF00);

					if (sideIntersection != null) {
						intersections.AddLast(sideIntersection ?? new Vector());
					}
				}

				var nextDir = dir.turnLeft();

				if (!canGoInDirection(dir) && !canGoInDirection(nextDir)) {
					var arcCenter = center + (new Vector(dir) + new Vector(nextDir)) * (Constants.tileSize * 0.5 - Constants.roadMargin * 2);

					var arc = new Arc(arcCenter, Constants.roadMargin, nextDir.angle(), dir.angle());

					if (debugDrawWalls)
						arc.draw(0x00FF00);

					var arcIntersections = arc.multiIntersect(ray);

					if (arcIntersections != null) {
						foreach (var inters in arcIntersections) {
							intersections.AddLast(inters);
						}
					}
				} else if (canGoInDirection(dir) && canGoInDirection(nextDir)) {

					var arcCenter = center + (new Vector(dir) + new Vector(nextDir)) * (Constants.tileSize * 0.5);

					var arc = new Arc(arcCenter, Constants.roadMargin, nextDir.back().angle(), dir.back().angle());
					if (debugDrawWalls)
						arc.draw(0x00FF00);

					var arcIntersections = arc.multiIntersect(ray);

					if (arcIntersections != null) {
						foreach (var inters in arcIntersections) {
							intersections.AddLast(inters);
						}
					}
				} else if (!canGoInDirection(dir) && canGoInDirection(nextDir)) {

					var lineFrom = center + (new Vector(dir) + new Vector(nextDir)) * (Constants.tileSize * 0.5);
					lineFrom = lineFrom - (new Vector(dir) * Constants.roadMargin);

					var side = new Ray(lineFrom, new Vector(nextDir.back()) * Constants.roadMargin * 2);

					if (debugDrawWalls)
						side.draw(0x00FF00);

					var sideIntersection = ray.intersect(side);

					if (sideIntersection != null) {
						intersections.AddLast(sideIntersection ?? new Vector());
					}
				} else if (canGoInDirection(dir) && !canGoInDirection(nextDir)) {

					var lineFrom = center + (new Vector(dir) + new Vector(nextDir)) * (Constants.tileSize * 0.5);
					lineFrom = lineFrom - (new Vector(nextDir) * Constants.roadMargin);

					var side = new Ray(lineFrom, new Vector(dir.back()) * Constants.roadMargin * 2);

					if (debugDrawWalls)
						side.draw(0x00FF00);

					var sideIntersection = ray.intersect(side);

					if (sideIntersection != null) {
						intersections.AddLast(sideIntersection ?? new Vector());
					}
				}
			}

			return intersections;
		}

		public Vector? intersect(Ray ray) {
			var intersections = multiIntersect(ray);

			if (intersections.Count == 0)
				return null;

			Vector? vec = null;
			var dist = double.MaxValue;

			foreach (var intersection in intersections) {
				var curDist = (intersection - ray.position).length;
				if (curDist < dist) {
					dist = curDist;
					vec = intersection;
				}
			}

			return vec;
		}

		public void draw(int color) {
			Debug.rect(rect.min, rect.max, color);
		}
	}

}
