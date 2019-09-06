using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace FF_Fishing.Pages.UIBuilders.Proxy
{
    public class NotifyPropertyChangedProxy : DynamicObject, INotifyPropertyChanged
    {
        public object WrappedObject { get; }

        public NotifyPropertyChangedProxy(object wrappedObject)
        {
            WrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }


        #region INotifyPropertyChanged support

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return from f in WrappedObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   select f.Name;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyInfo = WrappedObject.GetType().GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public | (binder.IgnoreCase ? BindingFlags.IgnoreCase : 0));
            if (propertyInfo == null || !propertyInfo.CanRead)
            {
                result = null;
                return false;
            }

            result = propertyInfo.GetValue(WrappedObject, null);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyInfo = WrappedObject.GetType().GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public | (binder.IgnoreCase ? BindingFlags.IgnoreCase : 0));
            if (propertyInfo == null || !propertyInfo.CanWrite)
                return false;

            var newValue = value;
            var propertyType = propertyInfo.PropertyType;
            if (!propertyType.IsInstanceOfType(value))
            {
                newValue = Convert.ChangeType(value, propertyType);
            }

            propertyInfo.SetValue(WrappedObject, newValue, null);
            OnPropertyChanged(binder.Name);
            return true;
        }

        public Type GetWrappedType()
        {
            return WrappedObject.GetType();
        }
    }
}
