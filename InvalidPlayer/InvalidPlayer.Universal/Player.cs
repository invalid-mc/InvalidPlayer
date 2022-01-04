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
        public void Init()
        {
            var control = MainPlayer.TransportControls;
            control.IsCompact = false;
            control.IsFastForwardButtonVisible = true;
            control.IsFastForwardEnabled = true;
            control.IsFastRewindButtonVisible = true;
            control.IsFastRewindEnabled = true;
            control.IsPlaybackRateButtonVisible = true;
            control.IsPlaybackRateEnabled = true;
            control.IsStopButtonVisible = true;
            control.IsStopEnabled = true;
            control.IsZoomButtonVisible = true;
            control.IsCompactOverlayButtonVisible = true;
            control.IsCompactOverlayEnabled = true;
            control.RegisterPropertyChangedCallback(MediaTransportControls.VisibilityProperty, (s, d) =>
            {
                InfoPanel.Visibility = (Visibility)s.GetValue(d);
            });
        }
    }
}