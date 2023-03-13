using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldLogic : MonoBehaviour {

    public GameObject shield;
    public bool shieldOn = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setVisibility(bool show)
    {
        shieldOn = show;
        shield.SetActive(show);
    }
}
