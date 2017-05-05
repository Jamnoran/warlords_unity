using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointLogic : MonoBehaviour {

    public bool start = false;
    public bool enemy = false;
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public GameObject point4;

    // Use this for initialization
    void Start () {
        //if (enemy) {
       //     getTestSpawn().registerSpawnPoint(gameObject, Point.ENEMY_POINT);
       // } else {
        //    getTestSpawn().registerSpawnPoint(gameObject, Point.SPAWN_POINT);
       // }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    TestSpawn getTestSpawn() {
        return ((TestSpawn)GameObject.Find("TestObject").GetComponent(typeof(TestSpawn)));
    }
}
