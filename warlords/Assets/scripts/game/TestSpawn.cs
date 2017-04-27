using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour {

    public bool foundSpawnPoint = false;
    public List<GameObject> spawns = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void registerSpawnPoint(GameObject spawnPoint) {
        if (spawns.Count == 0) {
            ((SpawnPointLogic)spawnPoint.GetComponent(typeof(SpawnPointLogic))).start = true;
            Debug.Log("Send to server we got spawn point");
        }
        spawns.Add(spawnPoint);
    }

}
