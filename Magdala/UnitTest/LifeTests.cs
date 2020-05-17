using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magdala;

namespace UnitTest
{
    [TestClass]
    public class LifeTests
    {
        private static readonly Grid life = new Grid(@"Data\life.tif");

        [TestMethod]
        public void Info()
        {
            Assert.AreEqual(50, life.Info.Width);
            Assert.AreEqual(35, life.Info.Height);
            Assert.AreEqual("", life.Info.Projection);
        }

        [TestMethod]
        public void Gosper()
        {
            var grid = life;
            var iterations = 1000;

            // Fill the canvas.
            for (var i = 0; i < 100; i++)
                grid = Tick(grid);

            // Capture the initial sequence.
            var initialState = grid.Rows.SelectMany(x => x).ToArray();

            var actual = new List<int>();

            for (var i = 0; i < iterations; i++)
            {
                grid = Tick(grid);

                if (grid.Rows.SelectMany(x => x).SequenceEqual(initialState))
                    actual.Add(i);
            }

            // Gosper Glider Gun repeats every 60 ticks.
            var expected = Enumerable.Range(1, int.MaxValue)
                .Select(x => x * 60 - 1)
                .TakeWhile(x => x < iterations)
                .ToList();

            Assert.IsTrue(actual.SequenceEqual(expected));
        }

        private static Grid Tick(Grid grid)
        {
            var g = grid.FocalSum(1) - grid;
            return grid == 1 & g == 2 | g == 3;
        }

        [TestMethod]
        public void Indexers()
        {
            Assert.AreEqual(null, life[int.MinValue, int.MinValue]);
            Assert.AreEqual(null, life[int.MaxValue, int.MaxValue]);
            Assert.AreEqual(null, life[-1, -1]);
            Assert.AreEqual(0, life[0, 0]);
            Assert.AreEqual(0, life[1, 1]);
            Assert.AreEqual(0, life[life.Info.Width - 1, life.Info.Height - 1]);
            Assert.AreEqual(null, life[life.Info.Width, life.Info.Height]);
        }

        [TestMethod]
        public void Add()
        {
            var grid1 = life;
            var grid2 = grid1 + 1;
            var grid3 = -3 + grid1;
            var grid4 = grid1 + grid3;

            Assert.IsTrue(grid1.Values().Select(x => x + 1).SequenceEqual(grid2.Values()));
            Assert.IsTrue(grid1.Values().Select(x => -3 + x).SequenceEqual(grid3.Values()));
            Assert.IsTrue(grid1.Values().Zip(grid3.Values(), (x, y) => x + y).SequenceEqual(grid4.Values()));
        }

        [TestMethod]
        public void Subtract()
        {
            var grid1 = life;
            var grid2 = grid1 - 4;
            var grid3 = 10 - grid1;
            var grid4 = grid1 - grid3;
            var grid5 = -life + life;

            Assert.IsTrue(grid1.Values().Select(x => x - 4).SequenceEqual(grid2.Values()));
            Assert.IsTrue(grid1.Values().Select(x => 10 - x).SequenceEqual(grid3.Values()));
            Assert.IsTrue(grid1.Values().Zip(grid3.Values(), (x, y) => x - y).SequenceEqual(grid4.Values()));
            Assert.IsTrue(grid5.Rows.SelectMany(x => x).All(x => x == 0));
        }

        [TestMethod]
        public void Multiply()
        {
            var grid1 = life;
            var grid2 = grid1 * 9;
            var grid3 = 7 * grid1;
            var grid4 = grid1 * grid2;

            Assert.IsTrue(grid1.Values().Select(x => x * 9).SequenceEqual(grid2.Values()));
            Assert.IsTrue(grid1.Values().Select(x => 7 * x).SequenceEqual(grid3.Values()));
            Assert.IsTrue(grid1.Values().Zip(grid2.Values(), (x, y) => x * y).SequenceEqual(grid4.Values()));
        }

        [TestMethod]
        public void Divide()
        {
            var grid1 = life;
            var grid2 = grid1 / 9;
            var grid3 = 7 / grid1;
            var grid4 = grid1 / grid2;

            Assert.IsTrue(grid1.Values().Select(x => x / 9).SequenceEqual(grid2.Values()));
            Assert.IsTrue(grid1.Values().Select(x => 7 / x).SequenceEqual(grid3.Values()));
            Assert.IsTrue(grid1.Values().Zip(grid2.Values(), (x, y) => x / y).SequenceEqual(grid4.Values()));
        }

