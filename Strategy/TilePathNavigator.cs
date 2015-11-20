using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {
	
	public class TilePathNavigator {

		private LinkedListNode<PathUtil.TilePathNode> _nextNode;

		public LinkedListNode<PathUtil.TilePathNode> nextNode { 
			get { 
				update();
				return _nextNode;
			}
		}
		private Vehicle vehicle;

		public TilePathNavigator(LinkedList<PathUtil.TilePathNode> path, Vehicle vehicle) {
			_nextNode = path.First;
			this.vehicle = vehicle;
		}

		void update() {
			var nextTile = _nextNode.Value.tile;

			if (nextTile.rect.contains(vehicle.position) && _nextNode.Next != null) {
				_nextNode = _nextNode.Next;
			}
		}

	}
}

