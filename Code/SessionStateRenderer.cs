using System;
using System.Collections.Generic;
using System.Text;

namespace CrossesTechTask.Code
{
    public static class SessionStateRenderer
    {
        static public void RenderSimple(GameSession session)
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

            if (session.Winner == GameSession.TurnOf.None)
            {
                //Вывод информации о текущем ходе
                string turnOf = session.CurrentTurn == GameSession.TurnOf.Player1_X ? "первый игрок" : "второй игрок";
                string humanOrAI = session.GetCurrentPlayer().IsHuman ? " (человек)" : " (компьютер)";

                Console.WriteLine("Ходит " + turnOf + humanOrAI);
            }
            else
            {
                string winnerPlayerStr = session.Winner == GameSession.TurnOf.Player1_X ? "первый игрок" : "второй игрок";

                Console.WriteLine("Побеждает " + winnerPlayerStr + "!");
            }
        }
    }
}
