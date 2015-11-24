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
			if (_currentTile != currentTile) {
				currentTile = _currentTile;

				if (path == null) {
					path = new TilePath(MyStrategy.waypoints, MyStrategy.map, this, currentWaypoint + 1);
				} else {
					if (path.tilePath[0].rect.contains(position)) {
						path.tilePath.RemoveAt(0);

						if (path.tilePath.Count < 10) {
							path = new TilePath(MyStrategy.waypoints, MyStrategy.map, this, currentWaypoint + 1);
						}
					} else {
						path = new TilePath(MyStrategy.waypoints, MyStrategy.map, this, currentWaypoint + 1);
					}


				}
			}

			driver.drive(this, path.tilePath, move);



		}	

	}
}

