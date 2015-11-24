using System;
using System.Collections.Generic;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class ManagedVehicle : Vehicle {

		private VehicleDriver driver = new VehicleDriver();
		private Tile currentTile;
		private int currentWaypoint = 0;

		private TilePath path;

		public void tick(Move move) {

			if (MyStrategy.tileAtWaypoint(currentWaypoint + 1).rect.contains(position)) {
				currentWaypoint += 1;
			}

			var _currentTile = MyStrategy.map.tileAt(position);
			if (currentTile != _currentTile) {
				currentTile = _currentTile;
				path = new TilePath(MyStrategy.waypoints, MyStrategy.map, this, currentWaypoint + 1);
			}

			path.draw(0xFF0000);

			//driver.drive(this, path, move);

		}	

	}
}

