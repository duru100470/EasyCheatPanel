using System.Collections;
using System.Collections.Generic;
using EasyCheatPanel;
using UnityEngine;

public class Test : MonoBehaviour
{
    [CheatMethod]
    public void TestMethod()
    {
        Debug.Log("TEST!!!");
    }

    [CheatMethod("Number test")]
    public void NumberTestMethod(int integer)
    {
        Debug.Log($"Number :{integer}");
    }

    [CheatMethod("Dropdown Test")]
    public void DropdownTestMethod([Dropdown(1, 2, 3, 4)] int integer)
    {
        Debug.Log($"Number :{integer}");
    }

    [ContextMenu("TEST")]
    public void TestUtility()
    {
        var list = CheatPanelUtility.GetCheatMonoDataList();
        foreach (var item in list)
        {
            foreach (var m in item.Methods)
            {
                m.Method.Invoke(item.Instance, null);
            }
        }
    }
}
