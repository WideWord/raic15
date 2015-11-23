using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public class RacePath {

		private LinkedList<Vector> points = new LinkedList<Vector>();

		private class PathPoint {
			public Vector position;
			public Vector forward;
			public double speed;
			public int depth;

			public PathPoint prev;

		}

		public RacePath(Vehicle vehicle, LinkedList<PathUtil.TilePathNode> path) {

			const double accel = 0.025;
			const double brake = 0.0175;
			const double steeringFactor = 0.05;
			const double movementFactor = 1;
			const double scale = 50;

			var initial = new PathPoint() { position = vehicle.position, speed = vehicle.speed.length, forward = vehicle.forward, depth = 0 };

			var queue = new Queue<PathPoint>();
			queue.Enqueue(initial);

			var target = path.First.Next.Next.Next.Next.Value.tile;

			PathPoint best = initial;
			double bestDist = double.MaxValue;

			while (queue.Count > 0) {

				var current = queue.Dequeue();

				if ((current.position - target.center).length < bestDist) {
					bestDist = (current.position - target.center).length;
					best = current;
				}

				if (target.rect.contains(current.position)) {
					break;
				}

				if (current.depth > 5)
					continue;

				for (int ste = -1; ste <= 1; ++ste) {

					double steering = (double)ste;

					//accel
					{
						var next = new PathPoint();
						next.speed = MyMath.limit(current.speed + accel * scale, 1.0);
						next.forward = current.forward.rotate(steering * steeringFactor * scale / next.speed);
						next.position = current.position + next.speed * next.forward * movementFactor * scale;

						next.prev = current;
						next.depth = current.depth + 1;

						if (!MyStrategy.map.isIntersectRoughly(Ray.line(current.position, next.position))) {
							queue.Enqueue(next);
						}
					}

					//brake
					{
						var next = new PathPoint();
						next.speed = current.speed - brake * scale;
						next.forward = current.forward.rotate(steering * steeringFactor * scale / next.speed);
						next.position = current.position + next.speed * next.forward * movementFactor * scale;

						next.prev = current;
						next.depth = current.depth + 1;


						if (!MyStrategy.map.isIntersectRoughly(Ray.line(current.position, next.position))) {
							queue.Enqueue(next);
						}

					}
				}

			}

			{
				var current = best;
				while (current != initial) {
					points.AddFirst(current.position);
					current = current.prev;
				}

				points.AddFirst(vehicle.position);
			}

		}

		public void draw(int color) {

			Vector last = Vector.up;
			bool paintLast = false;

			foreach (var point in points) {
				point.draw(color);

				if (!paintLast) {
					paintLast = true;
				} else {
					Ray.line(last, point).draw(color);
				}
				last = point;
			}

		}
	}
}

