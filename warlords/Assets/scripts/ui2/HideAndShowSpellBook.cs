using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAndShowSpellBook : MonoBehaviour {

    public GameObject spellBookOriginalPosition;
    private Vector3 hiddenPosition = new Vector3(1000000,0,0);
    private bool isHidden;
	// Use this for initialization
	void Start () {
        this.transform.position = hiddenPosition;
        isHidden = true;
    }
	
	// Update is called once per frame
	void Update () {

        

        if (Input.GetKeyDown("p") && isHidden)
        {
            this.transform.position = spellBookOriginalPosition.transform.position;
            isHidden = false;
        }
        else if (Input.GetKeyDown("p") && !isHidden)
        {
            this.transform.position = hiddenPosition;
            isHidden = true;
        }
		
	}
}
