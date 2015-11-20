using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{
	public static class Constants {

		public static readonly double turningSmoothCoef = 0.5;

		public static readonly double roadMargin = 80.0;

		public static double tileSize { get; private set; }

		public static void setConstants(Game game, World world) {
			tileSize = game.TrackTileSize;
		}

	}
}

