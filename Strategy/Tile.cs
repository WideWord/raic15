using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	public class Tile {
		public TileType Type = TileType.Unknown;
		public RoadMap RoadMap { get; private set; }

		public int posX { get; private set; }
		public int posY { get; private set; }

		public Vector Center {
			get {
				return new Vector (
					posX * Constants.TileSize + Constants.TileSize * 0.5,
					posY * Constants.TileSize + Constants.TileSize * 0.5
				);
			}
		}

		public Rect Rect {
			get {
				var half = new Vector(Constants.HalfTileSize, Constants.HalfTileSize);
				return new Rect(
					Center - half,
					Center + half
				);
			}
		}

		public Tile(RoadMap roadMap, int posX, int posY) {
			this.posX = posX;
			this.posY = posY;
			this.RoadMap = roadMap;
		}

		public bool CanGoInDirection(AxisDirection dir) {
			switch (dir) {
			case AxisDirection.Down:
				switch (Type) {
				case TileType.TopHeadedT:
				case TileType.Empty:
				case TileType.Horizontal:
				case TileType.LeftBottomCorner:
				case TileType.RightBottomCorner:
				case TileType.Unknown:
					return false;
				default:
					return true;
				}
			case AxisDirection.Up:
				switch (Type) {
				case TileType.BottomHeadedT:
				case TileType.Empty:
				case TileType.Unknown:
				case TileType.Horizontal:
				case TileType.LeftTopCorner:
				case TileType.RightTopCorner:
					return false;
				default:
					return true;
				}

			case AxisDirection.Left:
				switch (Type) {
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
			case AxisDirection.Right:
				switch (Type) {
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
			}

			return false;
		}

		public Tile NextTileInDirection(AxisDirection dir) {
			switch (dir) {
			case AxisDirection.Up:
				return RoadMap.TileAt(posX, posY - 1);
			case AxisDirection.Down:
				return RoadMap.TileAt(posX, posY + 1);
			case AxisDirection.Left:
				return RoadMap.TileAt(posX - 1, posY);
			case AxisDirection.Right:
				return RoadMap.TileAt(posX + 1, posY);
			}
			return null;
		}

		public AxisDirection? DirectionForTile(Tile tile) {
			if (tile.posX == posX) {
				if (tile.posY > posY)
					return AxisDirection.Down;
				else if (tile.posY < posY)
					return AxisDirection.Up;
				else
					return null;
			} else if (tile.posY == posY) {
				if (tile.posX > posX)
					return AxisDirection.Right;
				else if (tile.posX < posX)
					return AxisDirection.Left;
				else
					return null;
			} else
				return null;
		}

		void GetEdges(LinkedList<Ray> rays, LinkedList<Arc> arcs) {

			foreach (AxisDirection dir in Enum.GetValues(typeof(AxisDirection))) {
				if (!CanGoInDirection(dir)) {

					var sideCenter = Center + new Vector(dir) * (Constants.TileSize * 0.5 - Constants.RoadMargin);
					var sideHalfLength = (Constants.TileSize * 0.5 - Constants.RoadMargin * 2);
					var side = Ray.Line(
						sideCenter + new Vector(dir.TurnLeft()) * sideHalfLength, 
						sideCenter + new Vector(dir.TurnRight()) * sideHalfLength
					);

					rays.AddLast(side);
				}

				var nextDir = dir.TurnLeft();

				if (!CanGoInDirection(dir) && !CanGoInDirection(nextDir)) {
					var arcCenter = Center + (new Vector(dir) + new Vector(nextDir)) * (Constants.TileSize * 0.5 - Constants.RoadMargin * 2);

					var arc = new Arc(arcCenter, Constants.RoadMargin, nextDir.Angle(), dir.Angle());

					arcs.AddLast(arc);

				} else if (CanGoInDirection(dir) && CanGoInDirection(nextDir)) {

					var arcCenter = Center + (new Vector(dir) + new Vector(nextDir)) * (Constants.TileSize * 0.5);

					var arc = new Arc(arcCenter, Constants.RoadMargin, nextDir.Back().Angle(), dir.Back().Angle());

					arcs.AddLast(arc);

				} else if (!CanGoInDirection(dir) && CanGoInDirection(nextDir)) {

					var lineFrom = Center + (new Vector(dir) + new Vector(nextDir)) * (Constants.TileSize * 0.5);
					lineFrom = lineFrom - (new Vector(dir) * Constants.RoadMargin);

					var side = new Ray(lineFrom, new Vector(nextDir.Back()) * Constants.RoadMargin * 2);

					rays.AddLast(side);

				} else if (CanGoInDirection(dir) && !CanGoInDirection(nextDir)) {

					var lineFrom = Center + (new Vector(dir) + new Vector(nextDir)) * (Constants.TileSize * 0.5);
					lineFrom = lineFrom - (new Vector(nextDir) * Constants.RoadMargin);

					var side = new Ray(lineFrom, new Vector(dir.Back()) * Constants.RoadMargin * 2);

					rays.AddLast(side);
				}
			}

		}

		public bool IsIntersect(Ray ray) {
			var rays = new LinkedList<Ray>();
			var arcs = new LinkedList<Arc>();

			GetEdges(rays, arcs);

			foreach (var side in rays) {

				var sideIntersection = ray.Intersect(side);

				if (sideIntersection != null) {
					return true;
				}
			}

			foreach (var arc in arcs) {

				var arcIntersections = arc.MultiIntersect(ray);

				if (arcIntersections != null) {
					return true;
				}

			}

			return false;

		}

		public LinkedList<Vector> MultiIntersect(Ray ray) {

			var intersections = new LinkedList<Vector>();

			var rays = new LinkedList<Ray>();
			var arcs = new LinkedList<Arc>();

			GetEdges(rays, arcs);

			foreach (var side in rays) {
				
				var sideIntersection = ray.Intersect(side);

				if (sideIntersection != null) {
					intersections.AddLast(sideIntersection ?? new Vector());
				}
			}

			foreach (var arc in arcs) {

				var arcIntersections = arc.MultiIntersect(ray);

				if (arcIntersections != null) {
					foreach (var inters in arcIntersections) {
						intersections.AddLast(inters);
					}
				}

			}

			return intersections;
		}

		public Vector? Intersect(Ray ray) {
			var intersections = MultiIntersect(ray);

			if (intersections.Count == 0)
				return null;

			Vector? vec = null;
			var dist = double.MaxValue;

			foreach (var intersection in intersections) {
				var curDist = (intersection - ray.Position).Length;
				if (curDist < dist) {
					dist = curDist;
					vec = intersection;
				}
			}

			return vec;
		}

		public bool IsIntersect(FreeRect rect) {
			
			var rays = new LinkedList<Ray>();
			var arcs = new LinkedList<Arc>();

			GetEdges(rays, arcs);

			foreach (var side in rays) {

				if (rect.IsIntersect(side))
					return true;
			}

			foreach (var arc in arcs) {
				if (rect.IsIntersect(arc))
					return true;
			}

			return false;
		}

		public void Draw(Color color) {
			Rect.Draw(color);
		}
	}

}
