using System.Reflection;

namespace EasyCheatPanel
{
    public interface IValueAccessor
    {
        object GetValue(object instance);
        string GetName();
    }

    public class FieldAccessor : IValueAccessor
    {
        private readonly FieldInfo _fieldInfo;

        public FieldAccessor(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public string GetName()
        {
            return _fieldInfo.Name;
        }

        public object GetValue(object instance)
        {
            return _fieldInfo.GetValue(instance);
        }
    }

    public class PropertyAccessor : IValueAccessor
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public string GetName()
        {
            return _propertyInfo.Name;
        }

        public object GetValue(object instance)
        {
            return _propertyInfo.GetValue(instance);
        }
    }
}