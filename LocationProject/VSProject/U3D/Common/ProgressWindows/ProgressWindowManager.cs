using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Common.ProgressWindows
{
    public static class ProgressWindowManager
    {
        public static IProgressWindow Window;

        public static void Show(string text,int i,int max)
        {
            if (Window != null)
            {
                Window.ShowProgress(text, i, max);
            }
        }

        public static void Close()
        {
            if (Window != null)
            {
                Window.CloseWindow();
            }
        }
    }
}
