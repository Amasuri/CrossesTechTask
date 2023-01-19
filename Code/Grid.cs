using System;

namespace CrossesTechTask.Code
{
    public class Grid
    {
        private const char CrossChar = 'X';
        private const char CircleChar = 'O';

        public int FieldSize { get; private set; }
        public int FieldMaxIndex => FieldSize - 1;
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
            return TryPutCharOnField(x, y, CrossChar);
        }

        /// <summary>
        /// Пробует поставить O на поле. Если это возможно, делает и возвращает true. Если нет, то возвращает false.
        /// </summary>
        public bool TryPutNoughtOnField(int x, int y)
        {
            return TryPutCharOnField(x, y, CircleChar);
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

        /// <summary>
        /// Проверяет условия победы для заданного игрока: по вертикали, горизонтали и диагонали. В случае победы возвращает true, иначе false.
        /// </summary>
        public bool CheckWin(GameSession.TurnOf player)
        {
            char checkThisChar = player == GameSession.TurnOf.Player1_X ? CrossChar : CircleChar;
            int winAmount = this.FieldSize;

            bool hadWin = false;

            //Вертикальные и диагональные условия победы зависят от размера: по 3-5 возможных вариантов на каждое измерение
            for (int y = 0; y < winAmount; y++)
                hadWin = hadWin || CheckHorizontalWinAtLine(checkThisChar, winAmount, y);
            for (int x = 0; x < winAmount; x++)
                hadWin = hadWin || CheckVerticalWinAtRow(checkThisChar, winAmount, x);

            //Диагональных побед всегда возможно только две, и только в строго установленных местах
            hadWin = hadWin || CheckDiagonalWins(checkThisChar, winAmount);

            return hadWin;
        }

        private bool CheckHorizontalWinAtLine(char checkThisChar, int winAmount, int y)
        {
            for (int x = 0; x < winAmount; x++)
            {
                if (this.Field[x, y] == checkThisChar)
                    continue;
                else
                    return false;
            }

            return true;
        }

        private bool CheckVerticalWinAtRow(char checkThisChar, int winAmount, int x)
        {
            for (int y = 0; y < winAmount; y++)
            {
                if (this.Field[x, y] == checkThisChar)
                    continue;
                else
                    return false;
            }

            return true;
        }

        private bool CheckDiagonalWins(char checkThisChar, int winAmount)
        {
            //Первая диагональ изменяется следующим образом: xy = 0,0; 1,1; 2,2 ...
            for (int xy = 0; xy < winAmount; xy++)
            {
                if (this.Field[xy, xy] != checkThisChar)
                    break;

                //Если мы дошли до конца и до сих пор все символы были совпадающими, значит первая диагональ удовлетворяет условию победы
                if (xy == winAmount - 1)
                    return true;
            }

            //Вторая диагональ изменяется следующим образом: xy = 4,0; 3,1; 2,2; 1,3; 0,4
            for (int xy = 0; xy < winAmount; xy++)
            {
                if (this.Field[winAmount-1 - xy, xy] != checkThisChar)
                    break;

                //Если мы дошли до конца и до сих пор все символы были совпадающими, значит первая диагональ удовлетворяет условию победы
                if (xy == winAmount - 1)
                    return true;
            }

            return false;
        }
    }
}
