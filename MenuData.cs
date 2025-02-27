using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyCheatPanel
{
    public class MenuData
    {
        public string Name { get; }
        public List<CheatMethodData> Methods { get; } = new List<CheatMethodData>();

        public MenuData(string name)
        {
            Name = name;
        }
    }
}