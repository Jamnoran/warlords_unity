using UnityEngine;
using System.Collections;

public class DestroyPointer : MonoBehaviour {
    public GameObject gameobjectToDestroy;
    public float liveTime = 0.50f;

	// Use this for initialization
	void Start () {
        if (gameobjectToDestroy != null)
        {
            Destroy(gameobjectToDestroy, liveTime);
        }
        else
        {
            Destroy(gameObject, liveTime);
        }
	}

	// Update is called once per frame
	void Update () {
	}
}
