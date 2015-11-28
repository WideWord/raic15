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

		public void Tick(Move move) {

			if (MyStrategy.TileAtWaypoint(currentWaypoint + 1).Rect.contains(Position)) {
				currentWaypoint += 1;
			}

			var _currentTile = MyStrategy.Map.TileAt(Position);
			if (_currentTile != currentTile) {
				currentTile = _currentTile;

				if (path == null) {
					path = new TilePath(MyStrategy.Waypoints, MyStrategy.Map, this, currentWaypoint + 1);
				} else {
					if (path.TileList[0].Rect.contains(Position)) {
						path.TileList.RemoveAt(0);

						if (path.TileList.Count < 10) {
							path = new TilePath(MyStrategy.Waypoints, MyStrategy.Map, this, currentWaypoint + 1);
						}
					} else {
						path = new TilePath(MyStrategy.Waypoints, MyStrategy.Map, this, currentWaypoint + 1);
					}


				}
			}
			
			driver.Drive(this, path.TileList, move);
			path.Draw(Color.Red);


		}	

	}
}

