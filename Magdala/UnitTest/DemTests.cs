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
        private readonly float[] values;

        public DemTests()
        {
            this.values = (from x in new[] { x - 1, x, x + 1 }
                           from y in new[] { y - 1, y, y + 1 }
                           select this.dem.Rows[x][y]).ToArray();
        }

        [TestMethod]
        public void Info()
        {
            Assert.AreEqual(3601, this.dem.Info.Width);
            Assert.AreEqual(3601, this.dem.Info.Height);
            Assert.AreEqual(@"GEOGCS[""WGS 84"",DATUM[""WGS_1984"",SPHEROID[""WGS 84"",6378137,298.257223563,AUTHORITY[""EPSG"",""7030""]],AUTHORITY[""EPSG"",""6326""]],PRIMEM[""Greenwich"",0],UNIT[""degree"",0.0174532925199433],AUTHORITY[""EPSG"",""4326""]]", dem.Info.Projection);
        }

        [TestMethod]
        public void FocalAverage()
        {
            Assert.AreEqual(this.values.Average(), this.dem.FocalAverage(1).Rows[x][y]);
        }

        [TestMethod]
        public void FocalMax()
        {
            Assert.AreEqual(this.values.Max(), this.dem.FocalMax(1).Rows[x][y]);
        }

        [TestMethod]
        public void FocalMin()
        {
            Assert.AreEqual(this.values.Min(), this.dem.FocalMin(1).Rows[x][y]);
        }

        [TestMethod]
        public void FocalSum()
        {
            Assert.AreEqual(this.values.Sum(), this.dem.FocalSum(1).Rows[x][y]);
        }
    }
}
