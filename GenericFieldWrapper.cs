using UnityEngine.UIElements;

namespace EasyCheatPanel
{
    public class GenericFieldWrapper<T> : IFieldWrapper
    {
        private readonly BaseField<T> field;

        public GenericFieldWrapper(BaseField<T> field)
        {
            this.field = field;
        }

        public object Value
        {
            get { return field.value; }
            set { field.value = (T)value; }
        }
        public BaseField<T> Field => field;
    }

    public interface IFieldWrapper
    {
        object Value { get; set; }
    }
}
