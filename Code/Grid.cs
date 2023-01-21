using System;
using System.Linq;

namespace CrossesTechTask.Code
{
    public class Grid
    {
        public const char CrossChar = 'X';
        public const char CircleChar = 'O';
        public const char EmptyChar = '*';

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
                    Field[x, y] = EmptyChar;
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
            if (Field[x, y] == EmptyChar)
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
        /// Проверяет, выполнено ли хоть одно условие победы для заданного игрока: по вертикали, горизонтали и диагонали. В случае победы возвращает true, иначе false.
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

        /// <summary>
        /// Проверка, выполнено ли условие победы в заданном ряду Y для игрока checkThisChar
        /// </summary>
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

        /// <summary>
        /// Проверка, выполнено ли условие победы в заданной колонне X для игрока checkThisChar
        /// </summary>
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

        /// <summary>
        /// Проверка, выполнено ли условие победы хотя бы в одной из двух диагоналей
        /// </summary>
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

        /// <summary>
        /// Считает количество возможных горизонтальных побед в данной клетке. Возможная победа означает победу не сейчас, но в некотором будущем,
        /// через какое-то число ходов. Возвращает 0, если такая победа невозможна (например, из-за клетки противника в этой горизонтали),
        /// и 1, если возможна (максимальное число горизонтальных побед на клетку при данных правилах).
        /// </summary>
        public int CountPossibleHorizontalWinsAtCell(char self, int y)
        {
            for (int x = 0; x < this.FieldSize; x++)
            {
                if (this.Field[x, y] == self || this.Field[x, y] == Grid.EmptyChar)
                    continue;
                else
                    return 0;
            }

            return 1;
        }

        /// <summary>
        /// Считает количество возможных вертикальных побед в данной клетке. Возможная победа означает победу не сейчас, но в некотором будущем,
        /// через какое-то число ходов. Возвращает 0, если такая победа невозможна (например, из-за клетки противника в этой вертикали),
        /// и 1, если возможна (максимальное число вертикальных побед на клетку при данных правилах).
        /// </summary>
        public int CountPossibleVerticalWinsAtCell(char self, int x)
        {
            for (int y = 0; y < this.FieldSize; y++)
            {
                if (this.Field[x, y] == self || this.Field[x, y] == Grid.EmptyChar)
                    continue;
                else
                    return 0;
            }

            return 1;
        }

        /// <summary>
        /// Считает количество возможных диагональных побед в данной клетке. Возможная победа означает победу не сейчас, но в некотором будущем,
        /// через какое-то число ходов. Возвращает 0, если такая победа невозможна (например, из-за клетки противника в этой вертикали).
        /// Возвращает 1, либо 2, если победа возможна хотя бы в одной из диагоналей (в зависимости от того, в скольки диагоналях возможна победа)
        /// </summary>
        public int CountPossibleDiagonalWinsAtCell(char opponent, int x, int y)
        {
            //Проверка на принадлежность к какой-либо диагонали. Для сверху левой правило x==y, для сверху правой y == MaxIndex - x
            bool IsUpperLeft = y == x;
            bool IsUpperRight = y == this.FieldMaxIndex - x;

            //Если клетка не принадлежит ни к какой из диагоналей, то проверять дальше не имеет смысла
            if (!IsUpperLeft && !IsUpperRight)
                return 0;

            int diagWinCount = 0;

            if (IsUpperLeft)
                diagWinCount += CountPossibleDiagonalWins_Left(opponent);

            if (IsUpperRight)
                diagWinCount += CountPossibleDiagonalWins_Right(opponent);

            return diagWinCount;
        }

        private int CountPossibleDiagonalWins_Left(char opponent)
        {
            int diagWinCount = 0;

            //Сверху левая диагональ изменяется следующим образом: xy = 0,0; 1,1; 2,2 ...
            for (int xy = 0; xy < this.FieldSize; xy++)
            {
                if (this.Field[xy, xy] == opponent)
                    break;

                //Если мы дошли до конца и до сих пор все символы были либо своими, либо пустыми, значит сверху левая диагональ удовлетворяет условию победы
                if (xy == this.FieldMaxIndex)
                    diagWinCount++;
            }

            return diagWinCount;
        }

        private int CountPossibleDiagonalWins_Right(char opponent)
        {
            int diagWinCount = 0;

            //Сверху правая диагональ изменяется следующим образом: xy = 4,0; 3,1; 2,2; 1,3; 0,4
            for (int xy = 0; xy < this.FieldSize; xy++)
            {
                if (this.Field[this.FieldMaxIndex - xy, xy] == opponent)
                    break;

                //Если мы дошли до конца и до сих пор все символы были либо своими, либо пустыми, значит сверху правая диагональ удовлетворяет условию победы
                if (xy == this.FieldMaxIndex)
                    diagWinCount++;
            }

            return diagWinCount;
        }

        /// <summary>
        /// Проверяет, возможна ли победа, и если возможна, то считает наименьшее число ходов до победы среди всех возможных
        /// </summary>
        public int CountTurnsToFastestWinAtCell(char player, int x, int y)
        {
            int horizontal = Int32.MaxValue;
            int vertical = Int32.MaxValue;
            int diagonalLeft = Int32.MaxValue;
            int diagonalRight = Int32.MaxValue;

            char opponent = player == CrossChar ? CircleChar : CrossChar;

            bool canWinHor = CountPossibleHorizontalWinsAtCell(player, y) > 0;
            bool canWinVer = CountPossibleVerticalWinsAtCell(player, x) > 0;
            bool canWinDia = CountPossibleDiagonalWinsAtCell(opponent, x, y) > 0;

            //На практике, число ходов до победы (при условии, что она возможна) - это число пустых клеток
            if(canWinHor)
            {
                horizontal = 0;
                for (int lx = 0; lx < this.FieldSize; lx++)
                    if (this.Field[lx, y] == Grid.EmptyChar)
                        horizontal++;
            }
            if (canWinVer)
            {
                vertical = 0;
                for (int ly = 0; ly < this.FieldSize; ly++)
                    if (this.Field[x, ly] == Grid.EmptyChar)
                        vertical++;
            }
            if (canWinDia)
            {
                ;
            }

            return new int[] { horizontal, vertical, diagonalLeft, diagonalRight }.Min();
        }
    }
}
