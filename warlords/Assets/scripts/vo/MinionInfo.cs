using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionInfo : MonoBehaviour {

    public int minionId = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int getMinionId()
    {
        return minionId;
    }

    public void setMinionId(int id)
    {
        minionId = id;
    }
}
