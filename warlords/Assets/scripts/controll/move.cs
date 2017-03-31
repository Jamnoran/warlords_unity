using UnityEngine;
using System.Collections;
using Assets.scripts.vo;

public class move : MonoBehaviour {
    int leftMouseButton = 0;
    int rightMouseButton = 1;
    private Vector3 targetPosition;
    //private Vector3 desiredPosition;
    public float speed = 5;
    //public CharacterController controller;
    private Vector3 lastSentPosition = new Vector3(10.81f, 0.39f, 14.25f);           // last sent move position to server (too keep track of not sending move request too often)    
    public bool isMyHero = false;
    public int heroId = 0;

    public Transform greenPointer;
    public Transform bluePointer;
    public Transform redPointer;


    // Use this for initialization
    void Start () {
        //desiredPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        // Constantly check the heroes desired location and update it
        if(heroId > 0 && ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getHero(heroId) != null)
        {
            //desiredPosition = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getHero(heroId).getDesiredPosition();
        }


        //if(transform != null)
        //{
            // Handle mouse input
            if (isMyHero && Input.GetMouseButtonUp(leftMouseButton))
            {
                getTargetPosition();
                leftClick();
            }
            if (isMyHero && Input.GetMouseButtonUp(rightMouseButton))
            {
                getTargetPosition();
                rightClick();
            }

            // This should happen even if no mouse button is clicked
            moveToPosition();
        //}
    }


    void getTargetPosition()
    {
        RaycastHit hit;
        //cast a ray from our camera onto the ground to get our desired position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if we hit our ray, save the information to our "hit" variable
        if (Physics.Raycast(ray, out hit, 10000))
        {
            //update our desired position with the coordinates clicked
            targetPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
    }

    void leftClick()
    {
        // Code to see if we clicked on a minion (this should cancel walking if within attacking range)
        //bool foundTarget = ((Target)GameObject.Find("GameLogicObject").GetComponent(typeof(Target))).click(targetPosition, true);
        //Debug.Log("Left mouse clicked and found target : " + foundTarget);
        //if (foundTarget)
        //{
        //    placeTracker(2);
        //}
        //else
        //{
        //    placeTracker(3);
        //}
    }

    void rightClick()
    {
        Debug.Log("Right mouse clicked");
        placeTracker(1);

        //bool foundMinion = ((Target)GameObject.Find("GameLogicObject").GetComponent(typeof(Target))).click(targetPosition, true);

        if (((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero() != null)
        {
            // If boolean, then attack instead of walk
            //if (foundMinion)
            //{
                // Here we need to check the attack range of hero and move to that point if we are not within yet. Also set Attacking = true on hero
            //}else
            //{
                // Update desired position of own hero to send to sever
             //   ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero().desiredPositionX = targetPosition.x;
             //   ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero().desiredPositionZ = targetPosition.z;
             //   sendMove();
             //   getAnimation().setDesiredLocation(targetPosition);
            //}
        }
    }
    

    //actually move our hero to the desired position
    void moveToPosition()
    {
        if (isMyHero)
        {
            // Check that we moved enough from last position to send update to server that we moved more
            float dist = Vector3.Distance(lastSentPosition, transform.position);
            if (dist > 1.0f)
            {
                //print("Sending move request to server: " + dist);
                sendMove();
            }
        }
    }

    void sendMove()
    {
        lastSentPosition = transform.position;
        getCommunication().sendMoveRequest(transform.position.x, transform.position.z, targetPosition.x, targetPosition.z);
    }

    void placeTracker(int color)
    {
        if (color == 1) {
            Instantiate(greenPointer, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z), Quaternion.identity);
        } else if (color == 2)
        {
            Instantiate(bluePointer, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z), Quaternion.identity);
        }
        else if (color == 3)
        {
            Instantiate(redPointer, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z), Quaternion.identity);
        }
    }


    ServerCommunication getCommunication()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Communication");
        //Debug.Log("Found this many gameobjects with communication as tag : " + gos.Length);
        foreach (GameObject go in gos)
        {
            return (ServerCommunication)go.GetComponent(typeof(ServerCommunication));
        }
        return null;
    }

    CharacterAnimations getAnimation()
    {
         return (CharacterAnimations)transform.GetComponent(typeof(CharacterAnimations));
    }
}
