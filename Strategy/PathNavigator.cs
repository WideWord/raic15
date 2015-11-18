using System;
using System.Collections.Generic;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class PathNavigator {

		private LinkedListNode<PathUtil.PathNode> nextNodeNode;

		public PathNavigator(LinkedList<PathUtil.PathNode> path) {
			this.nextNodeNode = path.First;
		}

		public Vector nextPoint(Vehicle vehicle) {

			var nextNode = nextNodeNode.Value.position;

			if (nextNodeNode.Next != null) {
				var nextNextNode = nextNodeNode.Next.Value.position;

				if (((nextNode - vehicle.position) * vehicle.forward < 0 && (nextNextNode - vehicle.position) * vehicle.forward > 0) || (nextNode - vehicle.position).length < Constants.tileSize * 0.4) {
					nextNodeNode = nextNodeNode.Next;
					nextNode = nextNextNode;
				}
			}

			return nextNode;
		}

	}
}

