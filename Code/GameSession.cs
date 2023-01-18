using System;
using System.Collections.Generic;
using System.Text;

namespace CrossesTechTask.Code
{
    public class GameSession
    {
        public Mode CurrentMode { get; private set; }
        public enum Mode
        {
            PlayerVsAI = 1,
            PlayerVsPlayer = 2,
            AIvsAI = 3,
        }

        public TurnOf CurrentTurn;
        public enum TurnOf
        {
            Player1,
            Player2
        }

        //Может быть как компьютером, так и игроком
        private Player player1;
        private Player player2;

        public void SetMode(int mode)
        {
            CurrentMode = (Mode)mode;
        }

        public void Init()
        {
            player1 = new Player();
            player2 = new Player();

            //Подбрасываем монетку на то, кто ходит первый
            Random rand = new Random();
            int chance = rand.Next(0, 100);
            if (chance < 50)
                CurrentTurn = TurnOf.Player1;
            else
                CurrentTurn = TurnOf.Player2;

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
        public bool UpdatePlayers()
        {
            return GetCurrentPlayer().Update(this);
        }

        public Player GetCurrentPlayer()
        {
            return CurrentTurn == TurnOf.Player1 ? player1 : player2;
        }

        public void PassTurn()
        {
            if (CurrentTurn == TurnOf.Player1)
                CurrentTurn = TurnOf.Player2;
            else
                CurrentTurn = TurnOf.Player1;
        }
    }
}
