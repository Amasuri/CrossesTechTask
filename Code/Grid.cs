using System;
using System.Collections.Generic;
using System.Text;

namespace CrossesTechTask.Code
{
    public class Grid
    {
        public int FieldSize { get; private set; }
        public char[,] Field { get; private set; }

        /// <summary>
        /// Инициализирует пустое поле заданного размера
        /// </summary>
        public void SetField(int size)
        {
            Field = new char[size, size];
            FieldSize = size;
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    Field[x, y] = '*';
                }
        }

        /// <summary>
        /// Пробует поставить Х на поле. Если это возможно, делает и возвращает true. Если нет, то возвращает false.
        /// </summary>
        public bool TryPutCrossOnField(int x, int y)
        {
            return TryPutCharOnField(x, y, 'X');
        }

        /// <summary>
        /// Пробует поставить O на поле. Если это возможно, делает и возвращает true. Если нет, то возвращает false.
        /// </summary>
        public bool TryPutNoughtOnField(int x, int y)
        {
            return TryPutCharOnField(x, y, 'O');
        }

        /// <summary>
        /// Внутренний метод для переиспользования кода. Основа для TryPutCrossOnField и TryPutNoughtOnField.
        /// </summary>
        private bool TryPutCharOnField(int x, int y, char chr)
        {
            //Проверка на корректность значений
            if (x < 0 || x >= FieldSize)
                return false;
            if (y < 0 || y >= FieldSize)
                return false;

            //Проверка на то, что данная ячейка уже занята. Если нет, то ставится символ
            if (Field[x, y] == '*')
            {
                Field[x, y] = chr;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
