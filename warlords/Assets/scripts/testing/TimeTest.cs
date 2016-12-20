using UnityEngine;
using System.Collections;
using System;

public class TimeTest : MonoBehaviour {

    private long timeLast = 0;
	// Use this for initialization
	void Start () {
        Debug.Log("Started");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp("a")) {
            //var dt = DateTime.Now;
            //long test = (dt.Hour * 60 * 1000) + (dt.Second * 1000) + dt.Millisecond;

            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long test = (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;


            //Debug.Log("Time now: " + test);
            Debug.Log("Time now: " + test + " Time since last press : " + (test - timeLast));
            timeLast = test;
        }
    }
}
