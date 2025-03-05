using System.Collections;
using System.Collections.Generic;
using EasyCheatPanel;
using UnityEngine;
using UnityEngine.UIElements;

public class Test2 : MonoBehaviour
{
    [CheatField]
    public int Count = 5;
    [CheatField]
    public int ActualCount => 10;
    [CheatField]
    private string Name => "ASDASD";

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

    [CustomCheatPanel]
    private VisualElement TestCustomPanel()
    {
        var panel = new VisualElement();
        panel.Add(new Label("TEST"));

        var btn = new Button(() =>
        {
            TestMethod("Hello");
        });
        btn.style.height = 50;
        panel.Add(btn);

        return panel;
    }

    [CustomCheatPanel]
    private VisualElement TestCustomPanel2()
    {
        var panel = new VisualElement();
        panel.Add(new Label("TEST"));

        var btn = new Button(() =>
        {
            TestMethod("Hello");
        });
        btn.style.height = 50;
        panel.Add(btn);

        return panel;
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
