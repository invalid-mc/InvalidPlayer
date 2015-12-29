using System;
using Windows.Storage;

namespace InvalidPlayerCore.Common
{
    public abstract class AbstractLocalSetting : BaseSetting
    {
        private readonly ApplicationDataContainer _container;
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public AbstractLocalSetting()
        {
            var key = GetKey();
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key","key for localSettingsContainer is null");
            }
            _container = _localSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);
        }

        public abstract string GetKey();

        protected override object GetValue(string name)
        {
            object ov;
            _container.Values.TryGetValue(name, out ov);
            return ov;
        }

        protected override void SetValue(string key, object value)
        {
            _container.Values[key] = value;
        }
    }
}