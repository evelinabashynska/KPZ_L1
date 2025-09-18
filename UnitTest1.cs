using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using EvelinaBashynska;

namespace EvelinaBashynska.Tests
{
    [TestClass]
    public class DistanceHelperTests
    {
        [TestMethod]
        public void FindDistance_SamePosition_ReturnsZero()
        {
            // Arrange
            var positionA = new Position(5, 5);
            var positionB = new Position(5, 5);

            // Act
            int distance = DistanceHelper.FindDistance(positionA, positionB);

            // Assert
            Assert.AreEqual(0, distance);
        }

        [TestMethod]
        public void FindDistance_DifferentPositions_ReturnsCorrectSquaredDistance()
        {
            // Arrange
            var positionA = new Position(0, 0);
            var positionB = new Position(3, 4);

            // Act
            int distance = DistanceHelper.FindDistance(positionA, positionB);

            // Assert
            Assert.AreEqual(25, distance); // 3^2 + 4^2 = 9 + 16 = 25
        }

        [TestMethod]
        public void FindDistance_NegativeCoordinates_ReturnsCorrectSquaredDistance()
        {
            // Arrange
            var positionA = new Position(-2, -3);
            var positionB = new Position(1, 1);

            // Act
            int distance = DistanceHelper.FindDistance(positionA, positionB);

            // Assert
            Assert.AreEqual(25, distance); // (1-(-2))^2 + (1-(-3))^2 = 3^2 + 4^2 = 25
        }
    }

    [TestClass]
    public class EvelinaBashynskaAlgorithmTests
    {
        private EvelinaBashynskaAlgorithm algorithm;
        private Map map;
        private List<Robot.Common.Robot> robots;
        private Robot.Common.Robot movingRobot;

        [TestInitialize]
        public void Setup()
        {
            algorithm = new EvelinaBashynskaAlgorithm();

            // Створюємо карту з енергетичними станціями
            var stations = new List<EnergyStation>
            {
                new EnergyStation { Position = new Position(0, 0), Energy = 1000, RecoveryRate = 2 },
                new EnergyStation { Position = new Position(5, 5), Energy = 1000, RecoveryRate = 2 },
                new EnergyStation { Position = new Position(10, 0), Energy = 1000, RecoveryRate = 2 }
            };
            map = new Map { Stations = stations };

            // Створюємо роботів
            movingRobot = new Robot.Common.Robot
            {
                Position = new Position(2, 2),
                Energy = 100,
                OwnerName = "Evelina Bashynska"
            };

            robots = new List<Robot.Common.Robot> { movingRobot };
        }

        [TestMethod]
        public void Author_ReturnsCorrectName()
        {
            // Assert
            Assert.AreEqual("Evelina Bashynska", algorithm.Author);
        }

        [TestMethod]
        public void IsCellFree_EmptyCell_ReturnsTrue()
        {
            // Arrange
            var emptyPosition = new Position(1, 1);

            // Act
            bool isFree = algorithm.IsCellFree(emptyPosition, movingRobot, robots);

            // Assert
            Assert.IsTrue(isFree);
        }

        [TestMethod]
        public void IsCellFree_OccupiedByMovingRobot_ReturnsTrue()
        {
            // Act
            bool isFree = algorithm.IsCellFree(movingRobot.Position, movingRobot, robots);

            // Assert
            Assert.IsTrue(isFree);
        }

        [TestMethod]
        public void IsCellFree_OccupiedByFriendlyRobot_ReturnsFalse()
        {
            // Arrange
            var friendlyRobot = new Robot.Common.Robot
            {
                Position = new Position(3, 3),
                Energy = 50,
                OwnerName = "Evelina Bashynska"
            };
            robots.Add(friendlyRobot);

            // Act
            bool isFree = algorithm.IsCellFree(friendlyRobot.Position, movingRobot, robots);

            // Assert
            Assert.IsFalse(isFree);
        }

        [TestMethod]
        public void IsCellFree_OccupiedByEnemyRobot_ReturnsTrue()
        {
            // Arrange
            var enemyRobot = new Robot.Common.Robot
            {
                Position = new Position(3, 3),
                Energy = 50,
                OwnerName = "Enemy"
            };
            robots.Add(enemyRobot);

            // Act
            bool isFree = algorithm.IsCellFree(enemyRobot.Position, movingRobot, robots);

            // Assert
            Assert.IsTrue(isFree);
        }

        [TestMethod]
        public void IsCellNotUs_EmptyCell_ReturnsTrue()
        {
            // Arrange
            var emptyPosition = new Position(1, 1);

            // Act
            bool isNotUs = algorithm.IsCellNotUs(emptyPosition, movingRobot, robots);

            // Assert
            Assert.IsTrue(isNotUs);
        }

