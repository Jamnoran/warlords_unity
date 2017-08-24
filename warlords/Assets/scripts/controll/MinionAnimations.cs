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
	public bool inCombat = false;
	public float baseRotation = 3f;
	public float idleRotation = 1f;
	public float baseMovespeed = 5.0f;
	public float idleMoveSpeed = 2.0f;

    #endregion
    #region private variables
    private Animator anim;
    bool isMoving;
    #endregion



    // Use this for initialization
    void Start () {
        anim = character.GetComponent<Animator>();
        targetPosition = character.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        // First check if minion has a target to attack, in that case that takes priority over desiredlocation.
        if (heroTargetId > 0 ) {
            //Debug.Log("We got target to move to");
            Hero hero = getGameLogic().getHero(heroTargetId);
            if (hero != null) {
                targetPosition = hero.getTransformPosition();
				inCombat = true;
            } else {
                Debug.Log("Could not find a hero with this id: " + heroTargetId);
				inCombat = false;
            }
        }

        //if we are at the desired position we must stop moving
        //if minion is not too close we can move
        if (Vector3.Distance(transform.position, targetPosition) < attackRange) {
            isMoving = false;
            if (heroTargetId > 0) {
                if (!sentInAttackRange) {
                    //Debug.Log("Hero is now in range, we should stop moving and send attack command to server from minion");
                    sentInAttackRange = true;
                    sentClearAttackRange = false;
                    sendAttackInRange(heroTargetId);
                }
            }
        } else {
            if (anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
                isMoving = true;
                runAnimation();
                lookAndMove();
            }
            sentInAttackRange = false;
            if (!sentClearAttackRange) {
                sentClearAttackRange = true;
                sendAttackInRange(0);
            }
        }

        if (!isMoving) {
            idleAnimationRunning = true;
            //Debug.Log("Starting idle animation again");
            if (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("mutant_run_inPlace")) {
                anim.Play("zombie_idle", -1, 0f);
            }
        }


		if(getGameLogic().isGameMode(World.HORDE_MODE)){
			inCombat = true;
		}

		calculateMoveSpeed ();

    }

	void calculateMoveSpeed(){
		if (inCombat) {
			moveSpeed = baseMovespeed;
		} else {
			moveSpeed = idleMoveSpeed;
		}
	}
		

    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    public void lookAndMove() {
        //start moving the player towards the desired position
        Vector3 targetPostition = new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z);


		if (inCombat) {
			character.transform.LookAt(targetPostition);	
		} else {
			//find the vector pointing from our position to the target
			Vector3 direction = (targetPostition - transform.position).normalized;

			//create the rotation we need to be in to look at the target
			Quaternion lookRotation = Quaternion.LookRotation(direction);

			//rotate us over time according to speed until we are in the required rotation
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * idleRotation);
		}

			
        

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
    

    public void sendAttackInRange(int heroId) {
        //if (heroTargetId != 0) {
            //Debug.Log("Sending that minion is in attack range");
            Minion minion = getGameLogic().getClosestMinionByPosition(character.transform.position);
            getCommunication().sendMinionHasTargetInRange(minion.id, heroId);
        //}
    }



    public void attackAnimation()  {
        //Debug.Log("Showing attack animation");
        anim.Play("Attack", -1, 0f);
    }

    public void setDesiredLocation(Vector3 position) {
        if (anim != null && (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))) {
           // Debug.Log("Dont move to another position we are attacking or we are already running");
        } else {
           // Debug.Log("Setting new targetPostion and mutantRunInPlace");
            targetPosition = position;
            isMoving = true;
            idleAnimationRunning = false;
            runAnimation();
            //if (anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName("mutant_run_inPlace")) {
                //anim.Play("mutant_run_inPlace", -1, 0f);
            //}
        }
    }

    public void runAnimation() {
        // Debug.Log("Showing run animation");
        if (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("zombie_idle")) {
            anim.Play("mutant_run_inPlace", -1, 0f);
        }
    }

}
