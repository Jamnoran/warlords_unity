using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour {


    private float radius = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            

            foreach (var collider in hitColliders)
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 toOther = collider.transform.position - transform.position;
                if (Vector3.Dot(forward, toOther) > 0 && collider.tag == "Enemy")
                {
                    Debug.Log(collider.name + "is infront of me");
                }

            }
        }
		
	}
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
