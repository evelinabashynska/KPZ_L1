using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvelinaBashynska
{
    public class DistanceHelper
    {
        public static int FindDistance(Position a, Position b)
        {
            return (int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

        }
    }

    public class EvelinaBashynskaAlgorithm : IRobotAlgorithm
    {
        public string Author => "Evelina Bashynska";

        public Position FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)
            {
                if (IsStationFree(station, movingRobot, robots))
                {
                    int d = DistanceHelper.FindDistance(station.Position, movingRobot.Position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = station;
                    }
                }
            }
            return nearest == null ? null : nearest.Position;
        }
        public Position FindTrueNearestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)
            {
                if (IsCellNotUs(station.Position, movingRobot, robots))
                {
                    int d = DistanceHelper.FindDistance(station.Position, movingRobot.Position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = station;
                    }
                }
            }
            return nearest == null ? null : nearest.Position;
        }
        public bool IsStationFree(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            return IsCellFree(station.Position, movingRobot, robots);
        }
        public bool IsCellFree(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {

            foreach (var robot in robots)
            {
                if (robot.Position == cell)
                {
                    if (cell == movingRobot.Position)
                    {
                        return true;
                    }
                    if (robot.OwnerName == movingRobot.OwnerName)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return true;
        }
        public bool IsCellNotUs(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {

            foreach (var robot in robots)
            {
                if (robot.Position == cell)
                {
                    if (robot.OwnerName == movingRobot.OwnerName)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return true;
        }

        private Position FindNextPositionToMove(Robot.Common.Robot myRobot, Position destination)
        {
            Position nextPosition = new Position();
            int distance = DistanceHelper.FindDistance(destination, myRobot.Position);
            if (distance * distance < myRobot.Energy)
            {
                nextPosition = destination.Copy();
            }
            else
            {
                // Обмежимо крок на основі наявної енергії
                int maxDistance = (int)Math.Floor(Math.Sqrt(myRobot.Energy)); // Максимальна кількість клітинок, яку робот може пройти

                int deltaX = destination.X - myRobot.Position.X;
                int deltaY = destination.Y - myRobot.Position.Y;

                // Рахуємо, як далеко можна пересунутись на осях
                int stepX = 0;
                int stepY = 0;

                if (Math.Abs(deltaX) > 0)
                {
                    stepX = Math.Min(Math.Abs(deltaX), maxDistance) * Math.Sign(deltaX);
                }

                if (Math.Abs(deltaY) > 0)
                {
                    stepY = Math.Min(Math.Abs(deltaY), maxDistance - Math.Abs(stepX)) * Math.Sign(deltaY);
                }

                // Створюємо нову позицію на основі розрахованих кроків
                nextPosition.X = myRobot.Position.X + stepX;
                nextPosition.Y = myRobot.Position.Y + stepY;
            }


            return nextPosition;
        }

        int movingIndex = 5;


        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            Robot.Common.Robot movingRobot = robots[robotToMoveIndex];
            Position stationPosition = FindNearestFreeStation(movingRobot, map, robots);

            if (stationPosition == movingRobot.Position)
            {
                int nearestFreeStationDistance = DistanceHelper.FindDistance(FindTrueNearestFreeStation(movingRobot, map, robots), map.FindFreeCell(stationPosition, robots));
                if (movingRobot.Energy >= (nearestFreeStationDistance / movingIndex) * movingIndex * movingIndex + (movingIndex * movingIndex - movingIndex) + 40)
                    return new CreateNewRobotCommand() { NewRobotEnergy = (nearestFreeStationDistance / movingIndex) * movingIndex + (movingIndex * movingIndex - movingIndex) + 40 };
                else
                    return new CollectEnergyCommand();
            }

            return new MoveCommand() { NewPosition = FindNextPositionToMove(movingRobot, FindTrueNearestFreeStation(movingRobot, map, robots)) };

        }

    }
}
