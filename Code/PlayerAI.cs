using System;
using System.Numerics;
using System.Threading;

namespace CrossesTechTask.Code
{
    /// <summary>
    /// Набор методов логики хода ИИ игрока Player, вынесенные в отдельный класс для лучшей читабельности. При вводе
    /// информации о текущей сессии, игровом поле и себе как игроке, делает наилучший возможный ход
    /// </summary>
    static public class PlayerAI
    {
        /// <summary>
        /// Обработка логики хода компьютера. Анализирует игровое поле с нескольких сценариев, и выбирает наиболее оптимальный ход.
        /// Возвращает true при принятии решения (на практике - всегда, пока есть пустые клетки в ход ИИ)
        /// </summary>
        static public bool MakeNextMove(GameSession session, Grid gameGrid, Player selfPlayerEntity)
        {
            //Задержка хода, чтобы не дезориентировать человека резкостью действий
            Thread.Sleep(300);

            /// Заметка: ходы в порядке приоритета. Это не логика ИИ, а разбор видов ходов:
            /// 1.  Центральный, если 3х3 или 5х5 (4 варианта выигрыша: горизонталь, вертикаль и 2 диагонали)
            /// 2.  Один из четырех углов (3 варианта выигрыша: горизонталь, вертикаль и диагональ)
            /// 3а. Центральные на 4х4 (3 варианта выигрыша: горизонталь, вертикаль и диагональ)
            /// 3б. Вдоль диагонали на 5х5, кроме центра (3 варианта)
            /// 4.  Любое другое место (2 варианта выигрыша)
            ///
            /// Логика ИИ:
            /// 1. Запуск
            /// 2. Проверка вариантов победы другого игрока
            ///     а. Если другой игрок победит следующим ходом, то сделать ход туда, куда сделав бы ход, победил другой игрок
            /// 3. Проверка вариантов собственной победы
            ///     а. Если есть вариант победы в FieldSize-1 ходов, то имеет смысл сделать ход туда
            ///        (Например, вариант победы в 4, 3, 2, 1 ход при 5х5).
            ///        При этом выбрать клетку с наименьшим числом ходов для победы, и сделать ход туда.
            ///     б. Если таких вариантов несколько, то выбрать тот, у которого наибольшее число возможных побед
            ///        (Это часто углы, либо клетки при диагоналях, но к середине игры это не обязательно так)
            /// 4. Проверка на ходы с наибольшим числом побед
            ///     а. Если ИИ дошёл до этого пункта и не сделал ход, то скорее всего сейчас либо начало игры, либо
            ///        в прошлом ходу противник пресек все ближайшие варианты победы. В таком случае, анализируется
            ///        каждая клетка игрового поля, и выбирается та, где число возможных вариантов побед в будущем наибольше всего.

            // 1. Запуск
            char self = session.CurrentTurn == GameSession.PlayerType.Player1_X ? Grid.CrossChar : Grid.CircleChar;
            char opponent = session.CurrentTurn == GameSession.PlayerType.Player1_X ? Grid.CircleChar : Grid.CrossChar;
            bool madeMove = false;

            // 2. Проверка вариантов победы другого игрока:
            madeMove = MakeNextMove_CountEnemyWinVariants(session, gameGrid, selfPlayerEntity, opponent);
            if (madeMove)
                return true;

            // 3. Проверка вариантов собственной победы:
            madeMove = MakeNextMove_CountOwnWinVariants(session, gameGrid, selfPlayerEntity, self, opponent);
            if (madeMove)
                return true;

            // 4.Проверка на ходы с наибольшим числом побед
            madeMove = MakeNextMove_LookForHighestPossibleWinsCell(session, gameGrid, selfPlayerEntity, self, opponent);
            if (madeMove)
                return true;

            return false;
        }

        /// <summary>
        /// Первый этап логики ИИ. Проверка вариантов победы другого игрока. Если другой игрок победит следующим ходом, то сделать ход туда, куда сделав бы ход, победил другой игрок
        /// </summary>
        static private bool MakeNextMove_CountEnemyWinVariants(GameSession session, Grid gameGrid, Player selfPlayerEntity, char opponent)
        {
            for (int x = 0; x < gameGrid.FieldSize; x++)
                for (int y = 0; y < gameGrid.FieldSize; y++)
                {
                    int turnsToWin = Int32.MaxValue;
                    turnsToWin = gameGrid.CountTurnsToFastestWinAtCell(opponent, x, y);

                    if (turnsToWin <= 1)
                    {
                        //Если клетка занята противником, следующий цикл просто найдёт другую клетку с такими же условиями,
                        //поэтому дополнительно проверять куда именно поставить "противопобедный" ход не надо
                        bool lSuccess = selfPlayerEntity.TryPlaceOwnCharOnGrid(session, gameGrid, new Vector2(x, y));

                        if (lSuccess)
                        {
                            session.PassTurn();
                            return true;
                        }
                    }
                }

            return false;
        }

