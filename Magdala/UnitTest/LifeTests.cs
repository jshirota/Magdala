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
    }
}
