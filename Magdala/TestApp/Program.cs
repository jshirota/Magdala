using System;
using Magdala;

namespace TestApp
{
    class Program
    {
        static void Main()
        {
            var gosper = new Grid("gosper.tif");

            for (var i = 0; i < 100; i++)
            {
                gosper = Tick(gosper);
                Console.WriteLine(gosper.ToString(xSize: 50, ySize: 30, replace: n => n == 0 ? " " : "*"));
            }

            Console.ReadLine();
        }

        static Grid Tick(Grid grid)
        {
            var g = grid.FocalSum(1) - grid;
            return grid == 1 & g == 2 | g == 3;
        }
    }
}
