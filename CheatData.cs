using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyCheatPanel
{
    public class CheatMonoData
    {
        public MonoBehaviour Instance { get; }
        public string Name { get; }
        public List<CheatMethodData> Methods { get; }
        public List<CheatFieldData> Fields { get; }
        public List<CustomPanelData> CustomPanels { get; }

        public CheatMonoData(MonoBehaviour mono, string name, List<CheatMethodData> methods, List<CheatFieldData> fields, List<CustomPanelData> customs)
        {
            Instance = mono;
            Methods = methods;
            Fields = fields;
            CustomPanels = customs;
            Name = name;
        }
    }

    public class CheatMethodData
    {
        public string DisplayName { get; }
        public MethodInfo Method { get; }

        public CheatMethodData(MethodInfo method, string displayName = "")
        {
            Method = method;

            if (string.IsNullOrEmpty(displayName))
                DisplayName = method.Name;
            else
                DisplayName = displayName;
        }
    }

    public class CheatFieldData
    {
        public string DisplayName { get; }
        public IValueAccessor Accessor { get; }

        public CheatFieldData(IValueAccessor valueAccessor, string displayName = "")
        {
            Accessor = valueAccessor;

            if (string.IsNullOrEmpty(displayName))
                DisplayName = Accessor.GetName();
            else
                DisplayName = displayName;
        }
    }

    public class CustomPanelData
    {
        public MethodInfo Method { get; }

        public CustomPanelData(MethodInfo method)
        {
            if (method.ReturnType != typeof(VisualElement))
                throw new InvalidCastException($"{method.ReturnType} is not a VisualElement");
            Method = method;
        }
    }
}