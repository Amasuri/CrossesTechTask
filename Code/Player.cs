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
        public bool Update(GameSession session)
        {
            if (IsHuman)
                return DoHumanLogic(session);
            else
                return DoAiLogic(session);
        }

        private bool DoHumanLogic(GameSession session)
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
            }

            return false;
        }

        private bool DoAiLogic(GameSession session)
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
