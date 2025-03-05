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
        public List<CustomPanelData> CustomPanels { get; }

        public CheatMonoData(MonoBehaviour mono, string name, List<CheatMethodData> methods, List<CustomPanelData> customs)
        {
            Instance = mono;
            Methods = methods;
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