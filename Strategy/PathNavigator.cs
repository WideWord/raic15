using System;
using System.Collections.Generic;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class PathNavigator {

		private LinkedListNode<Vector> nextNodeNode;

		public PathNavigator(LinkedList<Vector> path) {
			this.nextNodeNode = path.First;
		}

		public Vector nextPoint(Car self) {
			var carPosition = new Vector(self.X, self.Y);
			var carForward = Vector.fromAngle(self.Angle);

			var nextNode = nextNodeNode.Value;

			if (nextNodeNode.Next != null) {
				var nextNextNode = nextNodeNode.Next.Value;

				if ((nextNode - carPosition) * carForward < 0 && (nextNextNode - carPosition) * carForward > 0) {
					nextNodeNode = nextNodeNode.Next;
					nextNode = nextNextNode;
				}
			}

			return nextNode;
		}

	}
}

