using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpThenDeactivate : MonoBehaviour {

    public int ticks = 50;
    public int tickCounter = 0;
    public float moveWith = 0.01f;
    public float timeTick = 0.01f;
    public float rotateSpeed = 40.0f;
    public bool rotate = false;
    public GameObject parent;

    // Use this for initialization
    void Start () {
        InvokeRepeating("tick", 0, timeTick);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void tick()
    {
        tickCounter++;
        if (tickCounter == ticks)
        {
            gameObject.SetActive(false);
            CancelInvoke("tick");
            Destroy(parent);
        }
        Vector3 newPosition = transform.position;
        newPosition.y = newPosition.y + moveWith;
        transform.position = newPosition;
        if (rotate)
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
    }
}
