using System.Collections;
using System.Collections.Generic;
using EasyCheatPanel;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [CheatMethod("Another Test")]
    private void Test(int integer, string name, bool isReal, TestEnum testEnum = TestEnum.Seven, string description = "asdasd")
    {
        Debug.Log(integer);
        Debug.Log(name);
        Debug.Log(description);
        Debug.Log(isReal);
        Debug.Log(testEnum);
    }

    [CheatMethod("Another Test 2")]
    private void TestMethod([DynamicDropdown(typeof(TestDropdownProvider))] string name)
    {
        Debug.Log(name);
    }

    public class TestDropdownProvider : IDropdownProvider
    {
        public object[] GetItems()
        {
            return new object[] { "hello", "world", "color", "sweeper" };
        }
    }
}


public enum TestEnum
{
    None,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
}
