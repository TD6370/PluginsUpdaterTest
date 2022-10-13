using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PluginUpdater.Engine
{
    public static class WpfHelper
    {
        public static void InvokeMethod(Action method)
        {
            if (null == Application.Current)
            {
                // create Application moved to Console.Program.cs Main();
                //new Application();
                //Logger.Notice("Application.Current is null. Skip InvokeMethod");
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            if (!Application.Current.Dispatcher.CheckAccess())
            { // CheckAccess returns true if you're on the dispatcher thread
                Application.Current.Dispatcher.BeginInvoke(method);
                return;
            }

            method();
        }

    }
}
