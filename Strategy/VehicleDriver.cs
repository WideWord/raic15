using System;
using System.Collections.Generic;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class VehicleDriver {

		private LinkedList<VehicleDriverStrategy> strategies = new LinkedList<VehicleDriverStrategy>();

		public VehicleDriver() {

			strategies.AddLast(new GetBackStrategy());

			strategies.AddLast(new LineRoadStrategy());
			strategies.AddLast(new JustBeforeTurningToLineStrategy());
			strategies.AddLast(new FromLineToTurn());

			strategies.AddLast(new BackupStrategy());
		}

		public void drive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move) {

			foreach (var strategy in strategies) {
				if (strategy.tryDrive(vehicle, tilePath, move)) {

					Debug.print(vehicle.position + new Vector(15, 15), strategy.GetType().Name);

					return;
				}
			}

			//throw new Exception("Do not have strategy for this situation");
		}
	}

	public interface VehicleDriverStrategy {

		bool tryDrive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move);

	}

	public class GetBackStrategy : VehicleDriverStrategy {

		public bool tryDrive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move) {

			if (vehicle.speed.length > 5)
				return false;

			var target = tilePath.First.Value.tile.center;

			if (MyStrategy.map.intersect(vehicle.forwardRay * Constants.vehicleLength * 2) != null) {
				move.IsBrake = false;
				move.EnginePower = -1;
				move.WheelTurn = -vehicle.forward.angleTo(target - vehicle.position) * 4;
				return true;
			}

			return false;
		}

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
			move.IsBrake = false;

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

			if (vehicle.speed.length > 20) {
				move.IsBrake = true;
				move.EnginePower = -1;
			} else {
				move.IsBrake = false;
				move.EnginePower = 1;
			}

			var turn = turningTo + turningFrom;
			move.WheelTurn = vehicle.forward.angleTo(turn) * 2;

			return true;

		}

	}

	//
	//
	// [ ][ ]
	// [ ]
	// [*]
	//
	public class FromLineToTurn : VehicleDriverStrategy {

		public bool tryDrive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move) {

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

				var currentToFirst = currentTile.directionForTile(first).Value;
				var firstToSecond = first.directionForTile(second).Value;
				var secondToThird = second.directionForTile(third).Value;

				if (currentToFirst != firstToSecond)
					return false;

				if (firstToSecond.isSameAxis(secondToThird))
					return false;

				turningFrom = new Vector(currentToFirst);
				turningTo = new Vector(secondToThird);
			}

			var turningAngle = turningFrom.angleTo(turningTo);

			var target = tilePath.First.Value.tile.center - turningTo * Constants.tileSize * 0.35;


			if (vehicle.forward * turningFrom > 0) {
				if (vehicle.speed.length > 20) {

					if (vehicle.speed * vehicle.forward > 0.3) {

						target.draw(0x00FFFF);

						move.WheelTurn = vehicle.forward.angleTo(target - vehicle.position) * 4;
						move.IsBrake = false;
						move.EnginePower = 1;
						return true;

					} else {
						move.IsBrake = true;

						if (vehicle.speed * vehicle.forward < 0) {
							move.WheelTurn = -vehicle.forward.angleTo(target - vehicle.position) * 4;
						} else {
							move.WheelTurn = vehicle.forward.angleTo(target - vehicle.position) * 4;
						}
						move.EnginePower = 1;
						return true;
					}
				} else {
					if (vehicle.speed * vehicle.forward < 0) {
						move.WheelTurn = -vehicle.forward.angleTo(target - vehicle.position) * 4;
						move.IsBrake = true;
					} else {
						move.WheelTurn = vehicle.forward.angleTo(target - vehicle.position) * 4;
						move.IsBrake = false;
					}
					move.EnginePower = 1;
					return true;
				}
			} else {
				if (vehicle.speed.length > 10) {
					move.EnginePower = 0;
					move.IsBrake = true;
					if (vehicle.speed * vehicle.forward < 0) {
						move.WheelTurn = -vehicle.forward.angleTo(target - vehicle.position) * 4;
					} else {
						move.WheelTurn = vehicle.forward.angleTo(target - vehicle.position) * 4;
					}
				} else {
					if (vehicle.speed * vehicle.forward < 0) {
						move.WheelTurn = -vehicle.forward.angleTo(target - vehicle.position) * 4;
					} else {
						move.WheelTurn = vehicle.forward.angleTo(target - vehicle.position) * 4;
					}
					move.EnginePower = 1;
				}
			}

			return true;

		}

	}

	public class BackupStrategy : VehicleDriverStrategy {

		public bool tryDrive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move) {

			var nextTile = tilePath.First.Value.tile;
			nextTile.draw(0xFF0000);
			nextTile.center.draw(0xFF1010);

			if (vehicle.forward * (nextTile.center - vehicle.position) < 0.4 && vehicle.speed.length > 10) {
				move.IsBrake = true;
				move.EnginePower = -1;
			} else {
				move.IsBrake = false;
				move.EnginePower = 1;
			}

			move.WheelTurn = vehicle.forward.angleTo((nextTile.center - vehicle.position)) * 4;


			return true;

		}

	}
}

