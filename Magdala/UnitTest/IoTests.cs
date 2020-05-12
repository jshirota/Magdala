using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magdala;

namespace UnitTest
{
    [TestClass]
    public class IoTests
    {
        private static readonly Grid life = new Grid(@"Data\life.tif");

        [TestMethod]
        public void Save()
        {
            var name = Path.GetTempFileName();

            life.Save(name);

            var grid = new Grid(name);

            Assert.IsTrue(grid.Rows.SelectMany(x => x).SequenceEqual(life.Rows.SelectMany(x => x)));
        }
    }
}
