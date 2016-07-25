using UnityEngine;
using System.Collections;

public class DestroyPointer : MonoBehaviour {
    public Transform pointer;
    public int liveTime = 50;
    private int time = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (time < liveTime)
        {
            time = time + 1;
        }
        else
        {
            Destroy(pointer.gameObject);
        }
	}
}
