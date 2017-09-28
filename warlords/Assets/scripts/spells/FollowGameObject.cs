using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour {

    public Transform objectToFollow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (objectToFollow != null)
        {
            Vector3 positionOfObject = objectToFollow.position;
            positionOfObject.y = positionOfObject.y - 1;
            transform.position = positionOfObject;
        }	
	}
}