        /// <summary>
        /// <para> Второй этап логики ИИ. Проверка вариантов собственной победы: </para>
        /// <para> а. Если есть вариант победы в FieldSize-1 ходов, то имеет смысл сделать ход туда
        ///           (Например, вариант победы в 4, 3, 2, 1 ход при 5х5).
        ///           При этом выбрать клетку с наименьшим числом ходов для победы, и сделать ход туда.</para>
        /// <para> б. Если таких вариантов несколько, то выбрать тот, у которого наибольшее число возможных побед
        ///           (Это часто углы, либо клетки при диагоналях, но к середине игры это не обязательно так)</para>
        /// </summary>
        static private bool MakeNextMove_CountOwnWinVariants(GameSession session, Grid gameGrid, Player selfPlayerEntity, char self, char opponent)
        {
            //Поиск самого быстрого варианта победы, с минимально возможным числом ходов
            Vector3 XY_turnsToWin = new Vector3(0, 0, Int32.MaxValue); //Переменная хранит координаты в XY и число ходов до победы в Z
            for (int x = 0; x < gameGrid.FieldSize; x++)
                for (int y = 0; y < gameGrid.FieldSize; y++)
                {
                    //Проверка на самый быстрый вариант победы
                    int loc_turnsToWin = gameGrid.CountTurnsToFastestWinAtCell(self, x, y);
                    if (loc_turnsToWin < XY_turnsToWin.Z && gameGrid.Field[x, y] == Grid.EmptyChar)
                    {
                        XY_turnsToWin = new Vector3(x, y, loc_turnsToWin);
                    }

                    //Если попался равный по скорости вариант победы, то проверить его на число возможных вариантов победы. Выбрать тот, где возможностей больше
                    else if (loc_turnsToWin == XY_turnsToWin.Z && gameGrid.Field[x, y] == Grid.EmptyChar)
                    {
                        int winsFormer = gameGrid.CountPossibleWinsAtCell(self, opponent, (int)XY_turnsToWin.X, (int)XY_turnsToWin.Y);
                        int winsCandidate = gameGrid.CountPossibleWinsAtCell(self, opponent, x, y);
                        if (winsCandidate > winsFormer)
                            XY_turnsToWin = new Vector3(x, y, loc_turnsToWin);
                    }
                }

            //Если самый быстрый вариант победы сейчас также при этом меньше максимально возможного числа ходов, то сделать ход туда
            //Т.е. в 5х5 при победе в 1-4 ходов, будет сделан ход туда. В 5 ходов будет пропуск что возможно либо в начале игры,
            //либо при хорошем прошлом ходе противника
            if (XY_turnsToWin.Z < gameGrid.FieldSize)
            {
                bool lSuccess = selfPlayerEntity.TryPlaceOwnCharOnGrid(session, gameGrid, new Vector2(XY_turnsToWin.X, XY_turnsToWin.Y));

                if (lSuccess)
                {
                    session.PassTurn();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// <para> Третий, последний этап логики ИИ. Проверка на ходы с наибольшим числом побед </para>
        /// <para>    а. Если ИИ дошёл до этого пункта и не сделал ход, то скорее всего сейчас либо начало игры, либо
        ///              в прошлом ходу противник пресек все ближайшие варианты победы. В таком случае, анализируется
        ///              каждая клетка игрового поля, и выбирается та, где число возможных вариантов побед в будущем наибольше всего. </para>
        /// </summary>
        static private bool MakeNextMove_LookForHighestPossibleWinsCell(GameSession session, Grid gameGrid, Player selfPlayerEntity, char self, char opponent)
        {
            // Посчитать число возможных побед для каждой клетки. Победа считается, только если на линии свои, либо пустые клетки.
            int[,] winsPerCell = new int[gameGrid.FieldSize, gameGrid.FieldSize];
            Vector3 XY_max = new Vector3(0, 0, 0); //Переменная хранит координаты в XY и максимальное число побед в Z
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

                    cellWinCount += gameGrid.CountPossibleWinsAtCell(self, opponent, x, y);

                    winsPerCell[x, y] = cellWinCount;

                    //Обновление данных о клетке с максимальным числом возможных побед
                    if (winsPerCell[x, y] >= XY_max.Z)
                        XY_max = new Vector3(x, y, winsPerCell[x, y]);
                }

            bool success = selfPlayerEntity.TryPlaceOwnCharOnGrid(session, gameGrid, new Vector2(XY_max.X, XY_max.Y));

            if (success)
            {
                session.PassTurn();
                return true;
            }

            return false;
        }
    }
}
