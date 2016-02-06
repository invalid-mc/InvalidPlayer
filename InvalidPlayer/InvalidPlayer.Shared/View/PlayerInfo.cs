using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace InvalidPlayer.View
{
    public sealed partial class Player
    {
        private void InitInfo()
        {
            this.Loaded += delegate
            {
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
                Windows.Phone.UI.Input.HardwareButtons.CameraReleased += HardwareButtons_CameraReleased;
#endif

#if WINDOWS_UWP
                Init();
#endif
            };
            this.Unloaded += delegate
            {
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;

#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.CameraPressed -= HardwareButtons_CameraPressed;
                Windows.Phone.UI.Input.HardwareButtons.CameraReleased -= HardwareButtons_CameraReleased;
#endif
            };
            this.SizeChanged += delegate { UpdateInfo(); };
        }

#if WINDOWS_PHONE_APP

        private void HardwareButtons_CameraPressed(object sender, Windows.Phone.UI.Input.CameraEventArgs e)
        {
            SetInfoVisible(true);
        }

        private void HardwareButtons_CameraReleased(object sender, Windows.Phone.UI.Input.CameraEventArgs e)
        {
            SetInfoVisible(false);
        }

#endif

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            Debug.WriteLine("CoreW: " + MainPlayer.IsFullWindow + "\t" + e.VirtualKey);
            var elem = FocusManager.GetFocusedElement();
            Debug.WriteLine(elem);
            if (elem == WebUrlTextBox || elem == SearchBtn) return;

            if (e.VirtualKey == VirtualKey.F1)
            {
                SetInfoVisible(true);
            }
            else if (e.VirtualKey == VirtualKey.Enter)
            {
                if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) == CoreVirtualKeyStates.Down)
                    MainPlayer.IsFullWindow = true;
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs e)
        {
            var elem = FocusManager.GetFocusedElement();
            if (elem == WebUrlTextBox || elem == SearchBtn) return;

            if (e.VirtualKey == VirtualKey.F1)
            {
                SetInfoVisible(false);
            }
            else if (e.VirtualKey == VirtualKey.Escape)
            {
                MainPlayer.IsFullWindow = false;
            }
        }

        private void player_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
            UpdateProgressInfo();
        }

        private void player_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
            //UpdateProgressInfo();
        }

        public void UpdateProgressInfo()
        {
            var bp = MainPlayer.BufferingProgress;
            if (bp<0.01||bp > 0.999)
            {
                ProgressInfo.Visibility = Visibility.Collapsed;
                return;
            }

            ProgressInfo.Visibility = Visibility.Visible;
            ProgressInfoText.Text = string.Format("{0}\n{1}", MainPlayer.BufferingProgress.ToString("P"), MainPlayer.DownloadProgress.ToString("P"));
        }

        private void player_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            InputGrid.Visibility = MainPlayer.CurrentState == MediaElementState.Playing
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

#region Format String

        private const string VideoInfoStr = "原始分辨率：{0} * {1}\n" +
                                            "视频纵横比：{2} * {3}\n" +
                                            "视频长度：{4}\n" +
                                            "是否 3D：{5}\n" +
                                            "音频流数目：{6}\n" +
                                            "当前使用音频流索引：{7}\n" +
                                            "音频流名称：{8}\n" +
                                            "视频源：{9}\n";

        private const string PlayInfoStr = "渲染分辨率：{0} * {1}\n" +
                                           "当前缓冲进度：{2}%\n" +
                                           "已下载：{3}%\n" +
                                           "下载进度偏移：{4}\n" +
                                           "播放速率：{5}\n";

        private const string ElemInfoStr = "自动播放：{0}\n" +
                                           "重复播放：{1}\n" +
                                           "默认播放速率：{2}\n" +
                                           "视频拉伸方式：{3}\n";

#endregion

        private void SetInfoVisible(bool visible)
        {
            if (visible && MainPlayer.CurrentState != MediaElementState.Closed)
            {
                InfoPanel.Visibility = Visibility.Visible;
                UpdateInfo();
            }
            else
            {
                InfoPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateInfo(bool force = false)
        {
            if (!force && InfoPanel.Visibility == Visibility.Collapsed) return;

            VideoInfo.Text = string.Format(VideoInfoStr,
                MainPlayer.NaturalVideoWidth, MainPlayer.NaturalVideoHeight,
                MainPlayer.AspectRatioWidth, MainPlayer.AspectRatioHeight,
                MainPlayer.NaturalDuration, MainPlayer.IsStereo3DVideo,
                MainPlayer.AudioStreamCount, MainPlayer.AudioStreamIndex,
                MainPlayer.GetAudioStreamLanguage(MainPlayer.AudioStreamIndex), MainPlayer.Source);
            PlayInfo.Text = string.Format(PlayInfoStr,
                (int) MainPlayer.ActualWidth, (int) MainPlayer.ActualHeight,
                MainPlayer.BufferingProgress*100, (MainPlayer.DownloadProgress*100).ToString("F"),
                MainPlayer.DownloadProgressOffset, MainPlayer.PlaybackRate);
            MediaElemInfo.Text = string.Format(ElemInfoStr,
                MainPlayer.AutoPlay, MainPlayer.IsLooping,
                MainPlayer.DefaultPlaybackRate, MainPlayer.Stretch);
        }

     
    }
}