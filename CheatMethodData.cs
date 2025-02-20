using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyCheatPanel
{
    public class CheatMonoData
    {
        public MonoBehaviour Instance { get; }
        public string Name { get; }
        public List<CheatMethodData> Methods { get; }

        public CheatMonoData(MonoBehaviour mono, string name, List<CheatMethodData> methods)
        {
            Instance = mono;
            Methods = methods;
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
}