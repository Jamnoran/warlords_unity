using UnityEngine;
using System.Collections;
using System;

public class TimeTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Started");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp("a")) {
            var dt = DateTime.Now;
            var test = (dt.Hour * 60 * 1000) + (dt.Second * 1000) + dt.Millisecond;
            Debug.Log("Test: " + test);
        }
    }
}
