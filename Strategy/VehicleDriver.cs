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

	public abstract class VehicleDriverStrategy {

		public abstract bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move);

		public virtual void resetState() {}
	}

	public class GetBackStrategy : VehicleDriverStrategy {

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

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

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			var current = MyStrategy.map.tileAt(vehicle.position);


			AxisDirection dir;

			{

				dir = current.directionForTile(tilePath[0]) ?? AxisDirection.up;

				for (int i = 1; i < 3  && i < tilePath.Count; ++i) {
					if (tilePath[i - 1].directionForTile(tilePath[i]) != dir)
						return false;
                }
			}

			var dirVec = new Vector(dir);

			var last = current;

			foreach (var tile in tilePath) {
				if (dir.turnLeft() == last.directionForTile(tile)) {
					var turnVector = new Vector(dir.turnLeft());
					var dist = (vehicle.position - current.center) * turnVector + 0.15 * Constants.tileSize;
					dirVec -= turnVector * dist / Constants.tileSize;
					break;
				} else if (dir.turnRight() == last.directionForTile(tile)) {
					var turnVector = new Vector(dir.turnRight());
					var dist = (vehicle.position - current.center) * turnVector + 0.15 * Constants.tileSize;
					dirVec -= turnVector * dist / Constants.tileSize;
					break;
				} else if (dir != last.directionForTile(tile))
					break;

				last = tile;

			}

			//new Ray(vehicle.position, dirVec * 500).draw(0x00FF00);

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

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

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

			if (vehicle.speed.length > 20 && vehicle.forward * turningFrom > 0) {
				var vv = new VirtualVehicle(vehicle);
				var steering = Math.Sign(turningFrom.angleTo(turningTo));

				var innerSide1 = Ray.line(
					currentTile.center + turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningFrom * Constants.tileSize * 0.5,
					currentTile.center + turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) + turningFrom * Constants.tileSize * 0.5
				);

				//innerSide1.draw(0x00FF00);

				var innerCircle = new Circle(currentTile.center + (turningTo + turningFrom) * Constants.tileSize * 0.5, Constants.roadMargin);

				//innerCircle.draw(0x00FF00);  

				for (int i = 0; i < 100; ++i) { 
					vv.simulateTick(1.0, steering);
					if (vv.rect.isIntersect(innerSide1))
						break;

					if (vv.rect.isIntersect(innerCircle))
						break;

					if (tilePath[1].rect.contains(vv.position)) {
						move.EnginePower = 1;
						move.WheelTurn = steering;
						move.IsBrake = false;
						return true;
					}

					//vv.position.draw(0xFF0000); 
					//vv.rect.draw(0x0000FF);
				}

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

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {


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


			if (vehicle.speed.length > 20 && vehicle.forward * turningFrom > 0) {
				var vv = new VirtualVehicle(vehicle);
				var steering = Math.Sign(turningFrom.angleTo(turningTo));

				var innerSide1 = Ray.line(
					currentTile.center + turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningFrom * Constants.tileSize * 0.5,
					currentTile.center + turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) + turningFrom * Constants.tileSize * 0.5
				);

				//innerSide1.draw(0x00FF00);

				var innerSide2 = new Ray(innerSide1.position + turningFrom * Constants.tileSize, innerSide1.direction);

				//innerSide2.draw(0x00FF00);

				var innerCircle = new Circle(currentTile.center + (turningTo + turningFrom) * Constants.tileSize * 0.5 + turningFrom * Constants.tileSize, Constants.roadMargin);

				//innerCircle.draw(0x00FF00);  

				for (int i = 0; i < 100; ++i) { 
					vv.simulateTick(1.0, steering);
					if (vv.rect.isIntersect(innerSide1) || vv.rect.isIntersect(innerSide2))
						break;

					if (vv.rect.isIntersect(innerCircle))
						break;

					if (tilePath[2].rect.contains(vv.position)) {
						move.EnginePower = 1;
						move.WheelTurn = steering;
						move.IsBrake = false;
						return true;
					}

					//vv.position.draw(0xFF0000); 
					//vv.rect.draw(0x0000FF);
				}

			}

				
			var target = currentTile.center + turningFrom * Constants.tileSize * 0.5 - turningTo * Constants.tileSize * 0.2;
			//target.draw(0xFF1010);

			move.WheelTurn = vehicle.steeringAngleForDirection(target - vehicle.position);
			move.EnginePower = 1;
			move.IsBrake = false;

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

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {


			if (tilePath.Count < 3)
				return false;

			var currentTile = MyStrategy.map.tileAt(vehicle.position);
			Vector target;

			{
				

				var currentToFirst = currentTile.directionForTile(tilePath[0]).Value;
				var firstToSecond = tilePath[0].directionForTile(tilePath[1]).Value;
				var secondToThird = tilePath[1].directionForTile(tilePath[2]).Value;

				if (currentToFirst != secondToThird) {
					return false;
				}

				if (firstToSecond.isSameAxis(currentToFirst)) {
					return false;
				}

				target = tilePath[2].center + (-new Vector(currentToFirst) + new Vector(firstToSecond)) * 0.25;

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

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

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

