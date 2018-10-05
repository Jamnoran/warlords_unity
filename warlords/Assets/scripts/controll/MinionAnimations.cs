 using UnityEngine;
using System.Collections;
using Assets.scripts.vo;

public class MinionAnimations : MonoBehaviour {
    #region public variables
    public GameObject character;
    public Vector3 targetPosition;
    public float moveSpeed = 5.0f;
    public int heroTargetId = 0;
    public float attackRange = 1.9f;
    public bool sentInAttackRange = false;
    public bool sentClearAttackRange = false;
	public float baseRotation = 3f;
	public float idleRotation = 1f;
	public float baseMovespeed = 5.0f;
	public float idleMoveSpeed = 2.0f;

    #endregion
    #region private variables
    private Animator anim;
    private bool isMoving = false;
    public bool inCombat = false;
    public bool isAttacking = false;
    public bool dead = false;
    #endregion



    // Use this for initialization
    void Start () {
        anim = character.GetComponent<Animator>();
        targetPosition = character.transform.position;
        dead = false;
    }
	
	// Update is called once per frame
	void Update () {
        // First check if minion has a target to attack, in that case that takes priority over desiredlocation.
        if (heroTargetId > 0 ) {
            Hero hero = getGameLogic().getHero(heroTargetId);
            if (hero != null) {
                targetPosition = hero.getTransformPosition();
				inCombat = true;
            } else {
				inCombat = false;
            }
        }

        if (anim != null && anim.GetBool("alive"))
        {
            //if we are at the desired position we must stop moving
            //if minion is not too close we can move
            if (Vector3.Distance(transform.position, targetPosition) < attackRange)
            {
                isMoving = false;
                if (heroTargetId > 0)
                {
                    if (!sentInAttackRange)
                    {
                        //Debug.Log("Hero is now in range, we should stop moving and send attack command to server from minion");
                        sentInAttackRange = true;
                        sentClearAttackRange = false;
                        sendAttackInRange(heroTargetId);
                    }
                }
            }
            else
            {
                if (anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName("auto"))
                {
                    isMoving = true;
                    lookAndMove();
                }
                sentInAttackRange = false;
                if (!sentClearAttackRange)
                {
                    sentClearAttackRange = true;
                    sendAttackInRange(0);
                }
            }
        }


		if(getGameLogic() != null && getGameLogic().isGameMode(World.HORDE_MODE)){
			inCombat = true;
		}

		calculateMoveSpeed ();

        if (dead)
        {
            deadAnimation();
        }

        runAnimation();

    }

	void calculateMoveSpeed(){
		if (inCombat) {
			moveSpeed = baseMovespeed;
		} else {
			moveSpeed = idleMoveSpeed;
		}
	}
		

    public void lookAndMove() {
        //start moving the player towards the desired position
        Vector3 targetPostition = new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z);


		if (inCombat) {
			character.transform.LookAt(targetPostition);	
		} else {
			//find the vector pointing from our position to the target
			Vector3 direction = (targetPostition - transform.position).normalized;

            //direction.y = character.transform.position.y;

            //create the rotation we need to be in to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            lookRotation.z = 0;
            lookRotation.x = 0;
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
        //Debug.Log("Sending that hero is in attack range");
        if (getGameLogic() != null)
        {
            Minion minion = getGameLogic().getMinionByTransform(gameObject.transform);
            getCommunication().sendMinionHasTargetInRange(minion.id, heroId);
        }
    }



    public void attackAnimation()  {
        //Debug.Log("Showing attack animation");
        //anim.Play("Attack", -1, 0f);
        if (anim != null && anim.GetBool("alive"))
        {
            anim.SetBool("attacking", true);
            isAttacking = true;
            StartCoroutine("clearAttackBool");
        }
    }

    IEnumerator clearAttackBool()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("attacking", false);
    }


    public void setDesiredLocation(Vector3 position) {
        if (anim != null && (anim.GetCurrentAnimatorStateInfo(0).IsName("auto"))) {
           //Debug.Log("Dont move to another position we are attacking or we are already running");
        } else {
            targetPosition = position;
            isMoving = true;
            runAnimation();
            //if (anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName("mutant_run_inPlace")) {
                //anim.Play("mutant_run_inPlace", -1, 0f);
            //}
        }
    }

    public void runAnimation() {
        // Debug.Log("Showing run animation");
        if (anim != null && anim.GetBool("alive")) {
            //anim.Play("mutant_run_inPlace", -1, 0f);
            anim.SetBool("walking", isMoving);
        }
    }

    public void deadAnimation()
    {
        if(anim != null)
        {
            anim.SetBool("alive", false);
        }
    }


    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic()
    {
        if (GameObject.Find("GameLogicObject") != null)
        {
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        }
        else
        {
            return null;
        }
    }


}
