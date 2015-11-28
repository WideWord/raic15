using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	class BFVehicleDriver {

		public MoveInstruction Drive(Vehicle vehicle, Tile target) {
			double cost;
			return FindBestMove(new VirtualVehicle(vehicle), target, out cost);
		}

		MoveInstruction FindBestMove(VirtualVehicle vehicle, Tile target, out double cost, int deepLimit = 1) {

			var map = target.RoadMap;

			double minCost = double.PositiveInfinity;
			MoveInstruction bestMove = new MoveInstruction();

			for (int steering = -1; steering <= 1; ++steering) {

				var vv = vehicle;
				var move = new MoveInstruction {
					EnginePower = 1,
					SteeringAngle = steering
				};

				VirtualVehicle[] vvs = new VirtualVehicle[100];

				for (int i = 0; i < 100; ++i) {
					vv.SimulateTick(1, steering);

					vvs[i] = vv;

                    if (vv.Rect.IsIntersect(map)) {
						break;
					}

					if (i % 5 == 0 && i != 0) {
						if (deepLimit > 0) {
							double curCost;
							FindBestMove(vv, target, out curCost, deepLimit - 1);
							if (minCost > curCost) {
								minCost = curCost;
								bestMove = move;
							}
						}

						{
							var curCost = map.TileAt(vv.Position).DistanceMap.DistanceFor(map.TileAt(vv.Position));
							if (minCost > curCost) {
								minCost = curCost;
								bestMove = move;
							}
						}
					}

					Debug.FillCircle(vv.Position, 2, Color.Red);

				}

			}

			cost = minCost;
			return bestMove;
		}


	}
}
