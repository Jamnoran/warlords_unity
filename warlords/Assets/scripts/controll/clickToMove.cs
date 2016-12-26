using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System.Collections.Generic;

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
        Debug.Log("Setting target position to : " + character.position);
        getAnimation().setDesiredLocation(targetPosition);
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
            if (getGameLogic() != null)
            {
                Hero hero = getGameLogic().getMyHero();
                //Debug.Log("Hero.getAutoAttacking " + hero.getAutoAttacking() + " auto attack ready : " + getGameLogic().getAbility(0).isReady());
                if (hero.getAutoAttacking() && getGameLogic().getAbility(0).isReady() && hero.targetEnemy > 0)
                {
                    // Check if user is in range of auto attack otherwise set its location as targetPostion
                    bool minionInRange = false;
                    FieldOfViewAbility fieldOfViewAbility = hero.trans.GetComponent<FieldOfViewAbility>();
                    List<int> enemiesInRange = fieldOfViewAbility.FindVisibleTargets(360f, 3f, false);
                    if (enemiesInRange != null && enemiesInRange.Contains(hero.targetEnemy))
                    {
                        minionInRange = true;
                    }
                    if (minionInRange)
                    {
                        getGameLogic().getAbility(0).waitingForCdResponse = true;
                        getGameLogic().autoAttack();
                    }else
                    {
                        targetPosition = getGameLogic().getMinion(hero.targetEnemy).getTransformPosition();
                        MovePlayer();
                    }
                    
                }
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
        if (getCommunication() != null)
        {
            getCommunication().sendMoveRequest(transform.position.x, transform.position.z, targetPosition.x, targetPosition.z);
        }
    }


    ServerCommunication getCommunication()
    {
        if (GameObject.Find("Communication") != null)
        {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        }
        else
        {
            return null;
        }
    }

    GameLogic getGameLogic()
    {
        if (GameObject.Find("GameLogicObject") != null) { 
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        }else
        {
            return null;
        }
    }
    CharacterAnimations getAnimation()
    {
        return (CharacterAnimations)GetComponent(typeof(CharacterAnimations));
    }
}
