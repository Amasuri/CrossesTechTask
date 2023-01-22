using System;

namespace CrossesTechTask.Code
{
    public static class SessionStateRenderer
    {
        /// <summary>
        /// Отрисовка текущего состояния игровой сессии (информация о текущем ходе, победителе, победе либо ничье, а также подсказки
        /// с управлением, если такие уместны)
        /// </summary>
        static public void Render(GameSession session, App app)
        {
            //Вывод информации о режиме игры
            RenderGameMode(session);

            //Если у нас нет победителя и игра продолжается
            if (session.Winner == GameSession.PlayerType.None && app.gameState == App.GameState.Playing)
                RenderCurrentTurn(session);

            //Если у нас есть победитель и игра окончена
            else if ((session.Winner != GameSession.PlayerType.None && app.gameState == App.GameState.Gameover))
                RenderWinningMessage(session);

            //Если у нас нет победителя и игра окончена, то есть ничья
            else if ((session.Winner == GameSession.PlayerType.None && app.gameState == App.GameState.Gameover))
                RenderParityMessage();
        }

        private static void RenderParityMessage()
        {
            Console.WriteLine("Сыграна ничья." + "\nНажмите Enter для новой партии.\n");
        }

        private static void RenderWinningMessage(GameSession session)
        {
            string winnerPlayerStr = session.Winner == GameSession.PlayerType.Player1_X ? "первый игрок (X)" : "второй игрок (O)";

            Console.WriteLine("Побеждает " + winnerPlayerStr + "!\nНажмите Enter для новой партии.\n");
        }

        private static void RenderCurrentTurn(GameSession session)
        {
            //Вывод информации о текущем ходе
            string turnOf = session.CurrentTurn == GameSession.PlayerType.Player1_X ? "первый игрок ( X, " : "второй игрок ( O, ";
            string humanOrAI = session.GetCurrentPlayer().IsHuman ? "человек)" : "компьютер)";

            Console.WriteLine("Ходит " + turnOf + humanOrAI);

            //Подсказка по управлению
            if (session.GetCurrentPlayer().IsHuman)
                Console.WriteLine("(Выберите клетку стрелками, и затем нажмите Space)\n");
            else
                Console.WriteLine("\n");
        }

        private static void RenderGameMode(GameSession session)
        {
            Console.Write("Режим игры: ");
            switch (session.CurrentMode)
            {
                case GameSession.Mode.PlayerVsAI:
                    Console.WriteLine("Игрок против ИИ");
                    break;

                case GameSession.Mode.PlayerVsPlayer:
                    Console.WriteLine("Игрок против игрока");
                    break;

                case GameSession.Mode.AIvsAI:
                    Console.WriteLine("ИИ против ИИ");
                    break;

                default:
                    throw new Exception("Режим игры задан некорректно. Проверьте код");
                    break;
            }
        }
    }
}
