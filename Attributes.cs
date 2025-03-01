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

    [AttributeUsage(AttributeTargets.Parameter)]
    public class DropdownAttribute : Attribute
    {
        public object[] Items;

        public DropdownAttribute(params object[] dropdown)
        {
            Items = dropdown;
        }
    }
}
