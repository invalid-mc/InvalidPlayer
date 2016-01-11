using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using InvalidPlayerCore.Service;

namespace InvalidPlayerCore.Controls
{
    public static class PopupManager
    {
        public static bool HasPopup(string tag)
        {
            if (null == tag) return false;

            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            return popups.Any(popup => tag.Equals(popup.Tag));
        }

        public static void ClosePopup(string tag)
        {
            if (null == tag) return;

            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popup in popups.Where(popup => tag.Equals(popup.Tag)))
            {
                popup.IsOpen = false;
            }
        }

        public static bool CanClosePopup(string tag)
        {
            if (null == tag) return false;

            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            return popups.Where(popup => tag.Equals(popup.Tag)).All(popup => popup.IsLightDismissEnabled);
        }
    }

    public abstract class YukiPopup : Control
    {
        public event EventHandler<object> Closed;

        public event EventHandler<object> Opened;

        private Popup _pop;

        public bool IsLightDismissEnabled
        {
            get { return true; }
            set { IsLightDismissEnabled = value; }
        }

        public bool IsToggleBorromBar { get; set; }

        public bool FullScreen
        {
            get { return true; }
            set { FullScreen = value; }
        }

        protected abstract string GetTag();

        public void Show()
        {
            _pop = new Popup { Tag = GetTag() };
            if (_pop.Transitions != null)
                _pop.Transitions.Add(new PopupThemeTransition());
            
            _pop.Opened += Pop_Opened;
            _pop.Closed += Pop_Closed;
            //     _pop.VerticalOffset = 0;
            //      _pop.HorizontalOffset = 0;
            var rootFrame = UiElementUtil.RootFrame;
            if (rootFrame != null)
            {
                rootFrame.SizeChanged += RootFrame_SizeChanged;
            }
            this.Width = rootFrame != null ? rootFrame.ActualWidth : 400;
            if (FullScreen)
            {
                this.Height = rootFrame != null ? rootFrame.ActualHeight : 800;
            }

            _pop.Child = this;
            _pop.IsLightDismissEnabled = IsLightDismissEnabled;
            _pop.IsOpen = true;
        }

        private void RootFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _pop.Width = e.NewSize.Width;
            if (FullScreen)
            {
                _pop.Height = e.NewSize.Height;
            }
        }

        private void ToggleBottonBar(bool show)
        {
            var frame = Window.Current.Content as Frame;
            var page = frame != null ? frame.Content as Page : null;
            var bar = page != null ? page.BottomAppBar : null;
            if (null != bar)
            {
                bar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Pop_Closed(object sender, object e)
        {
            if (IsToggleBorromBar)
            {
                ToggleBottonBar(true);
            }
            if (Closed != null)
                Closed.Invoke(this, e);
            var rootFrame = UiElementUtil.RootFrame;
            if (rootFrame != null)
            {
                rootFrame.SizeChanged -= RootFrame_SizeChanged;
            }
            Clean();
        }

        private void Pop_Opened(object sender, object e)
        {
            if (IsToggleBorromBar)
            {
                ToggleBottonBar(false);
            }
            if (Opened != null)
                Opened.Invoke(this, e);
        }


        public void Close()
        {
            if (null != _pop)
            {
                _pop.IsOpen = false;
            }
        }

        public void Clean()
        {
            this.DataContext = null;
            this.Closed = null;
            this.Opened = null;
            if (null != _pop)
            {
                _pop.Closed -= Pop_Closed;
                _pop.Opened -= Pop_Opened;
            }
        }
    }
}
