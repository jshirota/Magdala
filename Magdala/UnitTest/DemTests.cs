using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magdala;

namespace UnitTest
{
    [TestClass]
    public class DemTests
    {
        private static readonly Grid dem = new Grid(@"Data\dem.tif");
        private static readonly int x = 123;
        private static readonly int y = 234;
        private static readonly float?[] values;

        static DemTests()
        {
            values = (from x in new[] { x - 1, x, x + 1 }
                      from y in new[] { y - 1, y, y + 1 }
                      select dem[x, y]).ToArray();
        }

        [TestMethod]
        public void Info()
        {
            Assert.AreEqual(1370, dem.Info.Width);
            Assert.AreEqual(898, dem.Info.Height);
            Assert.AreEqual(35.3418056, dem.Info.Origin.X, 0.000001);
            Assert.AreEqual(32.9215278, dem.Info.Origin.Y, 0.000001);
            Assert.AreEqual(0.000277777777777776, dem.Info.PixelSize.X, 0.000001);
            Assert.AreEqual(-0.000277777777777776, dem.Info.PixelSize.Y, 0.000001);
            Assert.AreEqual(35.3418056, dem.Info.Extent.Xmin, 0.000001);
            Assert.AreEqual(32.6720834, dem.Info.Extent.Ymin, 0.000001);
            Assert.AreEqual(35.7223611, dem.Info.Extent.Xmax, 0.000001);
            Assert.AreEqual(32.9215278, dem.Info.Extent.Ymax, 0.000001);
            Assert.AreEqual(@"GEOGCS[""WGS 84"",DATUM[""WGS_1984"",SPHEROID[""WGS 84"",6378137,298.257223563,AUTHORITY[""EPSG"",""7030""]],AUTHORITY[""EPSG"",""6326""]],PRIMEM[""Greenwich"",0],UNIT[""degree"",0.0174532925199433],AUTHORITY[""EPSG"",""4326""]]", dem.Info.Projection);
        }

        [TestMethod]
        public void Stats()
        {
            Assert.AreEqual(51.12, dem.Average, 0.01);
            Assert.AreEqual(599, dem.Max);
            Assert.AreEqual(-236, dem.Min);
            Assert.AreEqual(835, dem.Range);
            Assert.AreEqual(197.57, dem.StDev, 0.01);
        }

        [TestMethod]
        public void IndexSomewhere()
        {
            Assert.AreEqual(-161, dem[35.5073194, 32.8384840]);
        }

        [TestMethod]
        public void IndexBottomLeft()
        {
            Assert.AreEqual(116, dem[35.3418112, 32.6720886]);
            Assert.AreEqual(null, dem[35.3417971, 32.6720778]);
        }

        [TestMethod]
        public void IndexBottomRight()
        {
            Assert.AreEqual(210, dem[35.7222947, 32.6721425]);
            Assert.AreEqual(null, dem[35.7223901, 32.6720609]);
        }

        [TestMethod]
        public void IndexTopLeft()
        {
            Assert.AreEqual(316, dem[35.3418416, 32.9214985]);
            Assert.AreEqual(null, dem[35.3417397, 32.9215739]);
        }

        [TestMethod]
        public void IndexTopRight()
        {
            Assert.AreEqual(293, dem[35.7223433, 32.9215165]);
            Assert.AreEqual(null, dem[35.7223848, 32.9215456]);
        }

        [TestMethod]
        public void Focal()
        {
            static float? Mode(float?[] x) => x
                .Where(x => x.HasValue)
                .GroupBy(y => y)
                .OrderBy(x => x.Count())
                .Select(z => z.Key)
                .FirstOrDefault();

            Assert.AreEqual(Mode(values), dem.Focal(Mode, 1)[x, y]);
        }

        [TestMethod]
        public void FocalAverage()
        {
            Assert.AreEqual(values.Average(), dem.FocalAverage(1)[x, y]);
        }

        [TestMethod]
        public void FocalMax()
        {
            Assert.AreEqual(values.Max(), dem.FocalMax(1)[x, y]);
        }

        [TestMethod]
        public void FocalMin()
        {
            Assert.AreEqual(values.Min(), dem.FocalMin(1)[x, y]);
        }

        [TestMethod]
        public void FocalRange()
        {
            Assert.AreEqual(values.Max() - values.Min(), dem.FocalRange(1)[x, y]);
        }

        [TestMethod]
        public void FocalStDev()
        {
            var mean = values.Average();
            var stDev = Math.Sqrt(values.Sum(x => (x - mean) * (x - mean)).Value / values.Length);

            Assert.AreEqual(stDev, (double)dem.FocalStDev(1)[x, y], 0.001);
        }

        [TestMethod]
        public void FocalSum()
        {
            Assert.AreEqual(values.Sum(), dem.FocalSum(1)[x, y]);
        }
    }
}
