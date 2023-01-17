using System;
using System.Threading;

namespace CrossesTechTask
{
    internal class Program
    {
        private static int FieldSize;
        private static int Mode;

        private static void Main(string[] args)
        {
            FieldSize = ChooseFromThree("Выберите размер поля:" +
                            "\n  1. 3х3" +
                            "\n  2. 4х4" +
                            "\n  3. 5х5");

            Mode = ChooseFromThree("Выберите режим:" +
                "\n  1. Игрок против компьютера" +
                "\n  2. Игрок против игрока" +
                "\n  3. Компьютер против компьютера (демо)");
        }

        private static int ChooseFromThree(string descriptionText)
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
