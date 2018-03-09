using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideAndShowSpellBook : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("p"))
        {
            //Debug.Log("Showing character");
            if (getUIWindow().IsVisible)
            {
                getUIWindow().Hide();
            }
            else
            {
                getUIWindow().Show();
            }
        }
    }

    UIWindow getUIWindow()
    {
        return ((UIWindow)transform.GetComponent(typeof(UIWindow)));
    }
}
