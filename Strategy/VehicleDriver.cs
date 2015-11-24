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
			strategies.AddLast(new SmoothDiagonalTurning());

			strategies.AddLast(new BackupStrategy());
		}

		public void drive(Vehicle vehicle, List<Tile> tilePath, Move move) {
			

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

		bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move);

	}

	public class GetBackStrategy : VehicleDriverStrategy {

		public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			if (vehicle.speed.length > 5)
				return false;

			var target = tilePath[0].center;

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

		public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			AxisDirection dir;

			{
				var current = MyStrategy.map.tileAt(vehicle.position);

				dir = current.directionForTile(tilePath[0]) ?? AxisDirection.up;

				for (int i = 1; i < 2 && i < tilePath.Count; ++i) {
					if (tilePath[i - 1].directionForTile(tilePath[i]) != dir)
						return false;
                }
			}

			var dirVec = new Vector(dir);

			if (vehicle.forward * dirVec > 0.3) {
				move.EnginePower = 1;
				move.WheelTurn = vehicle.steeringAngleForDirection(dirVec);
			} else {
				move.EnginePower = 0.2;
				move.WheelTurn = vehicle.steeringAngleForDirection(dirVec);
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

		public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			if (tilePath.Count < 3)
				return false;

			var currentTile = MyStrategy.map.tileAt(vehicle.position);

			Vector turningTo;
			Vector turningFrom;

			{
				var firstToSecond = tilePath[0].directionForTile(tilePath[1]).Value;

				turningTo = new Vector(firstToSecond);

				if (firstToSecond != tilePath[1].directionForTile(tilePath[2])) {
					return false;
				}

				var currentToFirst = currentTile.directionForTile(tilePath[0]).Value;
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
			move.WheelTurn = vehicle.steeringAngleForDirection(turn);

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

		public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {


			if (tilePath.Count < 3)
				return false;

			var currentTile = MyStrategy.map.tileAt(vehicle.position);

			Vector turningTo;
			Vector turningFrom;

			{

				var currentToFirst = currentTile.directionForTile(tilePath[0]).Value;
				var firstToSecond = tilePath[0].directionForTile(tilePath[1]).Value;
				var secondToThird = tilePath[1].directionForTile(tilePath[2]).Value;

				if (currentToFirst != firstToSecond)
					return false;

				if (firstToSecond.isSameAxis(secondToThird))
					return false;

				turningFrom = new Vector(currentToFirst);
				turningTo = new Vector(secondToThird);
			}
				
			var target = tilePath[0].center - turningTo * Constants.tileSize * 0.15;


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

	//
	//     [ ]
	//  [ ][ ]
	//  [*]
	//
	//
	public class SmoothDiagonalTurning : VehicleDriverStrategy {

		public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {


			if (tilePath.Count < 3)
				return false;

			var currentTile = MyStrategy.map.tileAt(vehicle.position);
			Vector target;

			{
				target = tilePath[2].center;

				var currentToFirst = currentTile.directionForTile(tilePath[0]).Value;
				var firstToSecond = tilePath[0].directionForTile(tilePath[1]).Value;
				var secondToThird = tilePath[1].directionForTile(tilePath[2]).Value;

				if (currentToFirst != secondToThird) {
					return false;
				}

				if (firstToSecond.isSameAxis(currentToFirst)) {
					return false;
				}

			}

			var vehicleToTarget = target - vehicle.position;

			if (vehicle.forward * vehicleToTarget < 0.5) {
				if (vehicle.speed.length > 10) {
					move.IsBrake = true;
				}
				move.EnginePower = 0.3;
			} else {
				move.EnginePower = 1;
			}
			move.WheelTurn = vehicle.steeringAngleForDirection(vehicleToTarget);
			move.IsBrake = false;

			return true;
		}

	}

	public class BackupStrategy : VehicleDriverStrategy {

		public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			var nextTile = tilePath[0];
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

