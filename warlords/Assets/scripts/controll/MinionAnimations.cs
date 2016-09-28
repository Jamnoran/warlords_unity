using UnityEngine;
using System.Collections;
using Assets.scripts.vo;

public class MinionAnimations : MonoBehaviour {
    #region public variables
    public GameObject character;
    public Vector3 targetPosition;
    public float moveSpeed = 5.0f;
    public bool idleAnimationRunning = true;
    public int heroTargetId = 0;
    public float attackRange = 1.9f;
    public bool sentInAttackRange = false;
    public bool sentClearAttackRange = false;
    #endregion
    #region private variables
    private Animator anim;
    bool isMoving;
    private Rigidbody rbody;
    #endregion



    // Use this for initialization
    void Start () {
        anim = character.GetComponent<Animator>();
        rbody = character.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        // First check if minion has a target to attack, in that case that takes priority over desiredlocation.
        if (heroTargetId > 0 ) {
            //Debug.Log("We got target to move to");
            Hero hero = getGameLogic().getHero(heroTargetId);
            targetPosition = hero.getTransformPosition();
        }



        //if we are at the desired position we must stop moving
        //if minion is not too close we can move
        if (Vector3.Distance(transform.position, targetPosition) < attackRange)
        {
            isMoving = false;
            if (heroTargetId > 0)
            {
                if (!sentInAttackRange)
                {
                    Debug.Log("Hero is now in range, we should stop moving and send attack command to server from minion");
                    sentInAttackRange = true;
                    sentClearAttackRange = false;
                    stopMovement();
                    sendAttackInRange(heroTargetId);
                }
                
            }
        }
        else {
            sentInAttackRange = false;
            if (!sentClearAttackRange)
            {
                sentClearAttackRange = true;
                sendAttackInRange(0);
            }
            lookAndMove();
        }

        if (!isMoving && !idleAnimationRunning)
        {
            idleAnimationRunning = true;
            //Debug.Log("Starting idle animation again");
            anim.Play("zombie_idle", -1, 0f);
        }

        if (Input.GetKeyDown("z"))
        {
            print("Minoin att!");
            attackAnimation();
        }

    }

    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    public void lookAndMove()
    {
        //start moving the player towards the desired position
        Vector3 targetPostition = new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z);

        character.transform.LookAt(targetPostition);

        // find the target position relative to the player:
        Vector3 dir = targetPosition - transform.position;
        // ignore any height difference:
        dir.y = 0;
        // calculate velocity limited to the desired speed:
        Vector3 velocity = Vector3.ClampMagnitude(dir * moveSpeed, moveSpeed);
        // move the character including gravity:
        CharacterController controller = (CharacterController)GetComponent(typeof(CharacterController));
        controller.SimpleMove(velocity);
    }

    public void stopMovement()
    {
        Vector3 targetPostition = new Vector3(character.transform.position.x, character.transform.position.y, character.transform.position.z);
    }


    public void sendAttackInRange(int heroId)
    {
        Debug.Log("Sending that minion is in attack range");
        Minion minion = getGameLogic().getClosestMinionByPosition(character.transform.position);
        getCommunication().sendMinionHasTargetInRange(minion.id, heroId);
    }



    public void attackAnimation()
    {
        Debug.Log("Showing attack animation");
        anim.Play("Attack", -1, 0f);
    }

    public void setDesiredLocation(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
        idleAnimationRunning = false;
        if (anim != null)
        {
            anim.Play("mutant_run_inPlace", -1, 0f);
        }
        else
        {
            Debug.Log("Could not find anim object on minion");
        }
        
    }

    void SetTargetPosition()
    {

        Plane plane = new Plane(Vector3.up, transform.position);             //create a plane for the player to move on
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);         //cast a ray at where the player is clicking
        float point = 0f;

        if (plane.Raycast(ray, out point))
        {
            targetPosition = ray.GetPoint(point);
        }
        //anim.SetBool("walking", true);
        isMoving = true;
        idleAnimationRunning = false;
        anim.Play("mutant_run_inPlace", -1, 0f);
    }

}
