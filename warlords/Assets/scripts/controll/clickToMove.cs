using UnityEngine;
using System.Collections;

public class clickToMove : MonoBehaviour {

    private float speed = 4;                   //base movementspeed
    private Vector3 targetPosition;             //where are we moving?
    bool isMoving;                              //are we moving?
    const int left_mouse_button = 0;            //move with left mouse button         

    // Use this for initialization
    void Start () {

        //start at our current position, standing still
        targetPosition = transform.position;
        isMoving       = false;


	}
	
	// Update is called once per frame
	void Update () {

      
        if (Input.GetMouseButton(left_mouse_button)) {                 //look to see if the player is clicking left mouse button
            SetTargetPosition();                                       //where did the player click?
        }
        if (isMoving) {
            MovePlayer();
        }

    }

    void SetTargetPosition()
    {
        
        Plane plane = new Plane(Vector3.up, transform.position);             //create a plane for the player to move on
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);         //cast a ray at where the player is clicking
        float point = 0f;

        if(plane.Raycast(ray, out point))
        {
            targetPosition = ray.GetPoint(point);
        }

        isMoving = true;
    }

    void MovePlayer()
    {
        //start moving the player towards the desired position
        transform.LookAt(targetPosition);                                                                                 
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        //if we are at the desired position we must stop moving
        if (transform.position == targetPosition)
        {
            isMoving = false;
        }

        Debug.DrawLine(transform.position, targetPosition, Color.red);                    
    }
}
