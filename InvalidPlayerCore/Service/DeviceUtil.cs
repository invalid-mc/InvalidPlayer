using Windows.System.Profile;

namespace InvalidPlayerCore.Service
{
    public static class DeviceUtil
    {
        public static string GetDeviceId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var id = SecurityKit.ComputeMd5(token.Id);
            return id;
        }
    }
}