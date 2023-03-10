using System;

namespace CrossesTechTask.Code
{
    /// <summary>
    /// Класс игровой сессии, то есть всего, что связано непосредственно с текущей сессией игры: режим игры, кто сейчас ходит, кто победитель (если есть),
    /// а так же сущности игроков.
    /// </summary>
    public class GameSession
    {
        public Mode CurrentMode { get; private set; }

        /// <summary>
        /// Режим игры: Игрок против компьютера, два игрока против друг друга, либо два компьютера.
        /// </summary>
        public enum Mode
        {
            PlayerVsAI = 1,
            PlayerVsPlayer = 2,
            AIvsAI = 3,
        }

        public PlayerType CurrentTurn { get; private set; }
        public PlayerType Winner { get; private set; }
        public enum PlayerType
        {
            None = -1, //технически такого игрока нет, но некоторые переменные (например, переменная победитель) должны хранить нулевое значение
            Player1_X = 0,
            Player2_O = 1,
        }

        //Игрок может быть как компьютером, так и человеком
        private Player player1;
        private Player player2;

        public void SetMode(int mode)
        {
            CurrentMode = (Mode)mode;
        }

        public void Init()
        {
            Winner = PlayerType.None;

            player1 = new Player();
            player2 = new Player();

            //Подбрасываем монетку на то, кто ходит первый
            Random rand = new Random();
            int chance = rand.Next(0, 100);
            if (chance < 50)
                CurrentTurn = PlayerType.Player1_X;
            else
                CurrentTurn = PlayerType.Player2_O;

            //Инициализируем игроков в зависимости от режима
            switch (CurrentMode)
            {
                case Mode.PlayerVsAI:
                    player1.SetAsHuman();
                    player2.SetAsAI();
                    break;

                case Mode.PlayerVsPlayer:
                    player1.SetAsHuman();
                    player2.SetAsHuman();
                    break;

                case Mode.AIvsAI:
                    player1.SetAsAI();
                    player2.SetAsAI();
                    break;
            }
        }

        /// <summary>
        /// Обновление логики хода игроков. Возвращает true если что-то поменялось в состоянии хода, и false, если нет
        /// (например, игрок - человек и думает над ходом, либо игрок - компьютер и сейчас задержка ввода (для удобного отображения) )
        /// </summary>
        public bool UpdatePlayers(Grid gameGrid)
        {
            return GetCurrentPlayer().Update(this, gameGrid);
        }

        public Player GetCurrentPlayer()
        {
            return CurrentTurn == PlayerType.Player1_X ? player1 : player2;
        }

        /// <summary>
        /// Заканчивает текущий ход и передаёт его другому игроку. Также сбрасывает указатели на клетку у игроков
        /// (актуально для игроков людей)
        /// </summary>
        public void PassTurn()
        {
            if (CurrentTurn == PlayerType.Player1_X)
                CurrentTurn = PlayerType.Player2_O;
            else
                CurrentTurn = PlayerType.Player1_X;

            player1.ResetPointer();
            player2.ResetPointer();
        }

        /// <summary>
        /// Запоминает победителя, но только если его ещё нет
        /// </summary>
        public void WriteWinner(PlayerType writeThisWinner)
        {
            if (this.Winner != PlayerType.None || writeThisWinner == PlayerType.None)
                return;

            this.Winner = writeThisWinner;
        }
    }
}
