using System;
using System.Collections.Generic;
using System.Text;

namespace CrossesTechTask.Code
{
    public class Player
    {
        /// <summary>
        /// Является ли данный игрок человеком, или же он компьютер? Влияет на модель поведения
        /// (для хода используется либо управление, либо алгоритм)
        /// </summary>
        public bool IsHuman { get; private set; }

        internal void SetAsHuman()
        {
            IsHuman = true;
        }

        internal void SetAsAI()
        {
            IsHuman = false;
        }
    }
}
