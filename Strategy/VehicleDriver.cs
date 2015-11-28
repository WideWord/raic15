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

		public void Drive(Vehicle vehicle, List<Tile> tilePath, Move move) {


			//defaults
			move.IsUseNitro = false;

			foreach (var strategy in strategies) {
				if (strategy.TryDrive(vehicle, tilePath, move)) {

					Debug.Print(vehicle.Position + new Vector(15, 15), strategy.GetType().Name, Color.Black);

					return;
				}
			}

			//throw new Exception("Do not have strategy for this situation");
		}
	}

	public abstract class VehicleDriverStrategy {

		public abstract bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move);

		public virtual void ResetState() {}
	}

	public class GetBackStrategy : VehicleDriverStrategy {

		private int zeroSpeedTicks = 0;

		override public bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			var forceGetBack = false;

			if (MyStrategy.CurrentTick > 180) {
				if (vehicle.Speed.Length < 1) {
					zeroSpeedTicks += 1;
				} else {
					zeroSpeedTicks = 0;
				}

				if (zeroSpeedTicks > 30) {
					forceGetBack = true;
                }
            }

			if (vehicle.Speed.Length > 5 && !forceGetBack)
				return false;

			var target = tilePath[0].Center;

			if (MyStrategy.Map.IsIntersect(vehicle.ForwardRay * Constants.VehicleLength * 2) || forceGetBack) {
				move.IsBrake = false;
				move.EnginePower = -1;
				move.WheelTurn = -vehicle.Forward.AngleTo(target - vehicle.Position) * 4;
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

		override public bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			var current = MyStrategy.Map.TileAt(vehicle.Position);


			AxisDirection dir;

			{

				dir = current.DirectionForTile(tilePath[0]) ?? AxisDirection.Up;

				for (int i = 1; i < 3  && i < tilePath.Count; ++i) {
					if (tilePath[i - 1].DirectionForTile(tilePath[i]) != dir)
						return false;
                }
			}

			var dirVec = new Vector(dir);

			var last = current;

			int lineTilesCtr = 0;

			foreach (var tile in tilePath) {
				if (dir.TurnLeft() == last.DirectionForTile(tile)) {
					var turnVector = new Vector(dir.TurnLeft());
					var dist = (vehicle.Position - current.Center) * turnVector + 0.15 * Constants.TileSize;
					dirVec -= turnVector * dist / Constants.TileSize;
					break;
				} else if (dir.TurnRight() == last.DirectionForTile(tile)) {
					var turnVector = new Vector(dir.TurnRight());
					var dist = (vehicle.Position - current.Center) * turnVector + 0.15 * Constants.TileSize;
					dirVec -= turnVector * dist / Constants.TileSize;
					break;
				} else if (dir != last.DirectionForTile(tile))
					break;

				lineTilesCtr += 1;

				last = tile;

			}

			if (lineTilesCtr >= 8) {
				move.IsUseNitro = true;
			}

			if (vehicle.Forward * dirVec > 0.3) {
				move.EnginePower = 1;
				move.WheelTurn = vehicle.SteeringAngleForDirection(dirVec);
			} else {
				move.EnginePower = 0.2;
				move.WheelTurn = vehicle.SteeringAngleForDirection(dirVec);
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

		override public bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			if (tilePath.Count < 3)
				return false;

			var currentTile = MyStrategy.Map.TileAt(vehicle.Position);

			Vector turningTo;
			Vector turningFrom;

			{
				var firstToSecond = tilePath[0].DirectionForTile(tilePath[1]).Value;

				turningTo = new Vector(firstToSecond);

				if (firstToSecond != tilePath[1].DirectionForTile(tilePath[2])) {
					return false;
				}

				var currentToFirst = currentTile.DirectionForTile(tilePath[0]).Value;
				if (currentToFirst.IsSameAxis(firstToSecond)) {
					return false;
				}

				turningFrom = new Vector(currentToFirst);

			}

			if (vehicle.Speed.Length > 10) {

				var innerSide1 = Ray.Line(
					currentTile.Center + turningTo * (Constants.TileSize * 0.5 - Constants.RoadMargin) - turningFrom * Constants.TileSize * 0.5,
					currentTile.Center + turningTo * (Constants.TileSize * 0.5 - Constants.RoadMargin) + turningFrom * Constants.TileSize * 0.5
				);

				//innerSide1.draw(0x00FF00);

				var innerCircle = new Circle(currentTile.Center + (turningTo + turningFrom) * Constants.TileSize * 0.5, Constants.RoadMargin);

				//innerCircle.draw(0x00FF00);

				var innerCircleExtended = new Circle(innerCircle.Position, innerCircle.Radius + 10);

				var backWall = new Ray(tilePath[0].Center  + turningFrom * (Constants.TileSize * 0.5 - Constants.RoadMargin) - turningTo * Constants.TileSize * 0.5,
					               turningTo * Constants.TileSize * 2);

				//backWall.draw(0x00FF00);
	
				if (vehicle.Forward * turningFrom > 0.45) {
					var vv = new VirtualVehicle(vehicle);
					var steering = Math.Sign(turningFrom.AngleTo(turningTo));

					for (int i = 0; i < 100; ++i) { 
						vv.SimulateTick(1.0, steering);



						if (vv.Rect.IsIntersect(innerSide1) || vv.Rect.IsIntersect(innerCircle)) {
							move.EnginePower = 1;
							move.WheelTurn = vehicle.SteeringAngleForDirection(vehicle.StabilizationDir(turningFrom, currentTile.Center, turningTo, -0.15 * Constants.TileSize));
							move.IsBrake = false;
							return true;
						}


						if (vv.Rect.IsIntersect(backWall)) {
							move.IsBrake = true;
							move.EnginePower = 1;
							move.WheelTurn = steering;
							return true;
						}

						if (tilePath[1].Rect.contains(vv.Position)) {

							// защита от слишком быстрого захода в поворот и ударения в заднюю стенку

							for (int j = 0; j < 50; ++j) {

								vv.SimulateTick(1, steering);
								if (vv.Rect.IsIntersect(backWall)) {
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
				} else if (vehicle.Forward * turningTo > 0.5) {
					
					var vv = new VirtualVehicle(vehicle);
					var steering = Math.Sign(turningFrom.AngleTo(turningTo));

					for (int i = 0; i < 100; ++i) { 
						vv.SimulateTick(1.0, 0);

						if (vv.Rect.IsIntersect(innerSide1) || vv.Rect.IsIntersect(innerCircleExtended)) {

							move.EnginePower = 1;
							move.WheelTurn = -steering;
							move.IsBrake = false;
							return true;
						}

						if (tilePath[1].Rect.contains(vv.Position)) {
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
			move.WheelTurn = vehicle.SteeringAngleForDirection(turn);

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

		override public bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {


			if (tilePath.Count < 3)
				return false;

			var currentTile = MyStrategy.Map.TileAt(vehicle.Position);

			Vector turningTo;
			Vector turningFrom;

			{

				var currentToFirst = currentTile.DirectionForTile(tilePath[0]).Value;
				var firstToSecond = tilePath[0].DirectionForTile(tilePath[1]).Value;
				var secondToThird = tilePath[1].DirectionForTile(tilePath[2]).Value;

				if (currentToFirst != firstToSecond)
					return false;

				if (firstToSecond.IsSameAxis(secondToThird))
					return false;

				if (tilePath.Count >= 4) {
					if (tilePath[2].DirectionForTile(tilePath[3]).Value.Back() == currentToFirst)
						return false;
				}

				turningFrom = new Vector(currentToFirst);
				turningTo = new Vector(secondToThird);
			}


			if (vehicle.Speed.Length > 10 && vehicle.Forward * turningFrom > 0.45) {
				var vv = new VirtualVehicle(vehicle);
				var steering = Math.Sign(turningFrom.AngleTo(turningTo));

				var innerSide1 = Ray.Line(
					currentTile.Center + turningTo * (Constants.TileSize * 0.5 - Constants.RoadMargin) - turningFrom * Constants.TileSize * 0.5,
					currentTile.Center + turningTo * (Constants.TileSize * 0.5 - Constants.RoadMargin) + turningFrom * Constants.TileSize * 0.5
				);

				//innerSide1.draw(0x00FF00);

				var innerSide2 = new Ray(innerSide1.Position + turningFrom * Constants.TileSize, innerSide1.Direction);

				//innerSide2.draw(0x00FF00);

				var innerCircle = new Circle(currentTile.Center + (turningTo + turningFrom) * Constants.TileSize * 0.5 + turningFrom * Constants.TileSize, Constants.RoadMargin);

				//innerCircle.draw(0x00FF00);  

				var backWall = new Ray(tilePath[1].Center + turningFrom * (Constants.TileSize * 0.5 - Constants.RoadMargin) - turningTo * (Constants.TileSize * 0.5), turningTo * 2 * Constants.TileSize);

				//backWall.draw(0x00FF00);

				var outerSide = new Ray(currentTile.Center - turningTo * (Constants.HalfTileSize - Constants.RoadMargin) - turningFrom * Constants.HalfTileSize, turningFrom * Constants.TileSize * 3);

				//outerSide.draw(0x00FF00);

				for (int i = 0; i < 100; ++i) { 
					vv.SimulateTick(1.0, steering);
					if (vv.Rect.IsIntersect(innerSide1) || vv.Rect.IsIntersect(innerSide2))
						break;

					if (vv.Rect.IsIntersect(innerCircle))
						break;

					if (vv.Rect.IsIntersect(outerSide)) {
						var target = currentTile.Center + turningFrom * Constants.TileSize * 1.5;

						move.WheelTurn = steering;
						move.EnginePower = 1;
						move.IsBrake = true;

						return true;
					}

					if (vv.Rect.IsIntersect(backWall)) {
						move.EnginePower = 1;
						move.WheelTurn = steering;
						move.IsBrake = true;
						return true;
					}

					if (tilePath[2].Rect.contains(vv.Position)) {

						// защита от слишком быстрого захода в поворот и ударения в заднюю стенку

						for (int j = 0; j < 50; ++j) {

							vv.SimulateTick(1, steering);
							if (vv.Rect.IsIntersect(backWall)) {
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

				

			move.WheelTurn = vehicle.SteeringAngleForDirection((currentTile.Center + turningFrom * Constants.TileSize * 1.5) - vehicle.Position);
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

		override public bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {


			if (tilePath.Count < 3)
				return false;

			var currentTile = MyStrategy.Map.TileAt(vehicle.Position);
			Vector target;
			Vector turningFrom;
			Vector turningTo;

			{
				

				var currentToFirst = currentTile.DirectionForTile(tilePath[0]).Value;
				var firstToSecond = tilePath[0].DirectionForTile(tilePath[1]).Value;
				var secondToThird = tilePath[1].DirectionForTile(tilePath[2]).Value;

				if (currentToFirst != secondToThird) {
					return false;
				}

				if (firstToSecond.IsSameAxis(currentToFirst)) {
					return false;
				}

				turningFrom = new Vector(currentToFirst);
				turningTo = new Vector(firstToSecond);

				target = tilePath[2].Center - turningFrom * 0.5 * Constants.TileSize;

			}

			var vehicleToTarget = target - vehicle.Position;


			if (vehicle.Speed.Length > 10 && vehicle.Forward * (turningFrom + turningTo) > 0) {

				var steering = Math.Sign(turningFrom.AngleTo(turningTo));

				Ray side = new Ray(
					           tilePath[0].Center - turningTo * (Constants.TileSize * 0.5 - Constants.RoadMargin) - turningFrom * Constants.TileSize * 0.5,
							   turningFrom * (Constants.TileSize - Constants.RoadMargin)
				           );

				Ray back = new Ray(side.EndPoint, turningTo * (Constants.TileSize - Constants.RoadMargin));

				Circle backCircle = new Circle(back.EndPoint + turningFrom * Constants.RoadMargin, Constants.RoadMargin);

				Ray innerSide = new Ray(
					currentTile.Center + turningTo * (Constants.TileSize * 0.5 - Constants.RoadMargin) - turningFrom * Constants.TileSize * 0.5,
					turningFrom * Constants.TileSize
				);

				Circle innerCircle = new Circle(innerSide.EndPoint + turningTo * Constants.RoadMargin, Constants.RoadMargin);
				

				var vv = new VirtualVehicle(vehicle);

				for (int i = 0; i < 100; ++i) {
					vv.SimulateTick(1.0, 0.0);

					var rect = vv.Rect;

					if (rect.IsIntersect(side) || rect.IsIntersect(back) || rect.IsIntersect(backCircle)) {

						vv = new VirtualVehicle(vehicle);

						for (int j = 0; j < 100; ++j) {
							vv.SimulateTick(1, steering);

							if (rect.IsIntersect(innerSide) || rect.IsIntersect(innerCircle)) {
								move.EnginePower = 1;
								move.IsBrake = true;
								move.WheelTurn = steering;
								return true;
							}
						}

						move.EnginePower = 1;
						move.IsBrake = false;
						move.WheelTurn = steering;
						return true;
					} else if (rect.IsIntersect(innerSide) || rect.IsIntersect(innerCircle)) {

						vv = new VirtualVehicle(vehicle);

						for (int j = 0; j < 100; ++j) {
							vv.SimulateTick(1, -steering);

							if (rect.IsIntersect(innerSide) || rect.IsIntersect(innerCircle)) {
								move.EnginePower = 1;
								move.IsBrake = true;
								move.WheelTurn = -steering;
								return true;
							}
                        }

						move.EnginePower = 1;
						move.IsBrake = false;
						move.WheelTurn = -steering;
						return true;
					}

					if (tilePath[2].Rect.contains(vv.Position)) {
						move.EnginePower = 1;
						move.IsBrake = false;
						move.WheelTurn = vehicle.SteeringAngleForDirection(vehicleToTarget);
						return true;
					}
					
				}

				move.EnginePower = 1;
				move.IsBrake = false;
				move.WheelTurn = vehicle.SteeringAngleForDirection(vehicleToTarget);

				//new Ray(vehicle.position, vehicleToTarget).draw(0x00FFFF); 

				return true;

			}


			if (vehicle.Forward * vehicleToTarget < 0.5) {
				if (vehicle.Speed.Length > 10) {
					move.IsBrake = true;
				}
				move.EnginePower = 0.3;
			} else {
				move.EnginePower = 1;
			}
			move.WheelTurn = vehicle.SteeringAngleForDirection(vehicleToTarget);
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

		public override bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			if (tilePath.Count < 2)
				return false;

			var current = MyStrategy.Map.TileAt(vehicle.Position);

			var currentToFirst = current.DirectionForTile(tilePath[0]).Value;
			var firstToSecond = tilePath[0].DirectionForTile(tilePath[1]).Value;
			var secondToThird = tilePath[1].DirectionForTile(tilePath[2]).Value;

			if (currentToFirst.IsSameAxis(firstToSecond))
				return false;

			if (currentToFirst.Back() != secondToThird)
				return false;


			var turningFrom = new Vector(currentToFirst);
			var turningTo = new Vector(firstToSecond);

			if (vehicle.Speed.Length > 10) {

				var steering = Math.Sign(turningFrom.AngleTo(turningTo));


				var backWall = new Ray(
					tilePath[0].Center + turningFrom * (Constants.TileSize * 0.5 - Constants.RoadMargin) - turningTo * Constants.TileSize * 0.5,
					turningTo * Constants.TileSize * 2
				);

				var backSideWall = new Ray(
					backWall.EndPoint - turningTo * Constants.RoadMargin,
					-turningFrom * (Constants.TileSize - Constants.RoadMargin)
				);

				var innerWall = new Ray(current.Center + turningTo * (Constants.TileSize * 0.5 - Constants.RoadMargin) - turningFrom * Constants.TileSize * 0.5,
					                turningFrom * Constants.TileSize);

				var innerCircle = new Circle(current.Center + (new Vector(currentToFirst) + new Vector(firstToSecond)) * Constants.TileSize * 0.5, Constants.RoadMargin);
				

				var vv = new VirtualVehicle(vehicle);

				for (int i = 0; i < 100; ++i) {

					vv.SimulateTick(0.5, steering);

					var rect = vv.Rect;

					if (rect.IsIntersect(innerWall) || rect.IsIntersect(innerCircle)) {
						move.EnginePower = 1;
						move.WheelTurn = -steering;
						move.IsBrake = false;
						return true;
					}

					if (rect.IsIntersect(backWall) || rect.IsIntersect(backSideWall)) {
						move.IsBrake = false;
						move.EnginePower = -1;
						//move.WheelTurn = vehicle.steeringAngleForDirection(vehicle.stabilizationDir(turningFrom, current.center, turningTo, -0.25 * Constants.tileSize));
						move.WheelTurn = steering;
						return true;
					}
					

				}
			}

			move.EnginePower = 0.5;
			move.WheelTurn = vehicle.SteeringAngleForDirection(vehicle.StabilizationDir(turningFrom, current.Center, turningTo, -0.25 * Constants.TileSize));
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

		override public bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			if (tilePath.Count < 4)
				return false;

			var current = MyStrategy.Map.TileAt(vehicle.Position);

			var currentToFirst = current.DirectionForTile(tilePath[0]).Value;
			var firstToSecond = tilePath[0].DirectionForTile(tilePath[1]).Value;
			var secondToThird = tilePath[1].DirectionForTile(tilePath[2]).Value;
			var thirdToFour = tilePath[2].DirectionForTile(tilePath[3]).Value;

			if (!currentToFirst.IsSameAxis(firstToSecond))
				return false;

			if (firstToSecond.IsSameAxis(secondToThird))
				return false;

			if (firstToSecond.Back() != thirdToFour)
				return false;

			var turningFrom = new Vector(currentToFirst);
			var turningTo = new Vector(firstToSecond);

			move.EnginePower = 1;
			move.WheelTurn = vehicle.SteeringAngleForDirection(vehicle.StabilizationDir(turningFrom, current.Center, turningTo, -0.25 * Constants.TileSize));

			if (vehicle.Speed.Length > 15) {
				move.IsBrake = true;
			} else {
				move.IsBrake = false;
			}
			return true;


		}

	}


	public class BackupStrategy : VehicleDriverStrategy {

		override public bool TryDrive(Vehicle vehicle, List<Tile> tilePath, Move move) {

			var nextTile = tilePath[0];

			if (vehicle.Forward * (nextTile.Center - vehicle.Position) < 0.4 && vehicle.Speed.Length > 10) {
				move.IsBrake = true;
				move.EnginePower = -1;
			} else {
				move.IsBrake = false;
				move.EnginePower = 1;
			}

			move.WheelTurn = vehicle.Forward.AngleTo((nextTile.Center - vehicle.Position)) * 4;


			return true;

		}

	}
}

