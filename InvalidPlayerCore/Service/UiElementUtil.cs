using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace InvalidPlayerCore.Service
{
    public static class UiElementUtil
    {
        public static FrameworkElement RootFrame
        {
            get { return Window.Current.Content as FrameworkElement; }
        }

        public static Size FrameSize
        {
            get
            {
                return RootFrame == null ? default(Size) : new Size(RootFrame.ActualWidth, RootFrame.ActualHeight);
            }
        }

        public static double GetContentViewWidth()
        {
            // var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            //  var h = Window.Current.Bounds.Width;
            return RootFrame != null ? RootFrame.ActualWidth : 400;
        }

        public static double GetContentViewHeight()
        {
            return RootFrame != null ? RootFrame.ActualHeight : 800;
        }

        public static T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parentElement);
            if (childrenCount == 0)
            {
                return default(T);
            }
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parentElement, i);
                if (child != null && child is T)
                {
                    return (T) child;
                }
                var t = FindFirstElementInVisualTree<T>(child);
                if (t != null)
                {
                    return t;
                }
            }
            return default(T);
        }
    }
}