using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {

	public struct MoveInstruction {

		public double SteeringAngle;
		public double EnginePower;
		public bool Brake;
		public bool Nitro;

		public void Apply(Move move) {
			move.WheelTurn = SteeringAngle;
			move.EnginePower = EnginePower;
			move.IsBrake = Brake;
			move.IsUseNitro = Nitro;
		}

	}

}
