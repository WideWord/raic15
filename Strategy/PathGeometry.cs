using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	class PathGeometry {

		private List<Ray> leftRays;
		private List<Ray> rightRays;
		private List<Circle> leftCircles;
		private List<Circle> rightCircles;

		public PathGeometry(Tile current, TilePath path) {

			

		}

		public void Draw() {

			const int leftColor = 0x00FF00;
			const int rightColor = 0xFF0000;

			foreach (var ray in leftRays) {
				ray.Draw(leftColor);
			}
			foreach (var circle in leftCircles) {
				circle.Draw(leftColor);
			}

			foreach (var ray in rightRays) {
				ray.Draw(rightColor);
			}

			foreach (var circle in rightCircles) {
				circle.Draw(rightColor);
			}
		}

	}
	

}
