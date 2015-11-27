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

			strategies.AddLast(new TurnBackStrategy());
			strategies.AddLast(new BeforeTurnBackStrategy());

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

		private int zeroSpeedTicks = 0;

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			var forceGetBack = false;

			if (MyStrategy.currentTick > 180) {
				if (vehicle.speed.length < 1) {
					zeroSpeedTicks += 1;
				} else {
					zeroSpeedTicks = 0;
				}

				if (zeroSpeedTicks > 30) {
					forceGetBack = true;
                }
            }

			if (vehicle.speed.length > 5 && !forceGetBack)
				return false;

			var target = tilePath[0].center;

			if (MyStrategy.map.intersect(vehicle.forwardRay * Constants.vehicleLength * 2) != null || forceGetBack) {
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

			if (vehicle.speed.length > 10) {

				var innerSide1 = Ray.line(
					currentTile.center + turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningFrom * Constants.tileSize * 0.5,
					currentTile.center + turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) + turningFrom * Constants.tileSize * 0.5
				);

				//innerSide1.draw(0x00FF00);

				var innerCircle = new Circle(currentTile.center + (turningTo + turningFrom) * Constants.tileSize * 0.5, Constants.roadMargin);

				//innerCircle.draw(0x00FF00);

				var innerCircleExtended = new Circle(innerCircle.position, innerCircle.radius + 10);

				var backWall = new Ray(tilePath[0].center  + turningFrom * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningTo * Constants.tileSize * 0.5,
					               turningTo * Constants.tileSize * 2);

				//backWall.draw(0x00FF00);
	
				if (vehicle.forward * turningFrom > 0.45) {
					var vv = new VirtualVehicle(vehicle);
					var steering = Math.Sign(turningFrom.angleTo(turningTo));

					for (int i = 0; i < 100; ++i) { 
						vv.simulateTick(1.0, steering);



						if (vv.rect.isIntersect(innerSide1) || vv.rect.isIntersect(innerCircle)) {
							move.EnginePower = 1;
							move.WheelTurn = vehicle.steeringAngleForDirection(vehicle.stabilizationDir(turningFrom, currentTile.center, turningTo, -0.15 * Constants.tileSize));
							move.IsBrake = false;
							return true;
						}


						if (vv.rect.isIntersect(backWall)) {
							move.IsBrake = true;
							move.EnginePower = 1;
							move.WheelTurn = steering;
							return true;
						}

						if (tilePath[1].rect.contains(vv.position)) {

							// защита от слишком быстрого захода в поворот и ударения в заднюю стенку

							for (int j = 0; j < 50; ++j) {

								vv.simulateTick(1, steering);
								if (vv.rect.isIntersect(backWall)) {
									move.EnginePower = 1;
									move.WheelTurn = steering;
									move.IsBrake = true;
									return true;
								}

								//vv.position.draw(0x00FF00);
								//vv.rect.draw(0x0000FF);

							}

							move.EnginePower = 1;
							move.WheelTurn = steering;
							move.IsBrake = false;
							return true;
						}

						//vv.position.draw(0xFF0000); 
						//vv.rect.draw(0x0000FF);
					}
				} else if (vehicle.forward * turningTo > 0.5) {
					
					var vv = new VirtualVehicle(vehicle);
					var steering = Math.Sign(turningFrom.angleTo(turningTo));

					for (int i = 0; i < 100; ++i) { 
						vv.simulateTick(1.0, 0);

						if (vv.rect.isIntersect(innerSide1) || vv.rect.isIntersect(innerCircleExtended)) {

							move.EnginePower = 1;
							move.WheelTurn = -steering;
							move.IsBrake = false;
							return true;
						}

						if (tilePath[1].rect.contains(vv.position)) {
							move.EnginePower = 1;
							move.WheelTurn = 0;
							move.IsBrake = false;
							return true;
						}

						//vv.position.draw(0xFF0000); 
						//vv.rect.draw(0x0000FF);
					}
				} else {
					move.IsBrake = true;
					move.WheelTurn = 0;
					move.EnginePower = 1;
					return true;
				}

			}

			move.IsBrake = false;
			move.EnginePower = 1;
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

				if (tilePath.Count >= 4) {
					if (tilePath[2].directionForTile(tilePath[3]).Value.back() == currentToFirst)
						return false;
				}

				turningFrom = new Vector(currentToFirst);
				turningTo = new Vector(secondToThird);
			}


			if (vehicle.speed.length > 10 && vehicle.forward * turningFrom > 0.45) {
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

				var backWall = new Ray(tilePath[1].center + turningFrom * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningTo * (Constants.tileSize * 0.5), turningTo * 2 * Constants.tileSize);

				//backWall.draw(0x00FF00);

				var outerSide = new Ray(currentTile.center - turningTo * (Constants.halfTileSize - Constants.roadMargin) - turningFrom * Constants.halfTileSize, turningFrom * Constants.tileSize * 3);

				//outerSide.draw(0x00FF00);

				for (int i = 0; i < 100; ++i) { 
					vv.simulateTick(1.0, steering);
					if (vv.rect.isIntersect(innerSide1) || vv.rect.isIntersect(innerSide2))
						break;

					if (vv.rect.isIntersect(innerCircle))
						break;

					if (vv.rect.isIntersect(outerSide)) {
						var target = currentTile.center + turningFrom * Constants.tileSize * 1.5;

						move.WheelTurn = steering;
						move.EnginePower = 1;
						move.IsBrake = true;

						return true;
					}

					if (vv.rect.isIntersect(backWall)) {
						move.EnginePower = 1;
						move.WheelTurn = steering;
						move.IsBrake = true;
						return true;
					}

					if (tilePath[2].rect.contains(vv.position)) {

						// защита от слишком быстрого захода в поворот и ударения в заднюю стенку

						for (int j = 0; j < 50; ++j) {

							vv.simulateTick(1, steering);
							if (vv.rect.isIntersect(backWall)) {
								move.EnginePower = 1;
								move.WheelTurn = steering;
								move.IsBrake = true;
								return true;
							}

							//vv.position.draw(0x00FF00);
							//vv.rect.draw(0x0000FF);

						}

						move.EnginePower = 1;
						move.WheelTurn = steering;
						move.IsBrake = false;
						return true;

					}

					//vv.position.draw(0xFF0000); 
					//vv.rect.draw(0x0000FF);
				}

			}

				

			move.WheelTurn = vehicle.steeringAngleForDirection((currentTile.center + turningFrom * Constants.tileSize * 1.5) - vehicle.position);
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
			Vector turningFrom;
			Vector turningTo;

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

				turningFrom = new Vector(currentToFirst);
				turningTo = new Vector(firstToSecond);

				target = tilePath[2].center - turningFrom * 0.5 * Constants.tileSize;

			}

			var vehicleToTarget = target - vehicle.position;


			if (vehicle.speed.length > 10 && vehicle.forward * (turningFrom + turningTo) > 0) {

				var steering = Math.Sign(turningFrom.angleTo(turningTo));

				Ray side = new Ray(
					           tilePath[0].center - turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningFrom * Constants.tileSize * 0.5,
							   turningFrom * (Constants.tileSize - Constants.roadMargin)
				           );

				Ray back = new Ray(side.p2, turningTo * (Constants.tileSize - Constants.roadMargin));

				Circle backCircle = new Circle(back.p2 + turningFrom * Constants.roadMargin, Constants.roadMargin);

				Ray innerSide = new Ray(
					currentTile.center + turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningFrom * Constants.tileSize * 0.5,
					turningFrom * Constants.tileSize
				);

				Circle innerCircle = new Circle(innerSide.p2 + turningTo * Constants.roadMargin, Constants.roadMargin);

				side.draw(0x00FF00);
				back.draw(0x00FF00);
				backCircle.draw(0x00FF00);

				innerSide.draw(0x00FF00);
				innerCircle.draw(0x00FF00);

				var vv = new VirtualVehicle(vehicle);

				for (int i = 0; i < 100; ++i) {
					vv.simulateTick(1.0, 0.0);

					var rect = vv.rect;

					if (rect.isIntersect(side) || rect.isIntersect(back) || rect.isIntersect(backCircle)) {

						vv = new VirtualVehicle(vehicle);

						for (int j = 0; j < 100; ++j) {
							vv.simulateTick(1, steering);

							if (rect.isIntersect(innerSide) || rect.isIntersect(innerCircle)) {
								move.EnginePower = 0;
								move.IsBrake = false;
								move.WheelTurn = steering;
								return true;
							}
						}

						move.EnginePower = 1;
						move.IsBrake = false;
						move.WheelTurn = steering;
						return true;
					} else if (rect.isIntersect(innerSide) || rect.isIntersect(innerCircle)) {

						vv = new VirtualVehicle(vehicle);

						for (int j = 0; j < 100; ++j) {
							vv.simulateTick(1, -steering);

							if (rect.isIntersect(innerSide) || rect.isIntersect(innerCircle)) {
								move.EnginePower = 0;
								move.IsBrake = false;
								move.WheelTurn = -steering;
								return true;
							}
                        }

						move.EnginePower = 1;
						move.IsBrake = false;
						move.WheelTurn = -steering;
						return true;
					}

					if (tilePath[2].rect.contains(vv.position)) {
						move.EnginePower = 1;
						move.IsBrake = false;
						move.WheelTurn = vehicle.steeringAngleForDirection(vehicleToTarget);
						return true;
					}

					rect.draw(0x0000FF);
					vv.position.draw(0xFF0000);
				}

				move.EnginePower = 1;
				move.IsBrake = false;
				move.WheelTurn = vehicle.steeringAngleForDirection(vehicleToTarget);

				//new Ray(vehicle.position, vehicleToTarget).draw(0x00FFFF); 

				return true;

			}


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

	//
	//
	// [ ][ ]
	// [*][ ]
	//
	//
	public class TurnBackStrategy : VehicleDriverStrategy {

		public override bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			if (tilePath.Count < 2)
				return false;

			var current = MyStrategy.map.tileAt(vehicle.position);

			var currentToFirst = current.directionForTile(tilePath[0]).Value;
			var firstToSecond = tilePath[0].directionForTile(tilePath[1]).Value;
			var secondToThird = tilePath[1].directionForTile(tilePath[2]).Value;

			if (currentToFirst.isSameAxis(firstToSecond))
				return false;

			if (currentToFirst.back() != secondToThird)
				return false;


			var turningFrom = new Vector(currentToFirst);
			var turningTo = new Vector(firstToSecond);

			if (vehicle.speed.length > 10) {

				var steering = Math.Sign(turningFrom.angleTo(turningTo));


				var backWall = new Ray(
					tilePath[0].center + turningFrom * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningTo * Constants.tileSize * 0.5,
					turningTo * Constants.tileSize * 2
				);

				var backSideWall = new Ray(
					backWall.p2 - turningTo * Constants.roadMargin,
					-turningFrom * (Constants.tileSize - Constants.roadMargin)
				);

				var innerWall = new Ray(current.center + turningTo * (Constants.tileSize * 0.5 - Constants.roadMargin) - turningFrom * Constants.tileSize * 0.5,
					                turningFrom * Constants.tileSize);

				var innerCircle = new Circle(current.center + (new Vector(currentToFirst) + new Vector(firstToSecond)) * Constants.tileSize * 0.5, Constants.roadMargin);

				backWall.draw(0x00FF00);
				backSideWall.draw(0x00FF00);
				innerWall.draw(0x00FF00);
				innerCircle.draw(0x00FF00);

				var vv = new VirtualVehicle(vehicle);

				for (int i = 0; i < 100; ++i) {

					vv.simulateTick(0.5, steering);

					var rect = vv.rect;

					if (rect.isIntersect(innerWall) || rect.isIntersect(innerCircle)) {
						move.EnginePower = 1;
						move.WheelTurn = -steering;
						move.IsBrake = false;
						return true;
					}

					if (rect.isIntersect(backWall) || rect.isIntersect(backSideWall)) {
						move.IsBrake = false;
						move.EnginePower = -1;
						//move.WheelTurn = vehicle.steeringAngleForDirection(vehicle.stabilizationDir(turningFrom, current.center, turningTo, -0.25 * Constants.tileSize));
						move.WheelTurn = steering;
						return true;
					}

					vv.position.draw(0xFF0000); 
					vv.rect.draw(0x0000FF);

				}
			}

			move.EnginePower = 0.5;
			move.WheelTurn = vehicle.steeringAngleForDirection(vehicle.stabilizationDir(turningFrom, current.center, turningTo, -0.25 * Constants.tileSize));
			move.IsBrake = false;
			return true;
		}

	}

	//
	// [/][\]
	// [|][|]
	// [*]
	//
	//
	public class BeforeTurnBackStrategy : VehicleDriverStrategy {

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			if (tilePath.Count < 4)
				return false;

			var current = MyStrategy.map.tileAt(vehicle.position);

			var currentToFirst = current.directionForTile(tilePath[0]).Value;
			var firstToSecond = tilePath[0].directionForTile(tilePath[1]).Value;
			var secondToThird = tilePath[1].directionForTile(tilePath[2]).Value;
			var thirdToFour = tilePath[2].directionForTile(tilePath[3]).Value;

			if (!currentToFirst.isSameAxis(firstToSecond))
				return false;

			if (firstToSecond.isSameAxis(secondToThird))
				return false;

			if (firstToSecond.back() != thirdToFour)
				return false;

			var turningFrom = new Vector(currentToFirst);
			var turningTo = new Vector(firstToSecond);

			move.EnginePower = 1;
			move.WheelTurn = vehicle.steeringAngleForDirection(vehicle.stabilizationDir(turningFrom, current.center, turningTo, -0.25 * Constants.tileSize));

			if (vehicle.speed.length > 15) {
				move.IsBrake = true;
			} else {
				move.IsBrake = false;
			}
			return true;


		}

	}


	public class BackupStrategy : VehicleDriverStrategy {

		override public bool tryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			var nextTile = tilePath[0];
//			nextTile.draw(0xFF0000);
//			nextTile.center.draw(0xFF1010);

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

