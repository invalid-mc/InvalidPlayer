using System;
using System.Collections.Generic;
using System.Text;
using Windows.System.Profile;
using Yuki.Common.Util;

namespace InvalidPlayer.Service
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
