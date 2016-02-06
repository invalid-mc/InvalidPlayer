using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace InvalidPlayer.View
{
    public sealed partial class Player
    {
        public void Init() {
            var control = MainPlayer.TransportControls;
            control.IsCompact = true;
            control.Background = new SolidColorBrush(Color.FromArgb(100,0,0,0));
            control.IsFastForwardButtonVisible = true;
            control.IsFastRewindButtonVisible = true;
            control.IsPlaybackRateButtonVisible = true;
            control.IsStopButtonVisible = true;
            control.IsZoomButtonVisible = true;
            control.RegisterPropertyChangedCallback(MediaTransportControls.VisibilityProperty, (s,d) => {
                InfoPanel.Visibility = (Visibility)s.GetValue(d);
            });
        }
    }
}