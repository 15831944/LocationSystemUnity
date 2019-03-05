using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Common.ProgressWindows
{
    public interface IProgressWindow
    {
        void ShowProgress(string text, double i, double max);

        void CloseWindow();
    }
}
