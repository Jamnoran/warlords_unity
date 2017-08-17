using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocator : MonoBehaviour {

    public bool foundSpawnPoint = false;
    public List<GameObject> spawns = new List<GameObject>();

	// Use this for initialization
	void Start () {
        Debug.Log("SpawnLocator object started");
        //StartCoroutine("sendSpawnPoints");
    }

    // Update is called once per frame
    void Update() {
    }

    public void startJobForSpawnPoints() {
        StartCoroutine("sendSpawnPoints");
    }

    IEnumerator sendSpawnPoints() {
        //yield return new WaitForSeconds(1.0f);
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Trying to find all spawn points");
        GameObject[] points = GameObject.FindGameObjectsWithTag("Point");
        int countSpawn = 0;
        int countEnemy = 0;
        Debug.Log("Found this many points : " + points.Length);


        List<Point> spawnPoints = new List<Point>();
        foreach (GameObject point in points) {
            SpawnPoint spawnPoint = (SpawnPoint)point.GetComponent(typeof(SpawnPoint));
            spawnPoints.Add(new Point(new Vector3(point.transform.position.x, point.transform.position.y, point.transform.position.z), spawnPoint.getType()));
            if (spawnPoint.getType() == Point.SPAWN_POINT) {
                countSpawn++;
            }else if (spawnPoint.getType() == Point.ENEMY_POINT) {
                countEnemy++;
            }
        }
        if (getCommunication() != null) {
            getCommunication().sendSpawnPoints(spawnPoints);
        }
        Debug.Log("Found : " + spawnPoints.Count +  " Spawn: " + countSpawn + " Enemy : " + countEnemy);
    }

    public void registerSpawnPoint(GameObject spawnPoint, int type) {
        Debug.Log("Got register of type : " + type);
        if (type == Point.SPAWN_POINT) {
            if (spawns.Count == 0) {
                SpawnPointLogic logic = ((SpawnPointLogic)spawnPoint.GetComponent(typeof(SpawnPointLogic)));
                logic.start = true;
                Debug.Log("Send to server we got spawn point");
                List<Point> spawnPoints = new List<Point>();
                if (logic.point1 != null) {
                    spawnPoints.Add(new Point(new Vector3(logic.point1.transform.position.x, logic.point1.transform.position.y, logic.point1.transform.position.z), type));
                }
                if (logic.point2 != null) {
                    spawnPoints.Add(new Point(new Vector3(logic.point2.transform.position.x, logic.point2.transform.position.y, logic.point2.transform.position.z), type));
                }
                if (logic.point3 != null) {
                    spawnPoints.Add(new Point(new Vector3(logic.point3.transform.position.x, logic.point3.transform.position.y, logic.point3.transform.position.z), type));
                }
                if (logic.point4 != null) {
                    spawnPoints.Add(new Point(new Vector3(logic.point4.transform.position.x, logic.point4.transform.position.y, logic.point4.transform.position.z), type));
                }
                if (getCommunication() != null) {
                    getCommunication().sendSpawnPoints(spawnPoints);
                }
            } 
        } else {
            List<Point> spawnPoints = new List<Point>();
            spawnPoints.Add(new Point(new Vector3(spawnPoint.transform.position.x, 0.0f, spawnPoint.transform.position.z), type));
            if (getCommunication() != null) {
                getCommunication().sendSpawnPoints(spawnPoints);
            }
            Debug.Log("Sent up enemy spawn point");
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
