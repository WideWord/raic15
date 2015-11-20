using System;
using System.Collections.Generic;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class VehicleDriver {
		
		public void drive(Vehicle vehicle, LinkedListNode<PathUtil.TilePathNode> pathPoint, Move move) {
			move.EnginePower = 0.3;

			var nextTile = pathPoint.Value.tile;

			move.WheelTurn = vehicle.forward.angleTo((nextTile.center - vehicle.position)) * 1.7;

			nextTile.draw(0xFF0000);

		}

	}
}

