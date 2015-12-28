using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using InvalidPlayerCore.Service;

namespace InvalidPlayerCore.Exceptions
{
    public class AppExceptionHandle
    {
        public AppExceptionHandle(Application app)
        {
            if (null == app)
            {
                return;
            }
            app.UnhandledException += app_UnhandledException;
        }

        private void app_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            if (null != exception)
            {
                if (exception is AppException)
                {
                    OnAppException(exception as AppException);
                }
                else if (exception is ServiceException)
                {
                    OnServiceException(exception as ServiceException);
                }
                else
                {
                    OnOtherException(exception);
                }
            }
            e.Handled = true;
        }

        protected void OnAppException(AppException exception)
        {
            var task = DispatcherHelper.RunAsync(async () => { await new MessageDialog("Error:\r\n" + exception.Message).ShowAsync(); });
        }

        protected void OnServiceException(ServiceException exception)
        {
            var message = GetLocalMessage(exception);
            var task = DispatcherHelper.RunAsync(async () => { await new MessageDialog("Error:\r\n" + message).ShowAsync(); });
        }

        protected void OnOtherException(Exception exception)
        {
            var task = DispatcherHelper.RunAsync(async () => { await new MessageDialog("Error:\r\n" + exception.Message).ShowAsync(); });
        }

        private string GetLocalMessage(ServiceException exception)
        {
            return exception.Message; //TODO
        }


        public void RegisterExceptionHandlingSynchronizationContext()
        {
            try
            {
                var context = ExceptionHandlingSynchronizationContext.Register();
                context.UnhandledException += SynchronizationContextUnhandledException;
            }
            catch (Exception)
            {
            }
        }

        private async void SynchronizationContextUnhandledException(object sender, AysncUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var ex = e.Exception;
            var message = ex.Message;
            if (message.Contains("A task was canceled"))    //TODO 
            {
            }
            else
            {
                await DispatcherHelper.RunAsync(async () => { await new MessageDialog("Error:\r\n" + message).ShowAsync(); });
            }
        }
    }
}