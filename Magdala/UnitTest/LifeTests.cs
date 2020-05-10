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
        private readonly Grid life = new Grid(@"Data\life.tif");

        [TestMethod]
        public void Info()
        {
            Assert.AreEqual(50, this.life.Info.Width);
            Assert.AreEqual(35, this.life.Info.Height);
            Assert.AreEqual("", this.life.Info.Projection);
        }

        [TestMethod]
        public void Gosper()
        {
            static Grid Tick(Grid grid)
            {
                var g = grid.FocalSum(1) - grid;
                return grid == 1 & g == 2 | g == 3;
            }

            var life = this.life;
            var iterations = 1000;

            // Fill the canvas.
            for (var i = 0; i < 100; i++)
                life = Tick(life);

            // Capture the initial sequence.
            var initialState = life.Rows.SelectMany(x => x).ToArray();

            var actual = new List<int>();

            for (var i = 0; i < iterations; i++)
            {
                life = Tick(life);

                if (life.Rows.SelectMany(x => x).SequenceEqual(initialState))
                    actual.Add(i);
            }

            // Gosper Glider Gun repeats every 60 ticks.
            var expected = Enumerable.Range(1, int.MaxValue)
                .Select(x => x * 60 - 1)
                .TakeWhile(x => x < iterations)
                .ToList();

            Assert.IsTrue(actual.SequenceEqual(expected));
        }

        static float[] Values(Grid grid)
        {
            return grid.Rows.SelectMany(x => x).Select(x => x.Value).ToArray();
        }

        [TestMethod]
        public void Add()
        {
            var grid1 = this.life;
            var grid2 = grid1 + 1;
            var grid3 = -3 + grid1;
            var grid4 = grid1 + grid3;

            Assert.IsTrue(Values(grid1).Select(x => x + 1).SequenceEqual(Values(grid2)));
            Assert.IsTrue(Values(grid1).Select(x => -3 + x).SequenceEqual(Values(grid3)));
            Assert.IsTrue(Values(grid1).Zip(Values(grid3), (x, y) => x + y).SequenceEqual(Values(grid4)));
        }

        [TestMethod]
        public void Subtract()
        {
            var grid1 = this.life;
            var grid2 = grid1 - 4;
            var grid3 = 10 - grid1;
            var grid4 = grid1 - grid3;

            Assert.IsTrue(Values(grid1).Select(x => x - 4).SequenceEqual(Values(grid2)));
            Assert.IsTrue(Values(grid1).Select(x => 10 - x).SequenceEqual(Values(grid3)));
            Assert.IsTrue(Values(grid1).Zip(Values(grid3), (x, y) => x - y).SequenceEqual(Values(grid4)));
        }

        [TestMethod]
        public void Multiply()
        {
            var grid1 = this.life;
            var grid2 = grid1 * 9;
            var grid3 = 7 * grid1;
            var grid4 = grid1 * grid2;

            Assert.IsTrue(Values(grid1).Select(x => x * 9).SequenceEqual(Values(grid2)));
            Assert.IsTrue(Values(grid1).Select(x => 7 * x).SequenceEqual(Values(grid3)));
            Assert.IsTrue(Values(grid1).Zip(Values(grid2), (x, y) => x * y).SequenceEqual(Values(grid4)));
        }

        [TestMethod]
        public void Divide()
        {
            var grid1 = this.life;
            var grid2 = grid1 / 9;
            var grid3 = 7 / grid1;
            var grid4 = grid1 / grid2;

            Assert.IsTrue(Values(grid1).Select(x => x / 9).SequenceEqual(Values(grid2)));
            Assert.IsTrue(Values(grid1).Select(x => 7 / x).SequenceEqual(Values(grid3)));
            Assert.IsTrue(Values(grid1).Zip(Values(grid2), (x, y) => x / y).SequenceEqual(Values(grid4)));
        }

        [TestMethod]
        public void Mod()
        {
            var grid1 = this.life;
            var grid2 = (grid1 * 123) % 7;
            var grid3 = 3456 % (grid1 * 13);
            var grid4 = grid1 % grid2;

            Assert.IsTrue(Values(grid1).Select(x => (x * 123) % 7).SequenceEqual(Values(grid2)));
            Assert.IsTrue(Values(grid1).Select(x => 3456 % (x * 13)).SequenceEqual(Values(grid3)));
            Assert.IsTrue(Values(grid1).Zip(Values(grid2), (x, y) => x % y).SequenceEqual(Values(grid4)));
        }
    }
}
