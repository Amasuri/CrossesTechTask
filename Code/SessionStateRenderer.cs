using System;

namespace CrossesTechTask.Code
{
    public static class SessionStateRenderer
    {
        static public void Render(GameSession session, App app)
        {
            //Вывод информации о режиме игры
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

            //Если у нас нет победителя и игра продолжается
            if (session.Winner == GameSession.TurnOf.None && app.gameState == App.GameState.Playing)
            {
                //Вывод информации о текущем ходе
                string turnOf = session.CurrentTurn == GameSession.TurnOf.Player1_X ? "первый игрок ( X, " : "второй игрок ( O, ";
                string humanOrAI = session.GetCurrentPlayer().IsHuman ? "человек)" : "компьютер)";

                Console.WriteLine("Ходит " + turnOf + humanOrAI);

                //Подсказка по управлению
                if (session.GetCurrentPlayer().IsHuman)
                    Console.WriteLine("(Выберите клетку стрелками, и затем нажмите Space)\n");
                else
                    Console.WriteLine("\n");
            }

            //Если у нас есть победитель и игра окончена
            else if ((session.Winner != GameSession.TurnOf.None && app.gameState == App.GameState.Gameover))
            {
                string winnerPlayerStr = session.Winner == GameSession.TurnOf.Player1_X ? "первый игрок (X)" : "второй игрок (O)";

                Console.WriteLine("Побеждает " + winnerPlayerStr + "!\nНажмите Enter для новой партии.\n");
            }

            //Если у нас нет победителя и игра окончена, то есть ничья
            else if ((session.Winner == GameSession.TurnOf.None && app.gameState == App.GameState.Gameover))
            {
                Console.WriteLine("Сыграна ничья." + "\nНажмите Enter для новой партии.\n");
            }
        }
    }
}
