using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using InvalidPlayerCore.Controls;

namespace InvalidPlayer.Controls
{
    [TemplatePart(Name = "PlayButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "StopButton", Type = typeof(Button))]
    [TemplatePart(Name = "FullButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "MuteButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "VolumeBar", Type = typeof(ProgressBar))]
    [TemplatePart(Name = "ScheduleBar", Type = typeof(ProgressBar))]
    public class MediaControl : YukiPopup
    {
        private const string PlayButtonName = "PlayButton";
        private const string StopButtonName = "StopButton";
        private const string FullButtonName = "FullButton";
        private const string MuteButtonName = "MuteButton";
        private const string VolumeBarName = "VolumeBar";
        private const string ScheduleBarName = "ScheduleBar";
        private ToggleButton _playButton;
        private ToggleButton _fullButton;
        private ToggleButton _muteButton;
        private Button _stopButton;
        private ProgressBar _volumeBar;
        private ProgressBar _scheduleBar;

        private bool _loaded;
        private MediaElement _mediaElement;

        public MediaControl() : this(null) { }

        public MediaControl(MediaElement mediaElement)
        {
            this.DefaultStyleKey = typeof(MediaControl);
            SetMediaElement(mediaElement);
        }

        public void SetMediaElement(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;
            SetStates();
        }

        protected override string GetTag()
        {
            return "MediaControl";
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _playButton = GetTemplateChild(PlayButtonName) as ToggleButton;
            _stopButton = GetTemplateChild(StopButtonName) as Button;
            _fullButton = GetTemplateChild(FullButtonName) as ToggleButton;
            _muteButton = GetTemplateChild(MuteButtonName) as ToggleButton;
            _volumeBar = GetTemplateChild(VolumeBarName) as ProgressBar;
            _scheduleBar = GetTemplateChild(ScheduleBarName) as ProgressBar;

            _loaded = true;
            SetStates();
        }

        private void SetStates()
        {
            if (!_loaded) return;

            // Clear Bindings
            if (null == _mediaElement)
            {
                _fullButton.IsChecked = false;
                _muteButton.IsChecked = false;
                return;
            }

            _playButton.IsChecked = _mediaElement.CurrentState == MediaElementState.Playing;
            var fullBinding = new Binding
            {
                Source = _mediaElement.IsFullWindow,
                Mode = BindingMode.TwoWay
            };
            _fullButton.SetBinding(ToggleButton.IsCheckedProperty, fullBinding);
            var muteBinding = new Binding
            {
                Source = _mediaElement.IsMuted,
                Mode = BindingMode.TwoWay
            };
            _muteButton.SetBinding(ToggleButton.IsCheckedProperty, muteBinding);
        }
    }
}
