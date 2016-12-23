using UnityEngine;
using System.Collections;
using Assets.scripts.vo;

public class clickToMove : MonoBehaviour {
    
    private Vector3 targetPosition;             //where are we moving?
    const int left_mouse_button = 0;            //move with left mouse button        
    const int right_mouse_button = 1;            //move with mouse mouse button        
    public Transform character;
    public bool isMyHero = false;
    public int heroId = 0;
    private Vector3 lastSentPosition = new Vector3(10.81f, 0.39f, 14.25f);           // last sent move position to server (too keep track of not sending move request too often)    


    // Use this for initialization
    void Start () {
        //start at our current position, standing still
        targetPosition = character.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (isMyHero && Input.GetMouseButton(right_mouse_button)) {                 //look to see if the player is clicking right mouse button
            getPosition();                                       //where did the player click?
            MovePlayer();
        }

        // Check if hero wants to auto attack
        if (isMyHero)
        {
            Hero hero = getGameLogic().getMyHero();
            //Debug.Log("Hero.getAutoAttacking " + hero.getAutoAttacking() + " auto attack ready : " + getGameLogic().getAbility(0).isReady());
            if (hero.getAutoAttacking() && getGameLogic().getAbility(0).isReady() && hero.targetEnemy > 0)
            {
                getGameLogic().getAbility(0).waitingForCdResponse = true;
                getGameLogic().autoAttack();
            }
        }
    }

    void getPosition()
    {
        RaycastHit hit;
        //cast a ray from our camera onto the ground to get our desired position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if we hit our ray, save the information to our "hit" variable
        if (Physics.Raycast(ray, out hit, 10000))
        {
            //update our desired position with the coordinates clicked
            targetPosition = new Vector3(hit.point.x, 0, hit.point.z);
        }
    }
    
    void MovePlayer()
    {
        //Debug.Log("Sending moveplayer to : " + targetPosition);
        getAnimation().setDesiredLocation(targetPosition);

        // Check that we moved enough from last position to send update to server that we moved more
        float dist = Vector3.Distance(lastSentPosition, transform.position);
        if (dist > 0.5f)
        {
            //print("Sending move request to server: " + dist);
            sendMove();
        }
    }
    
    void sendMove()
    {
        lastSentPosition = transform.position;
        getCommunication().sendMoveRequest(transform.position.x, transform.position.z, targetPosition.x, targetPosition.z);
    }


    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
    CharacterAnimations getAnimation()
    {
        return (CharacterAnimations)GetComponent(typeof(CharacterAnimations));
    }
}
