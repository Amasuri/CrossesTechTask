using System;
using System.Numerics;

namespace CrossesTechTask.Code
{
    /// <summary>
    /// Сущность игрока. Отвечает за обработку ИИ (для компьютеров), либо за приём ввода (для людей). Запоминает, человек ли этот игрок,
    /// а также то, куда на игровом поле смотрит сейчас этот игрок (актуально для людей)
    /// </summary>
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
                return PlayerAI.MakeNextMove(session, gameGrid, this);
        }

        /// <summary>
        /// Обновление логики хода человеческого игрока. Отвечает за приём и обработку ввода. Возвращает true если что-то поменялось в состоянии хода,
        /// например, игрок пододвинул курсор.
        /// </summary>
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

        /// <summary>
        /// Попытаться поставить символ своего типа на доску. Возвращает true при успешной попытке, и false при провале (клетка уже занята)
        /// </summary>
        public bool TryPlaceOwnCharOnGrid(GameSession session, Grid gameGrid, Vector2 coord)
        {
            bool success = false;

            if (session.CurrentTurn == GameSession.PlayerType.Player1_X)
                success = gameGrid.TryPutCrossOnField((int)coord.X, (int)coord.Y);
            else if (session.CurrentTurn == GameSession.PlayerType.Player2_O)
                success = gameGrid.TryPutNoughtOnField((int)coord.X, (int)coord.Y);

            return success;
        }

        /// <summary>
        /// Сбросить указатель на клетку игрового поля, закрепленный за данным игроком, в верхний левый угол
        /// </summary>
        public void ResetPointer()
        {
            Pointer = new Vector2(0, 0);
        }
    }
}
