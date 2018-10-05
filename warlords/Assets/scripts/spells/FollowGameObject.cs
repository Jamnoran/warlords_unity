using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour {

    public Transform objectToFollow;
    public Transform objectToLookAt;
    public Vector3 positionToLookAt;
    public long timeToLive = 500;

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
        if (objectToLookAt != null)
        {
            //transform.LookAt(objectToLookAt.transform);

            Vector3 rotatingPostition = new Vector3(objectToLookAt.transform.position.x, transform.position.y, objectToLookAt.transform.position.z);
            transform.LookAt(rotatingPostition);
        }
        if(positionToLookAt != null)
        {
            transform.LookAt(positionToLookAt);
        }

	}

    public void setUp(Transform lookAt, Transform follow, long time)
    {
        objectToLookAt = lookAt;
        objectToFollow = follow;
        timeToLive = time;
    }

    public void setObjectToLookAt(Transform transform)
    {
        objectToLookAt = transform;
    }
    public void setPositionToLookAt(Vector3 position)
    {
        positionToLookAt = position;
    }

    public void setTimeToLive(long time)
    {
        timeToLive = time;
    }
    
    public void setObjectToFollow(Transform transform)
    {
        objectToFollow = transform;
    }

    public void setObjectToFollow(GameObject gameObject)
    {
        objectToFollow = transform;
    }
}
