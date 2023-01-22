namespace CrossesTechTask.Code
{
    /// <summary>
    /// Wrapper для запуска приложения через консоль.
    /// </summary>
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
