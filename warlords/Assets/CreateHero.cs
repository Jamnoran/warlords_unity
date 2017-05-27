using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateHero : MonoBehaviour {

    public string classType = "WARRIOR";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setClassWarrior(bool value) {
        Debug.Log("WARRIOR chosen " + value);
        classType = "WARRIOR";
    }

    public void setClassPriest() {
        Debug.Log("PRIEST chosen");
        classType = "PRIEST";
    }

    public void setClassWarlock() {
        Debug.Log("WARLOCK chosen");
        classType = "WARLOCK";
    }

    public void createHero() {

        Toggle[] toggles = GameObject.FindObjectsOfType<Toggle>();
        Debug.Log("Found toggles: " + toggles.Length);

        Debug.Log("Create hero with class : " + classType);
    }
}
