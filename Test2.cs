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
    private void Test(int integer, string name, string description)
    {
        Debug.Log(integer);
        Debug.Log(name);
        Debug.Log(description);
    }
}
