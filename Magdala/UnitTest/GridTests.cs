using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magdala;

namespace UnitTest
{
    [TestClass]
    public class GridTests
    {
        [TestMethod]
        public void Info()
        {
            var dem = new Grid(@"Data\dem.tif");

            Assert.AreEqual(3601, dem.Info.Width);
            Assert.AreEqual(3601, dem.Info.Height);
            Assert.AreEqual(@"GEOGCS[""WGS 84"",DATUM[""WGS_1984"",SPHEROID[""WGS 84"",6378137,298.257223563,AUTHORITY[""EPSG"",""7030""]],AUTHORITY[""EPSG"",""6326""]],PRIMEM[""Greenwich"",0],UNIT[""degree"",0.0174532925199433],AUTHORITY[""EPSG"",""4326""]]", dem.Info.Projection);

            var life = new Grid(@"Data\life.tif");

            Assert.AreEqual(50, life.Info.Width);
            Assert.AreEqual(35, life.Info.Height);
            Assert.AreEqual("", life.Info.Projection);
        }

        [TestMethod]
        public void Gosper()
        {
            static Grid Tick(Grid grid)
            {
                var g = grid.FocalSum(1) - grid;
                return grid == 1 & g == 2 | g == 3;
            }

            var life = new Grid(@"Data\life.tif");
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
