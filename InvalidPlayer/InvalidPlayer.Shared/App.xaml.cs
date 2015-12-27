using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using InvalidPlayer.View;
using InvalidPlayerCore.Exceptions;
using SYEngineCore;

namespace InvalidPlayer
{
    public sealed partial class App : Application
    {
        private readonly AppExceptionHandle _appExceptionHandle;

        public App()
        {
            InitializeComponent();
            _appExceptionHandle = new AppExceptionHandle(this);
            Suspending += OnSuspending;
            Init();
        }

        public void Init()
        {
            Task.Run(() => { Core.Initialize(); });
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = BuildRootFrame(e.PreviousExecutionState);
            BuildRootContent(rootFrame, e.Arguments);
            Window.Current.Activate();
        }


        protected override void OnActivated(IActivatedEventArgs e)
        {
            var rootFrame = BuildRootFrame(e.PreviousExecutionState);
            if (e.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = e as ProtocolActivatedEventArgs;
                var uri = protocolArgs.Uri;
                var scheme = uri.Scheme;
                if (scheme == "weburl")
                {
                    rootFrame.Navigate(typeof (Player), uri);
                }
                else
                {
                    BuildRootContent(rootFrame, null);
                }
            }
            else
            {
                BuildRootContent(rootFrame, null);
            }
            Window.Current.Activate();
        }

        private static void BuildRootContent(Frame rootFrame, object param)
        {
            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                rootFrame.ContentTransitions = null;
#endif
                if (!rootFrame.Navigate(typeof (Player), param))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
        }

        private Frame BuildRootFrame(ApplicationExecutionState state)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame {CacheSize = 3};
                _appExceptionHandle.RegisterExceptionHandlingSynchronizationContext();
                if (state == ApplicationExecutionState.Terminated)
                {
                }
                Window.Current.Content = rootFrame;
            }
            return rootFrame;
        }


        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}