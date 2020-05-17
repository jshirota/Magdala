using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magdala;

namespace UnitTest
{
    public static class TestHelper
    {
        public static float[] Values(this Grid grid)
            => grid.Rows.SelectMany(x => x).Select(x => x.Value).ToArray();

        public static void AssertThat(this Grid grid, Func<float?[], bool> predicate)
            => Assert.IsTrue(predicate(grid.Rows.SelectMany(x => x).ToArray()));

        public static void AssertThat(this Grid grid, Func<float?, bool> predicate)
            => grid.AssertThat(x => x.All(predicate));

        public static void AssertEqual(this Grid grid, float? value)
            => grid.AssertThat(x => x == value);

        public static void AssertEqual(this Grid grid1, Grid grid2)
            => grid1.Rows.SelectMany(x => x).SequenceEqual(grid2.Rows.SelectMany(x => x));
    }
}