        [TestMethod]
        public void IsCellNotUs_OccupiedByFriendlyRobot_ReturnsFalse()
        {
            // Arrange
            var friendlyRobot = new Robot.Common.Robot
            {
                Position = new Position(3, 3),
                Energy = 50,
                OwnerName = "Evelina Bashynska"
            };
            robots.Add(friendlyRobot);

            // Act
            bool isNotUs = algorithm.IsCellNotUs(friendlyRobot.Position, movingRobot, robots);

            // Assert
            Assert.IsFalse(isNotUs);
        }

        [TestMethod]
        public void IsCellNotUs_OccupiedByEnemyRobot_ReturnsTrue()
        {
            // Arrange
            var enemyRobot = new Robot.Common.Robot
            {
                Position = new Position(3, 3),
                Energy = 50,
                OwnerName = "Enemy"
            };
            robots.Add(enemyRobot);

            // Act
            bool isNotUs = algorithm.IsCellNotUs(enemyRobot.Position, movingRobot, robots);

            // Assert
            Assert.IsTrue(isNotUs);
        }

        [TestMethod]
        public void FindNearestFreeStation_NoFreeStations_ReturnsNull()
        {
            // Arrange - блокуємо всі станції дружніми роботами
            var friendlyRobot1 = new Robot.Common.Robot { Position = new Position(0, 0), OwnerName = "Evelina Bashynska" };
            var friendlyRobot2 = new Robot.Common.Robot { Position = new Position(5, 5), OwnerName = "Evelina Bashynska" };
            var friendlyRobot3 = new Robot.Common.Robot { Position = new Position(10, 0), OwnerName = "Evelina Bashynska" };
            robots.AddRange(new[] { friendlyRobot1, friendlyRobot2, friendlyRobot3 });

            // Act
            Position nearestStation = algorithm.FindNearestFreeStation(movingRobot, map, robots);

            // Assert
            Assert.IsNull(nearestStation);
        }

        [TestMethod]
        public void FindNearestFreeStation_MultipleFreeStations_ReturnsNearestOne()
        {
            // Act
            Position nearestStation = algorithm.FindNearestFreeStation(movingRobot, map, robots);

            // Assert - найближча станція до позиції (2,2) має бути (0,0)
            Assert.IsNotNull(nearestStation);
            Assert.AreEqual(0, nearestStation.X);
            Assert.AreEqual(0, nearestStation.Y);
        }

        [TestMethod]
        public void FindTrueNearestFreeStation_AllStationsAvailable_ReturnsNearestOne()
        {
            // Act
            Position nearestStation = algorithm.FindTrueNearestFreeStation(movingRobot, map, robots);

            // Assert - найближча станція до позиції (2,2) має бути (0,0)
            Assert.IsNotNull(nearestStation);
            Assert.AreEqual(0, nearestStation.X);
            Assert.AreEqual(0, nearestStation.Y);
        }

        [TestMethod]
        public void IsStationFree_FreeStation_ReturnsTrue()
        {
            // Arrange
            var station = map.Stations.First();

            // Act
            bool isFree = algorithm.IsStationFree(station, movingRobot, robots);

            // Assert
            Assert.IsTrue(isFree);
        }

        [TestMethod]
        public void IsStationFree_OccupiedByFriendlyRobot_ReturnsFalse()
        {
            // Arrange
            var station = map.Stations.First();
            var friendlyRobot = new Robot.Common.Robot
            {
                Position = station.Position,
                OwnerName = "Evelina Bashynska"
            };
            robots.Add(friendlyRobot);

            // Act
            bool isFree = algorithm.IsStationFree(station, movingRobot, robots);

            // Assert
            Assert.IsFalse(isFree);
        }


        [TestMethod]
        public void DoStep_RobotNotOnStation_ReturnsMoveCommand()
        {
            // Arrange - робот не на станції
            movingRobot.Position = new Position(2, 2);

            // Act
            RobotCommand command = algorithm.DoStep(robots, 0, map);

            // Assert
            Assert.IsInstanceOfType(command, typeof(MoveCommand));
        }

        [TestMethod]
        public void DoStep_MoveCommand_HasValidNewPosition()
        {
            // Arrange
            movingRobot.Position = new Position(2, 2);
            movingRobot.Energy = 50;

            // Act
            RobotCommand command = algorithm.DoStep(robots, 0, map);

            // Assert
            Assert.IsInstanceOfType(command, typeof(MoveCommand));
            var moveCommand = (MoveCommand)command;
            Assert.IsNotNull(moveCommand.NewPosition);

            // Перевіряємо, що нова позиція відрізняється від поточної (якщо є куди рухатись)
            if (moveCommand.NewPosition.X != movingRobot.Position.X ||
                moveCommand.NewPosition.Y != movingRobot.Position.Y)
            {
                // Перевіряємо, що рух не витрачає більше енергії, ніж є у робота
                int energyNeeded = DistanceHelper.FindDistance(movingRobot.Position, moveCommand.NewPosition);
                Assert.IsTrue(energyNeeded <= movingRobot.Energy,
                    $"Рух вимагає {energyNeeded} енергії, але у робота тільки {movingRobot.Energy}");
            }
        }
    }
}