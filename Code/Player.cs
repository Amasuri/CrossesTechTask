using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
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
                    return true;

                case ConsoleKey.UpArrow:
                    Pointer += new Vector2(0, -1);
                    return true;

                case ConsoleKey.RightArrow:
                    Pointer += new Vector2(1, 0);
                    return true;

                case ConsoleKey.DownArrow:
                    Pointer += new Vector2(0, 1);
                    return true;

                case ConsoleKey.Spacebar:

                    //Нажатие кнопки "пробел" отвечает за попытку поставить символ на игровое поле по координатам указателя
                    //В случае успеха (на координатах нету другого символа) - ставит символ и передаёт ход другому игроку
                    Vector2 coord = session.GetCurrentPlayer().Pointer;

                    bool success = false;
                    if (session.CurrentTurn == GameSession.TurnOf.Player1_X)
                        success = gameGrid.TryPutCrossOnField((int)coord.X, (int)coord.Y);
                    else if (session.CurrentTurn == GameSession.TurnOf.Player2_O)
                        success = gameGrid.TryPutNoughtOnField((int)coord.X, (int)coord.Y);

                    if (success)
                        session.PassTurn();

                    return true;
            }

            return false;
        }

        private bool DoAiLogic(GameSession session, Grid gameGrid)
        {
            Thread.Sleep(500);
            session.PassTurn();

            return true;
        }

        public void ResetPointer()
        {
            Pointer = new Vector2(0, 0);
        }
    }
}
