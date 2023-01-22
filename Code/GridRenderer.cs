using System;

namespace CrossesTechTask.Code
{
    public static class GridRenderer
    {
        /// <summary>
        /// Отрисовка текущего состояния игрового поля. Выводит символы первого, второго игрока и пустые клетки. Помечает цветом каждого игрока,
        /// для удобства. Также подсвечивает текущую выбранную игроком клетку
        /// </summary>
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

                    //Подсветка символов определенного типа для удобства
                    else if (grid.Field[x, y] == Grid.CrossChar)
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else if (grid.Field[x, y] == Grid.CircleChar)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write(grid.Field[x, y] + " ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine();
            }
        }
    }
}
