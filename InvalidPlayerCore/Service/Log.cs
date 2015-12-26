
namespace InvalidPlayer.Service
{
    public static class Log
    {
        public static void Debug(string message, params object[] param)
        {
            System.Diagnostics.Debug.WriteLine(message, param);
        }
    }
}