        [TestMethod]
        public void Mod()
        {
            var grid1 = life;
            var grid2 = (grid1 * 123) % 7;
            var grid3 = 3456 % (grid1 * 13);
            var grid4 = grid1 % grid2;

            Assert.IsTrue(grid1.Values().Select(x => (x * 123) % 7).SequenceEqual(grid2.Values()));
            Assert.IsTrue(grid1.Values().Select(x => 3456 % (x * 13)).SequenceEqual(grid3.Values()));
            Assert.IsTrue(grid1.Values().Zip(grid2.Values(), (x, y) => x % y).SequenceEqual(grid4.Values()));
        }

        [TestMethod]
        public void EqualTo()
        {
            var grid1 = life == (life * 1);
            var grid2 = life == life * 1;
            var grid3 = life + (life == 0);
            var grid4 = life + (0 == life);

            grid1.AssertEqual(1);
            grid2.AssertEqual(1);
            grid3.AssertEqual(1);
            grid4.AssertEqual(1);
        }

        [TestMethod]
        public void NotEqualTo()
        {
            var grid1 = life != (life * 1);
            var grid2 = (life - 1) != life;
            var grid3 = life * 0 != 1;
            var grid4 = 1 != (life * 0 + 1);

            grid1.AssertEqual(0);
            grid2.AssertEqual(1);
            grid3.AssertEqual(1);
            grid4.AssertEqual(0);
        }

        [TestMethod]
        public void GreaterThan()
        {
            var grid1 = (life * 0 + 1) > (life * 0);
            var grid2 = (life * 0 + 1) > 0;
            var grid3 = 0 > (life * 0 + 1);

            grid1.AssertEqual(1);
            grid2.AssertEqual(1);
            grid3.AssertEqual(0);
        }

        [TestMethod]
        public void GreaterThanOrEqualTo()
        {
            var grid1 = life.FocalAverage(1);
            var grid2 = (grid1 >= 0.222222224) + (grid1 < 0.222222224);
            var grid3 = life.FocalAverage(2);
            var grid4 = (grid1 >= grid3) + (grid1 < grid3);
            var grid5 = (0.222222224 >= grid1) + (0.222222224 < grid1);

            grid2.AssertEqual(1);
            grid4.AssertEqual(1);
            grid5.AssertEqual(1);
        }

        [TestMethod]
        public void LessThan()
        {
            var grid1 = (life * 0 + 1) < (life * 0);
            var grid2 = (life * 0 + 1) < 0;
            var grid3 = 0 < (life * 0 + 1);

            grid1.AssertEqual(0);
            grid2.AssertEqual(0);
            grid3.AssertEqual(1);
        }

        [TestMethod]
        public void LessThanOrEqualTo()
        {
            var grid1 = life.FocalAverage(1);

            var grid2 = (grid1 <= 0.222222224) + (grid1 > 0.222222224);
            var grid3 = (grid1 <= grid2) + (grid1 > grid2);
            var grid4 = (0.222222224 <= grid1) + (0.222222224 > grid1);

            grid2.AssertEqual(1);
            grid3.AssertEqual(1);
            grid4.AssertEqual(1);
        }

        [TestMethod]
        public void Negation()
        {
            var grid1 = life;
            var grid2 = !life;
            var grid3 = grid1 == grid2;

            grid3.AssertEqual(0);
        }

        [TestMethod]
        public void Local()
        {
            var grid1 = Grid.Local(life, x => x > 0.5 ? 0 : x);
            var grid2 = life.Local(x => x > 0.5 ? 0 : x);

            grid1.AssertEqual(0);
            grid2.AssertEqual(0);
        }

        [TestMethod]
        public void Local2()
        {
            var grid1 = Grid.Local(life, -1 * life, (x, y) => x + y);

            grid1.AssertEqual(0);
        }

        [TestMethod]
        public void Con()
        {
            var grid1 = Grid.Con(life == 0, 1, 0);
            var grid2 = life + grid1;
            var grid3 = Grid.Con(life > 0, life, 0) == life;
            var grid4 = Grid.Con(life < 1, 0, 1) == life;
            var grid5 = Grid.Con(life < 0, 0, life) == life;

            grid2.AssertEqual(1);
            grid3.AssertEqual(1);
            grid4.AssertEqual(1);
            grid5.AssertEqual(1);
        }
    }
}
