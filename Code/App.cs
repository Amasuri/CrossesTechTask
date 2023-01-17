using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CrossesTechTask.Code
{
    public class App
    {
        private Grid grid;
        private GameSession session;

        /// <summary>
        /// Внутренняя переменная для оптимизации отрисовки. Можно обновлять экран раз в Х миллисекунд, но тогда экран заметно мерцает
        /// (издержки консольного приложения). Эта переменная используется, чтобы обновлять экран только при смене состояния игры.
        /// </summary>
        private bool ChangedStateOnUpdate = false;

        public App()
        {
            grid = new Grid();
            session = new GameSession();
        }

        public void Init()
        {
            int FieldSizeChoice = ChooseFromThree("Выберите размер поля:" +
                                        "\n  1. 3х3" +
                                        "\n  2. 4х4" +
                                        "\n  3. 5х5");

            int FieldSize = FieldSizeChoice + 2; //Удобным образом размер сетки соответствует пункту меню +2, но это можно поменять (при желании)
            grid.SetField(FieldSize);

            int ModeChoice = ChooseFromThree("Выберите режим:" +
                "\n  1. Игрок против компьютера" +
                "\n  2. Игрок против игрока" +
                "\n  3. Компьютер против компьютера (демо)");

            session.SetMode(ModeChoice);
        }

        public void Update()
        {
            ChangedStateOnUpdate = false;

            //on things...
            ChangedStateOnUpdate = true;
        }

        public void Draw()
        {
            if (!ChangedStateOnUpdate)
                return;

            Console.Clear();

            Console.WriteLine("Ходит..." + "(кто-то)" + "\n");
            GridRenderer.RenderSimple(this.grid);
        }

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
