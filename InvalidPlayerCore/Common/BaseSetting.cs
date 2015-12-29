using System.ComponentModel;
using System.Reflection;

namespace InvalidPlayerCore.Common
{
    public abstract class BaseSetting
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected abstract object GetValue(string name);

        protected abstract void SetValue(string key, object value);

        public virtual void Init()
        {
            var type = GetType();
            var pro = type.GetTypeInfo().DeclaredProperties;
            foreach (var propertyInfo in pro)
            {
                var name = propertyInfo.Name;
                var value = GetValue(name);
                propertyInfo.SetValue(this, value);
            }
        }

        protected void PropertyChangedAndSave<T>(string name, T value)
        {
            SetValue(name, value);
            OnPropertyChanged(name);
        }

        protected void PropertyChangedAndSave(string name, object oldvalue, object newvalue)
        {
            if (newvalue != oldvalue)
            {
                SetValue(name, newvalue);
                OnPropertyChanged(name);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}