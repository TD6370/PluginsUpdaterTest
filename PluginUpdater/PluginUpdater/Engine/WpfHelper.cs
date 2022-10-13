using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PluginUpdater.Engine
{
    public static class WpfHelper
    {
        public static void InvokeMethod(Action action)
        {
            if (Application.Current == null)
                return;

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(action);
                return;
            }
            action();
        }

    }
}
