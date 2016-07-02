using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {
    int leftMouseButton = 0;
    private Vector3 desiredPosition;
    public float speed = 5;
    public CharacterController controller;
    private Vector3 lastSentPosition = new Vector3(10.81f, 0.39f, 14.25f);           // last sent move position to server (too keep track of not sending move request too often)    
                                  
                                                            
    // Use this for initialization
    void Start () {

        desiredPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButton(leftMouseButton))
        {
            locatePosition();
        }

       
        moveToPosition();
       
    }

    //here we try to locate the desired position for our hero to move
    void locatePosition()
    {
        RaycastHit hit;
        //cast a ray from our camera onto the ground to get our desired position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if we hit our ray, save the information to our "hit" variable
        if(Physics.Raycast(ray, out hit, 10000))
        {
            //update our desired position with the coordinates clicked
            desiredPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            Debug.Log(desiredPosition);

            // Code to see if we clicked on a minion (this should cancel walking if within attacking range)
            ((Target)GameObject.Find("GameLogicObject").GetComponent(typeof(Target))).minionWithinClickDistance(desiredPosition);
        }
    }

    //actually move our hero to the desired position
    void moveToPosition()
    {
        if(Vector3.Distance(transform.position, desiredPosition)>2){ 
        //rotate our hero towards the desired position so we allways run forward
        Quaternion newRotation = Quaternion.LookRotation(desiredPosition-transform.position);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10);

        //call our charactercontroller to move forward
        controller.SimpleMove(transform.forward);
        }

        // Check that we moved enough from last position to send update to server that we moved more
        float dist = Vector3.Distance(lastSentPosition, transform.position);
        if (dist > 4.0f)
        {
            print("Sending move request to server: " + dist);
            ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication))).sendMoveRequest(transform.position.x, transform.position.z, desiredPosition.x, desiredPosition.z);
        }
    }

  
}
