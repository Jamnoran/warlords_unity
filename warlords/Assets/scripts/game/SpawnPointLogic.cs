using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointLogic : MonoBehaviour {

    public bool start = false;
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public GameObject point4;

    // Use this for initialization
    void Start () {
        getTestSpawn().registerSpawnPoint(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    TestSpawn getTestSpawn() {
        return ((TestSpawn)GameObject.Find("TestObject").GetComponent(typeof(TestSpawn)));
    }
}
