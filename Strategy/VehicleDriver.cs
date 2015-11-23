using System;
using System.Collections.Generic;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class VehicleDriver {

		private LinkedList<VehicleDriverStrategy> strategies = new LinkedList<VehicleDriverStrategy>();

		public VehicleDriver() {
			strategies.AddLast(new LineRoadStrategy());
		}

		public void drive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move) {

			foreach (var strategy in strategies) {
				if (strategy.tryDrive(vehicle, tilePath, move)) {
					return;
				}
			}

			//throw new Exception("Do not have strategy for this situation");
		}
	}

	public interface VehicleDriverStrategy {

		bool tryDrive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move);

	}


	//
	// [ ]
	// [ ]
	// [ ]
	// [*]
	//
	public class LineRoadStrategy : VehicleDriverStrategy {

		public bool tryDrive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move) {

			AxisDirection dir;

			{
				var current = MyStrategy.map.tileAt(vehicle.position);

				var node = tilePath.First;
				if (node == null)
					return false;

				dir = current.directionForTile(node.Value.tile) ?? AxisDirection.up;

				for (int i = 0; i < 2 && node != null; ++i) {

					var next = node.Next;

					if (next == null) {
						break;
					}

					if (node.Value.tile.directionForTile(next.Value.tile) != dir)
						return false;

					node = next;
                }
			}

			var dirVec = new Vector(dir);

			if (vehicle.forward * dirVec < 0.3) {

				var power = 1.0;
				if (vehicle.speed.length > 12) {
					power = 0.1;
				}

				if (MyStrategy.map.intersect(-vehicle.forwardRay * Constants.vehicleLength * 1.5) == null) {
					move.EnginePower = power;
					move.WheelTurn = vehicle.forward.angleTo(dirVec) * 100;
				} else {
					move.EnginePower = -power;
					move.WheelTurn = -vehicle.forward.angleTo(dirVec) * 100;
				}

			} else {
				move.EnginePower = 1;
				move.WheelTurn = vehicle.forward.angleTo(dirVec) * 4;
			}

			return true;

		}

	}

	//
	//   [ ][ ][ ]
	//   [*]
	//
	//
	public class JustBeforeTurningToLineStrategy : VehicleDriverStrategy {

		public bool tryDrive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move) {

			AxisDirection dir;
			var currentTile = MyStrategy.map.tileAt(vehicle.position);

			Vector turningTo;
			Vector turningFrom;

			{
				var node = tilePath.First;

				var first = node.Value.tile;

				node = node.Next;

				var second = node.Value.tile;

				node = node.Next;

				var third = node.Value.tile;

				var firstToSecond = first.directionForTile(second).Value;

				turningTo = new Vector(firstToSecond);

				if (firstToSecond != second.directionForTile(third)) {
					return false;
				}

				var currentToFirst = currentTile.directionForTile(first).Value;
				if (currentToFirst.isSameAxis(firstToSecond)) {
					return false;
				}

				turningFrom = new Vector(currentToFirst);

			}

			if (turningFrom + turningTo)

		}

	}
}

