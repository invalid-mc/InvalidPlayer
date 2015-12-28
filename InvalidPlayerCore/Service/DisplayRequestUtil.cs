using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Display;

namespace InvalidPlayerCore.Service
{
   public  static  class DisplayRequestUtil
    {
        private static DisplayRequest _displayRequest;
        public static void RequestActive()
        {
            try
            {
                if (_displayRequest == null)
                {
                    _displayRequest = new DisplayRequest();
                    _displayRequest.RequestActive();
                }
            }
            catch(Exception)
            {
                //
            }
        }
        public static void RequestRelease()
        {
            try
            {
                if (_displayRequest != null)
                {
                    _displayRequest.RequestRelease();
                    _displayRequest = null;
                }
            }
            catch (Exception)
            {
                //
            }
        }
    }
}
