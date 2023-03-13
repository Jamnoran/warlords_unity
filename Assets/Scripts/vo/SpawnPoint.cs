using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnPoint : MonoBehaviour {

    public bool enemy = false;
    public bool start = false;
    public bool end = false;

    private int type = Point.SPAWN_POINT;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int getType() {
        if (enemy) {
            return Point.ENEMY_POINT;
        } else if (start) {
            return Point.SPAWN_POINT;
        }else if (end) {
            return Point.END_POINT;
        }
        return 0;
    }
}
