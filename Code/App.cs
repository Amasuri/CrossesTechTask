using System;
using System.Threading;

namespace CrossesTechTask.Code
{
    /// <summary>
    /// Класс приложения. Высокоуровневый, отвечает за глобальные операции, вызов нужных методов Update и Draw у других классов, а так же машину состояний
    /// этого приложения. Хранит и делится информацией о текущем состоянии приложения (загрузка, игра, либо её конец).
    /// </summary>
    public class App
    {
        private Grid grid;
        private GameSession session;

        /// <summary>
        /// Описывает состояния игры с точки зрения экранов приложения, и соответствующей им логики и отрисовки.
        /// Не описывает состояния текущей игровой сессии
        /// </summary>
        public enum GameState
        {
            Init,
            Playing,
            Gameover
        }
        public GameState gameState { get; private set; }

        /// <summary>
        /// Внутренняя переменная для оптимизации отрисовки. Можно обновлять экран раз в Х миллисекунд, но тогда экран заметно мерцает
        /// (издержки консольного приложения). Эта переменная используется, чтобы обновлять экран только при смене состояния игры.
        /// </summary>
        private bool ChangedStateOnUpdate = false;

        public App()
        {
            Reset();
        }

        private void Reset()
        {
            gameState = GameState.Init;
            grid = new Grid();
            session = new GameSession();
        }

        public void Init()
        {
            int FieldSizeChoice = ChooseFromThree("Выберите размер поля (Введите 1-3 и нажмите Enter):" +
                                        "\n  1. 3х3" +
                                        "\n  2. 4х4" +
                                        "\n  3. 5х5");

            int FieldSize = FieldSizeChoice + 2; //Удобным образом размер сетки соответствует пункту меню по номеру +2, но это можно поменять (при желании)
            grid.SetField(FieldSize);

            int ModeChoice = ChooseFromThree("Выберите режим (Введите 1-3 и нажмите Enter):" +
                "\n  1. Игрок против компьютера" +
                "\n  2. Игрок против игрока" +
                "\n  3. Компьютер против компьютера (демо)");

            session.SetMode(ModeChoice);
            session.Init();

            RunFirstRefresh();
        }

        public void Update(bool forceUpdate = false)
        {
            ChangedStateOnUpdate = false;

            //Обновление игроков при игре
            if (this.gameState == GameState.Playing)
            {
                ChangedStateOnUpdate =
                    forceUpdate ||
                    session.UpdatePlayers(this.grid);

                CheckForWin();

                //Если игра достигла ничьи и победителя до сих пор не было
                if (grid.CheckParity() && session.Winner == GameSession.PlayerType.None)
                {
                    this.gameState = GameState.Gameover;
                }
            }

            //Проверка на ресет при нажатии Enter во время конца игры. Начинает новую сессию с нуля
            else if (this.gameState == GameState.Gameover)
            {
                if(Console.ReadKey(true).Key == ConsoleKey.Enter) //enter
                {
                    this.Reset();
                    this.Init();
                }
            }
        }

        public void Draw()
        {
            //Обновлять отрисовку только при наличии изменений. Ограничения консольного приложения - иначе экран будет мерцать
            if (!ChangedStateOnUpdate)
                return;

            Console.Clear();

            SessionStateRenderer.Render(this.session, this);
            GridRenderer.Render(this.grid, this.session);
        }

        /// <summary>
        /// Первое обновление после инициализации
        /// </summary>
        private void RunFirstRefresh()
        {
            gameState = GameState.Playing;

            this.Update(forceUpdate: true);
            this.Draw();
        }

        /// <summary>
        /// Проверять условия победы только если хоть что-то поменялось на экране. Порядок проверки не важен, потому что игроки всегда ходят по очереди.
        /// То есть, если что-то поменялось от первого игрока, то второй игрок по правилам игры ничего не мог сделать в этот момент.
        /// </summary>
        private void CheckForWin()
        {
            if (ChangedStateOnUpdate)
            {
                bool P1wins = grid.CheckWin(GameSession.PlayerType.Player1_X);
                bool P2wins = grid.CheckWin(GameSession.PlayerType.Player2_O);

                if (P1wins || P2wins)
                {
                    gameState = GameState.Gameover;
                    var winner = P1wins ? GameSession.PlayerType.Player1_X : GameSession.PlayerType.Player2_O;

                    session.WriteWinner(winner);
                }
            }
        }

        /// <summary>
        /// Локальный метод для выбора трёх пунктов меню. Возвращает 1-3 в зависимости от выбора. Валидирует ввод.
        /// При неправильном вводе будет бесконечно просить ввести снова.
        /// </summary>
        private int ChooseFromThree(string descriptionText)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(descriptionText);

                string input = Console.ReadLine();

                if (input == "1" || input == "2" || input == "3")
                {
                    return Convert.ToInt32(input);
                }
                else
                {
                    Console.WriteLine("Введите правильное число от 1 до 3");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
