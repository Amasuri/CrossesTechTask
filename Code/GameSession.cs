using System;
using System.Collections.Generic;
using System.Text;

namespace CrossesTechTask.Code
{
    public class GameSession
    {
        public int CurrentMode { get; private set; }

        public void SetMode(int mode)
        {
            CurrentMode = mode;
        }
    }
}
