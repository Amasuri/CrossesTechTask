using System;
using System.Numerics;
using System.Threading;

namespace CrossesTechTask.Code
{
    public class Player
    {
        /// <summary>
        /// Является ли данный игрок человеком, или же он компьютер? Влияет на модель поведения
        /// (для хода используется либо управление, либо алгоритм)
        /// </summary>
        public bool IsHuman { get; private set; }

        /// <summary>
        /// Координаты на карте, куда целится данный игрок сейчас. В основном актуально для людей, но при желании
        /// может использоваться и для ИИ.
        /// </summary>
        public Vector2 Pointer { get; private set; }

        public Player()
        {
            ResetPointer();
        }

        public void SetAsHuman()
        {
            IsHuman = true;
        }

        public void SetAsAI()
        {
            IsHuman = false;
        }

        /// <summary>
        /// Обновление логики хода конкретного игрока. Возвращает true если что-то поменялось в состоянии хода, и false, если нет
        /// (например, игрок - человек и думает над ходом, либо игрок - компьютер и сейчас задержка ввода (для удобного отображения) )
        /// </summary>
        public bool Update(GameSession session, Grid gameGrid)
        {
            if (IsHuman)
                return DoHumanLogic(session, gameGrid);
            else
                return DoAiLogic(session, gameGrid);
        }

        private bool DoHumanLogic(GameSession session, Grid gameGrid)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.LeftArrow:
                    Pointer += new Vector2(-1, 0);

                    if (Pointer.X < 0)
                        Pointer = new Vector2(0, Pointer.Y);

                    return true;

                case ConsoleKey.UpArrow:
                    Pointer += new Vector2(0, -1);

                    if (Pointer.Y < 0)
                        Pointer = new Vector2(Pointer.X, 0);

                    return true;

                case ConsoleKey.RightArrow:
                    Pointer += new Vector2(1, 0);

                    if (Pointer.X > gameGrid.FieldMaxIndex)
                        Pointer = new Vector2(gameGrid.FieldMaxIndex, Pointer.Y);

                    return true;

                case ConsoleKey.DownArrow:
                    Pointer += new Vector2(0, 1);

                    if (Pointer.Y > gameGrid.FieldMaxIndex)
                        Pointer = new Vector2(Pointer.X, gameGrid.FieldMaxIndex);

                    return true;

                case ConsoleKey.Spacebar:

                    //Нажатие кнопки "пробел" отвечает за попытку поставить символ на игровое поле по координатам указателя
                    //В случае успеха (на координатах нету другого символа) - ставит символ и передаёт ход другому игроку
                    Vector2 coord = session.GetCurrentPlayer().Pointer;

                    bool success = TryPlaceOwnCharOnGrid(session, gameGrid, coord);

                    if (success)
                        session.PassTurn();

                    return true;
            }

