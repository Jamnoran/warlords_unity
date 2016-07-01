using UnityEngine;
using System.Collections;

public class cameraFollow : MonoBehaviour {
    public Transform Player;
    public Vector3 Offset;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void LateUpdate()
    {
        if (Player != null)
            transform.position = Player.position + Offset;
    }

}
