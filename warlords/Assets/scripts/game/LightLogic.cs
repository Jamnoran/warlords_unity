using UnityEngine;
using System.Collections;

public class LightLogic : MonoBehaviour {
    public Light lightObject;
    private bool turnedOn = false;
	// Use this for initialization
	void Start () {
        if(lightObject != null)
        {
            lightObject.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
    }

    void OnTriggerEnter(Collider other)
    {
        if (!turnedOn)
        {
           // Debug.Log("Hero entered, turn on light : " + other.gameObject.layer);
            if (other.gameObject.layer == 10) {
                lightObject.enabled = true;
                turnedOn = true;
            }
        }
    }
}
