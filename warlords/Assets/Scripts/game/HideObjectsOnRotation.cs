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
            foreach (int rotation in rotationsToHide) {
                if(parentToCheckObjective.transform.rotation.eulerAngles.y == rotation)
                {
                    foreach (GameObject objectToHide in objectsToHide)
                    {
                        Renderer rend = objectToHide.GetComponent<Renderer>();
                        rend.enabled = false;
                    }
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
