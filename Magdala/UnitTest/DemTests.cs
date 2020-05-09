using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magdala;

namespace UnitTest
{
    [TestClass]
    public class DemTests
    {
        private readonly Grid dem = new Grid(@"Data\dem.tif");
        private readonly int x = 1234;
        private readonly int y = 2345;
        private readonly float?[] values;

        public DemTests()
        {
            this.values = (from x in new[] { x - 1, x, x + 1 }
                           from y in new[] { y - 1, y, y + 1 }
                           select this.dem[x, y]).ToArray();
        }

        [TestMethod]
        public void Info()
        {
            Assert.AreEqual(3601, this.dem.Info.Width);
            Assert.AreEqual(3601, this.dem.Info.Height);
            Assert.AreEqual(34.99986111111111, this.dem.Info.Origin.X, 0.000001);
            Assert.AreEqual(33.00013888888889, this.dem.Info.Origin.Y, 0.000001);
            Assert.AreEqual(0.0002777777777777778, this.dem.Info.PixelSize.X, 0.000001);
            Assert.AreEqual(-0.0002777777777777778, this.dem.Info.PixelSize.Y, 0.000001);
            Assert.AreEqual(34.999861111111109, this.dem.Info.Extent.Xmin, 0.000001);
            Assert.AreEqual(31.999861111111112, this.dem.Info.Extent.Ymin, 0.000001);
            Assert.AreEqual(36.000138888888884, this.dem.Info.Extent.Xmax, 0.000001);
            Assert.AreEqual(33.000138888888891, this.dem.Info.Extent.Ymax, 0.000001);
            Assert.AreEqual(@"GEOGCS[""WGS 84"",DATUM[""WGS_1984"",SPHEROID[""WGS 84"",6378137,298.257223563,AUTHORITY[""EPSG"",""7030""]],AUTHORITY[""EPSG"",""6326""]],PRIMEM[""Greenwich"",0],UNIT[""degree"",0.0174532925199433],AUTHORITY[""EPSG"",""4326""]]", dem.Info.Projection);
        }

        [TestMethod]
        public void Stats()
        {
            Assert.AreEqual(301.790375, this.dem.Average, 0.001);
            Assert.AreEqual(1246, this.dem.Max);
            Assert.AreEqual(-373, this.dem.Min);
            Assert.AreEqual(1619, this.dem.Range);
            Assert.AreEqual(319.03653, this.dem.StDev, 0.001);
        }

        [TestMethod]
        public void IndexSomewhere()
        {
            Assert.AreEqual(559, this.dem[35.7108081, 32.3941722]);
        }

        [TestMethod]
        public void IndexBottomLeft()
        {
            Assert.AreEqual(210, this.dem[34.9998637, 31.9998650]);
        }

        [TestMethod]
        public void IndexBottomRight()
        {
            Assert.AreEqual(730, this.dem[36.0001274, 31.9998709]);
        }

        [TestMethod]
        public void IndexTopLeft()
        {
            Assert.AreEqual(0, this.dem[34.9999788, 33.0000638]);
        }

        [TestMethod]
        public void IndexTopRight()
        {
            Assert.AreEqual(673, this.dem[36.0001055, 33.0001062]);
        }

        [TestMethod]
        public void FocalAverage()
        {
            Assert.AreEqual(this.values.Average(), this.dem.FocalAverage(1)[x, y]);
        }

        [TestMethod]
        public void FocalMax()
        {
            Assert.AreEqual(this.values.Max(), this.dem.FocalMax(1)[x, y]);
        }

        [TestMethod]
        public void FocalMin()
        {
            Assert.AreEqual(this.values.Min(), this.dem.FocalMin(1)[x, y]);
        }

        [TestMethod]
        public void FocalStDev()
        {
            var mean = values.Average();
            var stDev = Math.Sqrt(values.Sum(x => (x - mean) * (x - mean)).Value / values.Length);

            Assert.AreEqual(stDev, (double)this.dem.FocalStDev(1)[x, y], 0.001);
        }

        [TestMethod]
        public void FocalSum()
        {
            Assert.AreEqual(this.values.Sum(), this.dem.FocalSum(1)[x, y]);
        }
    }
}
