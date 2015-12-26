using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace InvalidPlayer.Service
{
    public static class DispatcherHelper
    {
        public static bool IsUiThread()
        {
            var window = CoreWindow.GetForCurrentThread();
            return window != null && window.Dispatcher != null && window.Dispatcher.HasThreadAccess;
        }

        public static async Task RunAsync(Action action)

        {
            if (IsUiThread())
            {
                action.Invoke();
            }
            else
            {
                var dispatcher = Window.Current.Dispatcher;
                if (null != dispatcher)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
                }
            }
        }
    }
}