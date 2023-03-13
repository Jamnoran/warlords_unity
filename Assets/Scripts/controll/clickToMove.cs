using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System.Collections.Generic;
using Assets.scripts.util;

public class clickToMove : MonoBehaviour {
    
    private Vector3 targetPosition;             //where are we moving?
    const int left_mouse_button = 0;            //move with left mouse button        
    const int right_mouse_button = 1;            //move with mouse mouse button        
    public Transform character;
    public int heroId = 0;
    private Vector3 lastSentPosition = new Vector3(0f, 0f, 0f);           // last sent move position to server (too keep track of not sending move request too often)    

    // Use this for initialization
    void Start () {
        //start at our current position, standing still
        if (character != null)
        {
            targetPosition = character.position;
            Debug.Log("Setting target position to : " + character.position);
            getAnimation().setDesiredLocation(targetPosition);
        }
    }
	
	// Update is called once per frame
	void Update () {
        HeroInfo heroInfo = getHeroInfo();
        if (heroInfo  != null && heroInfo.isAlive())
        {
            Hero hero = getGameLogic().getHero(heroInfo.getHeroId());

            // Check if hero wants to auto attack
            if (heroInfo.isMyHero) {
                if(Input.GetMouseButton(right_mouse_button))
                {
                    //look to see if the player is clicking right mouse button
                    getPosition();
                    //where did the player click?
                    movePlayer();
                }
                //Debug.Log("Hero.getAutoAttacking " + hero.getAutoAttacking() + " auto attack ready : " + getGameLogic().getAbility(0).isReady());
                if (hero.getAutoAttacking() && getGameLogic().getAbilityByAbilityName("Auto Attack").isReady() && hero.targetEnemy > 0) {
                    // Check if user is in range of auto attack otherwise set its location as targetPostion
                    FieldOfViewAbility fieldOfViewAbility = hero.trans.GetComponent<FieldOfViewAbility>();
                    List<int> enemiesInRange = fieldOfViewAbility.FindVisibleTargets(360f, hero.attackRange, false);
                    if (enemiesInRange != null && enemiesInRange.Contains(hero.targetEnemy)) {
                        Debug.Log("Hero is in range of autoattack");
                        // Is in range
                        getAnimation().stopMove();
                        getAnimation().rotateToTarget(getGameLogic().getMinion(hero.targetEnemy).getTransformPosition());
                        getGameLogic().getAbility(0).waitingForCdResponse = true;
                        getGameLogic().autoAttack();
                    } else {
                        if (hero.targetEnemy > 0 && getGameLogic() != null && getGameLogic().getMinion(hero.targetEnemy) != null && getGameLogic().getMinion(hero.targetEnemy).getTransformPosition() != null)
                        {
                            targetPosition = getGameLogic().getMinion(hero.targetEnemy).getTransformPosition();
                        }
                        movePlayer();
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
            targetPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
    }
    
    void movePlayer() {
        //Debug.Log("Sending moveplayer to : " + targetPosition);
        getAnimation().setDesiredLocation(targetPosition);

        // Check that we moved enough from last position to send update to server that we moved more
        float dist = Vector3.Distance(lastSentPosition, transform.position);
        if (dist > 0.5f) {
            //print("Sending move request to server: " + dist);
            sendMove();
        }
    }
    
    void sendMove() {
        lastSentPosition = transform.position;
        if (getCommunication() != null) {
            getCommunication().sendMoveRequest(transform.position.x, transform.position.y, transform.position.z, targetPosition.x, targetPosition.y, targetPosition.z);
        }
    }


    ServerCommunication getCommunication() {
        if (GameObject.Find("Communication") != null) {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        } else {
            return null;
        }
    }

    GameLogic getGameLogic() {
        if (GameObject.Find("GameLogicObject") != null) { 
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        } else {
            return null;
        }
    }

    CharacterAnimations getAnimation() {
        return (CharacterAnimations)GetComponent(typeof(CharacterAnimations));
    }

    HeroInfo getHeroInfo()
    {
        return (HeroInfo)GetComponent(typeof(HeroInfo));
    }
}
