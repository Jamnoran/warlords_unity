using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField]
    private Stat health;

	private void Awake()
    {
        health.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
	
        if (Input.GetKeyDown(KeyCode.I))
        {
            health.CurrentVal -= 10;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            health.CurrentVal += 10;
        }
    }
}
