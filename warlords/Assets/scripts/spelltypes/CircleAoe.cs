using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAoe : MonoBehaviour
{

    private float _offset;
    public float _radius;
    public LayerMask _lookForThis;

    public CircleAoe(float radius, int lookForThis)
    {
        _radius = radius;
        _lookForThis = lookForThis;
    }

    void Start()
    {
        _offset = 5;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        RaycastHit hit;
        
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
                this.transform.position = new Vector3(hit.point.x, hit.point.y + _offset, hit.point.z);         
        }

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit[] sphere = Physics.SphereCastAll(transform.position, _radius, transform.forward, Mathf.Infinity, _lookForThis);
        }
    }

    public RaycastHit[] GetAoeTargets(RaycastHit[] targets)
    {
     
        return targets;
    }

}
