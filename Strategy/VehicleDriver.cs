using System;
using System.Collections.Generic;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class VehicleDriver {

		private RacePath path;

		public void drive(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> tilePath, Move move) {

			move.EnginePower = 1.0;

			if (MyStrategy.currentTick % 10 == 0) {
				path = new RacePath(vehicle, tilePath);
			}
			path?.draw(0x50FF50);

		} 
	}

}

