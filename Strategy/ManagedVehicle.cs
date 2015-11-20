using System;
using System.Collections.Generic;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class ManagedVehicle : Vehicle {

		private TilePathNavigator navigator;
		private VehicleDriver driver;

		public ManagedVehicle(LinkedList<PathUtil.TilePathNode> tilePath) {
			navigator = new TilePathNavigator(tilePath, this);
			driver = new VehicleDriver();
		}

		public void tick(Move move) {

			driver.drive(this, navigator.nextNode, move);

		}

	}
}

