using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;
using LitJson;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string json = "{ \"test\":2.54 }";
        //TestObject responseGameStatus = JsonMapper.ToObject<TestObject>(json);
        TestObject testResult = JsonUtility.FromJson<TestObject>(json);
        Debug.Log("ParsedJson: " + testResult.test);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class TestObject
{
    public string test;
}
