using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectsOnRotation : MonoBehaviour {

    public GameObject parentToCheckObjective;
    public List<int> rotationsToHide = new List<int>();
    public List<GameObject> objectsToHide = new List<GameObject>();

    // Use this for initialization
    void Start () {
        if (parentToCheckObjective != null)
        {
            foreach (GameObject objectToHide in objectsToHide)
            {
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
