using System;
using System.Collections.Generic;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class VehicleDriver {
		
		public void drive(Vehicle vehicle, LinkedListNode<PathUtil.TilePathNode> pathPoint, Move move) {

			var path = new LinkedList<Tile>();

			var node = pathPoint;

			for (int i = 0; i < 6 && node != null; ++i) {

				path.AddLast(node.Value.tile);

				node = node.Next;
			}

			foreach (var tile in path) {
				tile.draw(0xFF0000);
			}

			var nextTile = path.First.Value;

			var directionToTile = nextTile.center - vehicle.position;

			move.EnginePower = 0.3;

			move.WheelTurn = vehicle.forward.angleTo(directionToTile) * 4;


		}

	}
}

