using System;
using System.Collections.Generic;
using System.Linq;
using OSGeo.GDAL;

namespace Magdala
{
    /// <summary>
    /// Map algebra grid.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Gets the grid metadata.
        /// </summary>
        public GridInfo Info { get; }

        /// <summary>
        /// Gets the grid rows.
        /// </summary>
        public int[][] Rows { get; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        /// <param name="file">Input file.</param>
        public Grid(string file)
        {
            using (var dataset = Gdal.Open(file, Access.GA_ReadOnly))
            {
                this.Info = new GridInfo(dataset);
                this.Rows = Readrows(dataset).ToArray();
            }
        }

        internal Grid(GridInfo info, IEnumerable<IEnumerable<int>> rows)
        {
            this.Info = info;
            this.Rows = rows.Select(x => x.ToArray()).ToArray();
        }

        #endregion

        #region Operators

        /// <summary>
        /// Addition operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator +(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x + y);

        /// <summary>
        /// Addition operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator +(Grid grid, int n) => Local(grid, x => x + n);

        /// <summary>
        /// Addition operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator +(int n, Grid grid) => Local(grid, x => n + x);

        /// <summary>
        /// Subtraction operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator -(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x - y);

        /// <summary>
        /// Subtraction operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator -(Grid grid, int n) => Local(grid, x => x - n);

        /// <summary>
        /// Subtraction operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator -(int n, Grid grid) => Local(grid, x => n - x);

        /// <summary>
        /// Multiplication operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator *(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x * y);

        /// <summary>
        /// Multiplication operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator *(Grid grid, int n) => Local(grid, x => x * n);

        /// <summary>
        /// Multiplication operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator *(int n, Grid grid) => Local(grid, x => n * x);

        /// <summary>
        /// Division operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator /(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x / y);

        /// <summary>
        /// Division operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator /(Grid grid, int n) => Local(grid, x => x / n);

        /// <summary>
        /// Division operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator /(int n, Grid grid) => Local(grid, x => n / x);

        /// <summary>
        /// Equality operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator ==(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x == y ? 1 : 0);

        /// <summary>
        /// Equality operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator ==(Grid grid, int n) => Local(grid, x => x == n ? 1 : 0);

        /// <summary>
        /// Equality operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator ==(int n, Grid grid) => Local(grid, x => n == x ? 1 : 0);

        /// <summary>
        /// Inequality operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator !=(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x != y ? 1 : 0);

        /// <summary>
        /// Inequality operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator !=(Grid grid, int n) => Local(grid, x => x != n ? 1 : 0);

        /// <summary>
        /// Inequality operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator !=(int n, Grid grid) => Local(grid, x => n != x ? 1 : 0);

        /// <summary>
        /// Greater than operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator >(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x > y ? 1 : 0);

        /// <summary>
        /// Greater than operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator >(Grid grid, int n) => Local(grid, x => x > n ? 1 : 0);

        /// <summary>
        /// Greater than operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator >(int n, Grid grid) => Local(grid, x => n > x ? 1 : 0);

        /// <summary>
        /// Less than operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator <(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x < y ? 1 : 0);

        /// <summary>
        /// Less than operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator <(Grid grid, int n) => Local(grid, x => x < n ? 1 : 0);

        /// <summary>
        /// Less than operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator <(int n, Grid grid) => Local(grid, x => n < x ? 1 : 0);

        /// <summary>
        /// Greater than or equal to operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator >=(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x >= y ? 1 : 0);

        /// <summary>
        /// Greater than or equal to operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator >=(Grid grid, int n) => Local(grid, x => x >= n ? 1 : 0);

        /// <summary>
        /// Greater than or equal to operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator >=(int n, Grid grid) => Local(grid, x => n >= x ? 1 : 0);

        /// <summary>
        /// Less than or equal to operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator <=(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x <= y ? 1 : 0);

        /// <summary>
        /// Less than or equal to operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator <=(Grid grid, int n) => Local(grid, x => x <= n ? 1 : 0);

        /// <summary>
        /// Less than or equal to operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator <=(int n, Grid grid) => Local(grid, x => n <= x ? 1 : 0);

        /// <summary>
        /// Modulus operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator %(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x % y);

        /// <summary>
        /// Modulus operation.
        /// </summary>
        /// <param name="grid">Grid.</param>
        /// <param name="n">Number.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator %(Grid grid, int n) => Local(grid, x => x % n);

        /// <summary>
        /// Modulus operation.
        /// </summary>
        /// <param name="n">Number.</param>
        /// <param name="grid">Grid.</param>
        /// <returns>Output grid.</returns>
        public static Grid operator %(int n, Grid grid) => Local(grid, x => n % x);

        /// <summary>
        /// Logical "AND" operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Boolean grid.</returns>
        public static Grid operator &(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x == 1 && y == 1 ? 1 : 0);

        /// <summary>
        /// Logical "OR" operation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <returns>Boolean grid.</returns>
        public static Grid operator |(Grid grid1, Grid grid2) => Local(grid1, grid2, (x, y) => x == 1 || y == 1 ? 1 : 0);

        #endregion

        #region Local

        /// <summary>
        /// Provides local transformation.
        /// </summary>
        /// <param name="func">Transformation.</param>
        /// <returns>Transformed grid.</returns>
        public Grid Local(Func<int, int> func)
        {
            return Local(this, func);
        }

        /// <summary>
        /// Provides local transformation.
        /// </summary>
        /// <param name="grid">Input grid.</param>
        /// <param name="func">Transformation.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Local(Grid grid, Func<int, int> func)
        {
            return new Grid(grid.Info, grid.Rows.Select(x => x.Select(func)));
        }

        /// <summary>
        /// Provides local transformation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <param name="func">Transformation.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Local(Grid grid1, Grid grid2, Func<int, int, int> func)
        {
            return new Grid(grid1.Info, grid1.Rows.Zip(grid2.Rows, (x, y) => x.Zip(y, func)));
        }

        /// <summary>
        /// Provides local transformation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <param name="grid3">Third grid.</param>
        /// <param name="func">Transformation.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Local(Grid grid1, Grid grid2, Grid grid3, Func<int, int, int, int> func)
        {
            var rows = grid1.Rows
                .Zip(grid2.Rows, (x, y) => x.Zip(y, (a, b) => (a, b)))
                .Zip(grid3.Rows, (x, y) => x.Zip(y, (a, b) => func(a.a, a.b, b)));

            return new Grid(grid1.Info, rows);
        }

        /// <summary>
        /// Provides conditional transformation.
        /// </summary>
        /// <param name="predicate">Predicate grid.</param>
        /// <param name="trueGrid">Value returned when the predicate evaluates to true.</param>
        /// <param name="falseGrid">Value returned when the predicate evaluates to false.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Con(Grid predicate, Grid trueGrid, Grid falseGrid)
        {
            return Local(predicate, trueGrid, falseGrid, (x, y, z) => x == 1 ? y : z);
        }

        /// <summary>
        /// Provides conditional transformation.
        /// </summary>
        /// <param name="predicate">Predicate grid.</param>
        /// <param name="trueGrid">Value returned when the predicate evaluates to true.</param>
        /// <param name="falseValue">Value returned when the predicate evaluates to false.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Con(Grid predicate, Grid trueGrid, int falseValue)
        {
            return Local(predicate, trueGrid, (x, y) => x == 1 ? y : falseValue);
        }

        /// <summary>
        /// Provides conditional transformation.
        /// </summary>
        /// <param name="predicate">Predicate grid.</param>
        /// <param name="trueValue">Value returned when the predicate evaluates to true.</param>
        /// <param name="falseGrid">Value returned when the predicate evaluates to false.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Con(Grid predicate, int trueValue, Grid falseGrid)
        {
            return Local(predicate, falseGrid, (x, y) => x == 1 ? trueValue : y);
        }

        /// <summary>
        /// Provides conditional transformation.
        /// </summary>
        /// <param name="predicate">Predicate grid.</param>
        /// <param name="trueValue">Value returned when the predicate evaluates to true.</param>
        /// <param name="falseValue">Value returned when the predicate evaluates to false.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Con(Grid predicate, int trueValue, int falseValue)
        {
            return Local(predicate, x => x == 1 ? trueValue : falseValue);
        }

        #endregion

        #region Focal

        /// <summary>
        /// Provides focal transformation.
        /// </summary>
        /// <param name="size">Size (buffer).</param>
        /// <param name="func">Transformation.</param>
        /// <param name="circle">If set to true, excludes cells outside of the radius.</param>
        /// <returns>Transformed grid.</returns>
        public Grid Focal(byte size, Func<int[], int> func, bool circle = false)
        {
            return Focal(this, size, func, circle);
        }

        /// <summary>
        /// Provides focal transformation.
        /// </summary>
        /// <param name="grid">Input grid.</param>
        /// <param name="size">Size (buffer).</param>
        /// <param name="func">Transformation.</param>
        /// <param name="circle">If set to true, excludes cells outside of the radius.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Focal(Grid grid, byte size, Func<int[], int> func, bool circle = false)
        {
            return Focal(grid, Neighbourhood(size, circle), func);
        }

        /// <summary>
        /// Provides focal transformation.
        /// </summary>
        /// <param name="grid">Input grid.</param>
        /// <param name="neighbourhood">Neighbourhood.</param>
        /// <param name="func">Transformation.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Focal(Grid grid, (sbyte dx, sbyte dy)[] neighbourhood, Func<int[], int> func)
        {
            return new Grid(grid.Info, Focal(grid, neighbourhood).Select(x => x.Select(func)));
        }

        /// <summary>
        /// Provides focal transformation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <param name="size">Size (buffer).</param>
        /// <param name="func">Transformation.</param>
        /// <param name="circle">If set to true, excludes cells outside of the radius.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Focal(Grid grid1, Grid grid2, byte size, Func<int[], int[], int> func, bool circle = false)
        {
            return Focal(grid1, grid2, Neighbourhood(size, circle), func);
        }

        /// <summary>
        /// Provides focal transformation.
        /// </summary>
        /// <param name="grid1">First grid.</param>
        /// <param name="grid2">Second grid.</param>
        /// <param name="neighbourhood">Neighbourhood.</param>
        /// <param name="func">Transformation.</param>
        /// <returns>Transformed grid.</returns>
        public static Grid Focal(Grid grid1, Grid grid2, (sbyte dx, sbyte dy)[] neighbourhood, Func<int[], int[], int> func)
        {
            return new Grid(grid1.Info, Focal(grid1, neighbourhood).Zip(Focal(grid2, neighbourhood), (x, y) => x.Zip(y, func)));
        }

        private static (sbyte dx, sbyte dy)[] Neighbourhood(byte size, bool circle)
        {
            var sequence = Enumerable.Range(-size, size * 2 + 1).Select(i => (sbyte)i).ToArray();

            return (from dx in sequence
                    from dy in sequence
                    where !circle || Math.Pow(dx, 2) + Math.Pow(dy, 2) <= Math.Pow(size, 2)
                    select (dx, dy)).ToArray();
        }

        private static IEnumerable<int[][]> Focal(Grid grid, (sbyte dx, sbyte dy)[] neighbourhood)
        {
            int[][] Blocks(int h)
            {
                var blocks = new List<int[]>();

                for (var w = 0; w < grid.Info.Width; w++)
                {
                    var block = new List<int>();

                    foreach (var (dx, dy) in neighbourhood)
                    {
                        var x = w + dx;

                        if (x > -1 && x < grid.Info.Width)
                        {
                            var y = h + dy;

                            if (y > -1 && y < grid.Info.Height)
                            {
                                block.Add(grid.Rows[y][x]);
                            }
                        }
                    }

                    blocks.Add(block.ToArray());
                }

                return blocks.ToArray();
            }

            return ParallelEnumerable.Range(0, grid.Info.Height).AsOrdered().Select(Blocks);
        }

        #endregion

        #region IO

        private static IEnumerable<int[]> Readrows(Dataset dataset)
        {
            var band = dataset.GetRasterBand(1);
            var width = band.XSize;
            var height = band.YSize;

            for (var h = 0; h < height; h++)
            {
                var row = new int[width];
                band.ReadRaster(0, h, width, 1, row, width, 1, 0, 0);
                yield return row;
            }
        }

        /// <summary>
        /// Saves the grid.
        /// </summary>
        /// <param name="file">File name.</param>
        public void Save(string file)
        {
            var width = this.Info.Width;
            var height = this.Info.Height;

            using (var dataset = Gdal.GetDriverByName("GTiff").Create(file, width, height, 1, this.Info.DataType, null))
            {
                dataset.SetGeoTransform(this.Info.GeoTransform);
                dataset.SetProjection(this.Info.Projection);

                var band = dataset.GetRasterBand(1);

                var h = 0;

                foreach (var row in this.Rows)
                {
                    band.WriteRaster(0, h++, width, 1, row, width, 1, 0, 0);
                }

                dataset.FlushCache();
            }
        }

        #endregion

        ///<inheritdoc/>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            return this.ToString(10, 10);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <param name="xSize">X size.</param>
        /// <param name="ySize">Y size.</param>
        /// <param name="xOff">X offset.</param>
        /// <param name="yOff">Y offset.</param>
        /// <param name="replace">Replace function.</param>
        /// <returns>A string that represents the current object.</returns>
        public string ToString(int xSize, int ySize, int xOff = 0, int yOff = 0, Func<int, string> replace = null)
        {
            return string.Join("\r\n", this.Rows.Skip(yOff).Take(ySize).Select(x =>
                string.Join(" ", x.Skip(xOff).Take(xSize).Select(replace ?? (y => y.ToString())))));
        }
    }

    /// <summary>
    /// Grid metadata.
    /// </summary>
    public class GridInfo
    {
        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the data type.
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// Gets the geo transform.
        /// </summary>
        public double[] GeoTransform { get; }

        /// <summary>
        /// Gets the projection.
        /// </summary>
        public string Projection { get; }

        internal GridInfo(Dataset dataset)
        {
            var band = dataset.GetRasterBand(1);
            var geoTransform = new double[6];

            dataset.GetGeoTransform(geoTransform);

            this.Width = band.XSize;
            this.Height = band.YSize;
            this.DataType = band.DataType;
            this.GeoTransform = geoTransform;
            this.Projection = dataset.GetProjection();
        }
    }
}
