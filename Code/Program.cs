using System;
using System.Threading;

namespace CrossesTechTask.Code
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            App app = new App();

            app.Init();

            while (true)
            {
                app.Update();
                app.Draw();
            }
        }
    }
}
