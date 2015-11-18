using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{

    public sealed class MyStrategy : IStrategy {

		private bool showed = false;

        public void Move(Car self, World world, Game game, Move move) {
			if (!showed) {
				Debug.beginPre();
				Debug.circle(new Vector(self.X, self.Y), 20, 0xFF0000);
				Debug.endPre();

				showed = true;
			}

        }
    }


}