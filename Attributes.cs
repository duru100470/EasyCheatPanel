using System;

namespace EasyCheatPanel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CheatMethodAttribute : Attribute
    {
        public string Display;

        public CheatMethodAttribute(string display = "")
        {
            Display = display;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CustomCheatPanelAttribute : Attribute
    {
        public CustomCheatPanelAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CheatFieldAttribute : Attribute
    {
        public string Display;

        public CheatFieldAttribute(string display = "")
        {
            Display = display;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class DropdownAttribute : Attribute
    {
        public object[] Items;

        public DropdownAttribute(params object[] dropdown)
        {
            Items = dropdown;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class DynamicDropdownAttribute : Attribute
    {
        public Type ProviderType { get; }

        public DynamicDropdownAttribute(Type providerType)
        {
            if (!typeof(IDropdownProvider).IsAssignableFrom(providerType))
            {
                throw new ArgumentException("Provider type must implement IDropdownProvider.");
            }
            ProviderType = providerType;
        }
    }

    public interface IDropdownProvider
    {
        object[] GetItems();
    }
}
