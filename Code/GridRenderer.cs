using System;
using System.Collections.Generic;
using System.Text;

namespace CrossesTechTask.Code
{
    public static class GridRenderer
    {
        static public void RenderSimple(Grid grid)
        {
            for (int y = 0; y < grid.FieldSize; y++)
            {
                string line = "    ";
                for (int x = 0; x < grid.FieldSize; x++)
                {
                    line += grid.Field[x, y];
                }
                Console.WriteLine(line);
            }
        }
    }
}
