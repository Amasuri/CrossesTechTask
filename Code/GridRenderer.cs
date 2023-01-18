using System;
using System.Collections.Generic;
using System.Text;

namespace CrossesTechTask.Code
{
    public static class GridRenderer
    {
        static public void Render(Grid grid, GameSession session)
        {
            var pointer = session.GetCurrentPlayer().Pointer;

            //Последовательный рендер игрового поля с учётом того, куда указывает текущий игрок сейчас
            for (int y = 0; y < grid.FieldSize; y++)
            {
                Console.Write("    ");
                for (int x = 0; x < grid.FieldSize; x++)
                {
                    //Если позиция прохода по сетке совпадает с позицией, куда указывает текущий игрок, то эту позицию надо подсветить
                    if (x == (int)pointer.X && y == (int)pointer.Y)
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.Write(grid.Field[x, y]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine();
            }
        }
    }
}
