using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {

    public GameObject cameraOrbit;
    private GameObject target;
    public Transform pref;
    public float rotateSpeed = 8f;
    public bool acceptsRotationOfCamera = true;
    public bool acceptsZoomOfCamera = true;

    private void Start()
    {
    }

    public void setParent(GameObject newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target != null)
        {
            cameraOrbit.transform.position = target.transform.position;
        }
        if (Input.GetMouseButton(0) && acceptsRotationOfCamera)
        {
            float h = rotateSpeed * Input.GetAxis("Mouse X");
            float v = rotateSpeed * Input.GetAxis("Mouse Y");

            if (cameraOrbit.transform.eulerAngles.z + v <= 0.1f || cameraOrbit.transform.eulerAngles.z + v >= 179.9f)
                v = 0;

            cameraOrbit.transform.eulerAngles = new Vector3(cameraOrbit.transform.eulerAngles.x, cameraOrbit.transform.eulerAngles.y + h, cameraOrbit.transform.eulerAngles.z + v);
        }

        float scrollFactor = Input.GetAxis("Mouse ScrollWheel");

        if (scrollFactor != 0 && acceptsZoomOfCamera)
        {
            cameraOrbit.transform.localScale = cameraOrbit.transform.localScale * (1f - scrollFactor);
        }
    }
}
