using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk
{

    public sealed class MyStrategy : IStrategy
    {

        public void Move(Car self, World world, Game game, Move move)
        {
            int nextWaypoint = self.NextWaypointIndex;          

            double x = self.NextWaypointX * game.TrackTileSize + game.TrackTileSize / 2;
            double y = self.NextWaypointY * game.TrackTileSize + game.TrackTileSize / 2;

            double angleToNextWaypoint = self.GetAngleTo(x, y);

            double speed = Math.Sqrt(Math.Pow(self.SpeedX, 2) * Math.Pow(self.SpeedY, 2));

            if (angleToNextWaypoint > Math.PI * 0.25 && speed > 20)
            {
                move.IsBrake = true;
            }
            else
            {
                move.IsBrake = false;
            }

            move.WheelTurn = angleToNextWaypoint;

            if (speed < 25)
            {
                move.EnginePower = 1;
            }
            else
            {
                move.EnginePower = 0;
            }
        }



    }


}