            return false;
        }

        private bool DoAiLogic(GameSession session, Grid gameGrid)
        {
            Thread.Sleep(300);

            /// Ходы в порядке приоритета:
            /// 1. Центральный, если 3х3 или 5х5 (4 варианта выигрыша: горизонталь, вертикаль и 2 диагонали)
            /// 2. Один из четырех углов (3 варианта выигрыша: горизонталь, вертикаль и диагональ)
            /// 3а. Центральные на 4х4 (3 варианта выигрыша: горизонталь, вертикаль и диагональ)
            /// 3б. Вдоль диагонали на 5х5, кроме центра (3 варианта)
            /// 4. Любое другое место (2 варианта выигрыша)
            ///
            /// 1. Запуск
            /// 2. Проверка вариантов победы другого игрока
            ///     а. Если другой игрок победит следующим ходом, то сделать ход туда
            /// 3. Проверка вариантов победы
            ///     а. Проверить чистые клетки в ряд для каждой ячейки. Для каждой считать число побед
            ///     б. Выбор варианта с наибольшим числом возможных побед (к середине игры это не обязательно центр или угол)
            ///     в. Проверить собственное число возможных "близких побед". Ранжировать по числу оставшихся для побед клеток
            ///     г. Если остался менее чем максимальный ход до победы (т.е. не первый ход игры), запомнить эту клетку.
            ///        Выбрать наименьший возможный и поставить клетку туда.
            ///        Если таких наименьших вариантов несколько, то выбрать тот, у которого наибольшее число возможных побед и поставить туда
            ///

            char self = session.CurrentTurn == GameSession.TurnOf.Player1_X ? Grid.CrossChar : Grid.CircleChar;
            char opponent = session.CurrentTurn == GameSession.TurnOf.Player1_X ? Grid.CircleChar : Grid.CrossChar;

            //Проверка вариантов победы другого игрока: Если другой игрок победит следующим ходом, то сделать ход туда
            for (int x = 0; x < gameGrid.FieldSize; x++)
                for (int y = 0; y < gameGrid.FieldSize; y++)
                {
                    int turnsToWin = Int32.MaxValue;
                    turnsToWin = gameGrid.CountTurnsToFastestWinAtCell(opponent, x, y);

                    if(turnsToWin <= 1)
                    {
                        //Если клетка занята противником, следующий цикл просто найдёт другую клетку с такими же условиями,
                        //поэтому дополнительно проверять куда именно поставить "противопобедный" ход не надо
                        bool lSuccess = TryPlaceOwnCharOnGrid(session, gameGrid, new Vector2(x, y));

                        if (lSuccess)
                        {
                            session.PassTurn();
                            return true;
                        }
                    }
                }

            //Проверка своих вариантов победы: если есть вариант победы в FieldSize-1, то имеет смысл сделать ход туда
            //На практике это означает, что в 90% случаев все, кроме первого хода, будет приближением уже обозначенной победы
            //Если таких вариантов несколько, то выбрать тот, у которого наибольшее число возможных побед
            Vector3 XY_turnsToWin = new Vector3(0, 0, Int32.MaxValue);
            for (int x = 0; x < gameGrid.FieldSize; x++)
                for (int y = 0; y < gameGrid.FieldSize; y++)
                {
                    //Проверка на самый оптимальный вариант победы
                    int loc_turnsToWin = gameGrid.CountTurnsToFastestWinAtCell(self, x, y);
                    if(loc_turnsToWin < XY_turnsToWin.Z && gameGrid.Field[x,y] == Grid.EmptyChar)
                        XY_turnsToWin = new Vector3(x, y, loc_turnsToWin);
                }

            //Если самый быстрый вариант победы сейчас также при этом меньше максимально возможного числа ходов, то сделать ход туда
            if (XY_turnsToWin.Z < gameGrid.FieldSize)
            {
                bool lSuccess = TryPlaceOwnCharOnGrid(session, gameGrid, new Vector2(XY_turnsToWin.X, XY_turnsToWin.Y));

                if (lSuccess)
                {
                    session.PassTurn();
                    return true;
                }
            }

            //Посчитать число возможных побед для каждой клетки. Победа считается, только если на линии свои, либо пустые клетки.
            int[,] winsPerCell = new int[gameGrid.FieldSize, gameGrid.FieldSize];
            Vector3 XY_max = new Vector3(0, 0, 0);
            for (int x = 0; x < gameGrid.FieldSize; x++)
                for (int y = 0; y < gameGrid.FieldSize; y++)
                {
                    if (gameGrid.Field[x, y] != Grid.EmptyChar)
                    {
                        winsPerCell[x, y] = 0;
                        continue;
                    }

                    //Проверка (диагонали, горизонталь, вертикаль)
                    int cellWinCount = 0;

                    cellWinCount += gameGrid.CountPossibleHorizontalWinsAtCell(self, y);
                    cellWinCount += gameGrid.CountPossibleVerticalWinsAtCell(self, x);
                    cellWinCount += gameGrid.CountPossibleDiagonalWinsAtCell(opponent, x, y);

                    winsPerCell[x, y] = cellWinCount;

                    //Обновление данных о клетке с максимальным числом возможных побед
                    if (winsPerCell[x, y] >= XY_max.Z)
                        XY_max = new Vector3(x, y, winsPerCell[x, y]);
                }

            bool success = TryPlaceOwnCharOnGrid(session, gameGrid, new Vector2(XY_max.X, XY_max.Y));

            if(success)
                session.PassTurn();

            return true;
        }

        private bool TryPlaceOwnCharOnGrid(GameSession session, Grid gameGrid, Vector2 coord)
        {
            bool success = false;

            if (session.CurrentTurn == GameSession.TurnOf.Player1_X)
                success = gameGrid.TryPutCrossOnField((int)coord.X, (int)coord.Y);
            else if (session.CurrentTurn == GameSession.TurnOf.Player2_O)
                success = gameGrid.TryPutNoughtOnField((int)coord.X, (int)coord.Y);

            return success;
        }

        public void ResetPointer()
        {
            Pointer = new Vector2(0, 0);
        }
    }
}
