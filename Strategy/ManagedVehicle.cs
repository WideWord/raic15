using System;
using System.Collections.Generic;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class ManagedVehicle : Vehicle {

		private VehicleDriver driver = new VehicleDriver();
		private int currentWaypoint = 0;

		private VirtualVehicle virtualVehicle;
		private bool initialized = false;

		public void tick(Move move) {

			/*var nextWaypointCoords = MyStrategy.waypoints[(currentWaypoint + 1) % MyStrategy.waypoints.Length];
			var nextWaypointTile = MyStrategy.map.tileAt(nextWaypointCoords[0], nextWaypointCoords[1]);

			if (nextWaypointTile.rect.contains(position)) {
				currentWaypoint += 1;
			}

			var path = PathUtil.findPathFromWaypoints(MyStrategy.waypoints, MyStrategy.map, position, currentWaypoint + 1);
	
			driver.drive(this, path.First, move);
			*/


			if (Constants.currentTick > 300) {
				if (!initialized) {

					virtualVehicle = new VirtualVehicle(this);

					initialized = true;
				} else {
				
					move.EnginePower = -1.0;

					virtualVehicle.simulateTick(-1.0, 0.0);

					Console.WriteLine("{0} {1} {2}", (position - virtualVehicle.position).length, enginePower, virtualVehicle.enginePower);

					virtualVehicle.position.draw(0xFF0000);
				
				}
			}
		}	

	}
}

