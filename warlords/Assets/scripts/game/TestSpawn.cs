using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour {

    public bool foundSpawnPoint = false;
    public List<GameObject> spawns = new List<GameObject>();

	// Use this for initialization
	void Start () {
        Debug.Log("TestSpawn object started");
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void registerSpawnPoint(GameObject spawnPoint) {
        if (spawns.Count == 0) {
            SpawnPointLogic logic = ((SpawnPointLogic)spawnPoint.GetComponent(typeof(SpawnPointLogic)));
            logic.start = true;
            Debug.Log("Send to server we got spawn point");
            List<Point> spawnPoints = new List<Point>();
            spawnPoints.Add(new Point(new Vector3(logic.point1.transform.position.x, 0.0f, logic.point1.transform.position.z), Point.SPAWN_POINT));
            spawnPoints.Add(new Point(new Vector3(logic.point2.transform.position.x, 0.0f, logic.point2.transform.position.z), Point.SPAWN_POINT));
            spawnPoints.Add(new Point(new Vector3(logic.point3.transform.position.x, 0.0f, logic.point3.transform.position.z), Point.SPAWN_POINT));
            spawnPoints.Add(new Point(new Vector3(logic.point4.transform.position.x, 0.0f, logic.point4.transform.position.z), Point.SPAWN_POINT));
            if (getCommunication() != null) {
                getCommunication().sendSpawnPoints(spawnPoints);
            }
        }
        spawns.Add(spawnPoint);

    }

    ServerCommunication getCommunication() {
        if (GameObject.Find("Communication") != null) {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        }else {
            return null;
        }
        
    }
}
