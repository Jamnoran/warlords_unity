using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System;

public class CharacterAnimations : MonoBehaviour {
    public GameObject character;
    private Animator anim;
    public Vector3 targetPosition;
    bool isMoving;
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 3.0F;
    public bool sentStopAnimation = false;

    int grounded = 0;
    float slopefix = 8.0f;
    public bool isAttacking = false;
    public bool isGrounded = false;
    public float distanceToTarget = 10.0f;
    public float distanceBeforeSnappingHeroes = 3.0f;

    // Use this for initialization
    void Start () {
        anim = character.GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPostition = new Vector3(targetPosition.x, character.transform.position.y, targetPosition.z);
        // Update target position to minion position if we have a target and is auto attacking
        if (getGameLogic() != null && getGameLogic().getMyHero() != null)
        {
            Hero thisHero = getGameLogic().getClosestHeroByPosition(character.transform.position);
            if (thisHero.targetEnemy > 0 && isAttacking)
            {
                Vector3 pos = getGameLogic().getMinion(thisHero.targetEnemy).getTransformPosition();
                targetPostition = new Vector3(pos.x, character.transform.position.y, pos.z);
            }
        }

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

        //if we are at the desired position we must stop moving
        distanceToTarget = Vector3.Distance(character.transform.position, targetPosition);
        if (distanceToTarget < 0.50f)
        {
            isMoving = false;
        }

        if (getGameLogic() != null && getGameLogic().getMyHero() != null) {
            Hero thisHero = getGameLogic().getClosestHeroByPosition(character.transform.position);
            if (!isMoving && !sentStopAnimation && thisHero.id == getGameLogic().getMyHero().id && !thisHero.getAutoAttacking()) {
                sentStopAnimation = true;
                isMoving = false;

                getCommunication().sendMoveRequest(transform.position.x, transform.position.z, targetPosition.x, targetPosition.z);
                getCommunication().sendStopHero(getGameLogic().getMyHero().id);
            }
        }
        runAnimation();
    }

    public void rotateToTarget(Vector3 postition) {
        Debug.Log("Rotating towards target");
        Vector3 rotatingPostition = new Vector3(postition.x, character.transform.position.y, postition.z);
        character.transform.LookAt(rotatingPostition);

    }

    public void attackAnimation() {
        Debug.Log("Got auto attack animation");
        anim.SetBool("attacking", true);
        isAttacking = true;
        StartCoroutine("clearAttackBool");
    }

    IEnumerator clearAttackBool()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("attacking", false);
    }

    IEnumerator clearSpell1()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("spell1", false);
    }

    IEnumerator clearSpell2()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("spell2", false);
    }

    IEnumerator clearSpell3()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("spell3", false);
    }

    internal void setAlive(bool status)
    {
        anim.SetBool("alive", false);
    }

    public void runAnimation() {
        anim.SetBool("walking", isMoving);
    }

    public void idleAnimation() {
        //anim.SetBool("walking", false);
    }


    public void spellAnimation(int spellAnimationId)
    {
        if (spellAnimationId == 1)
        {
            anim.SetBool("spell1", true);
            StartCoroutine("clearSpell1");
        }
        else if (spellAnimationId == 2)
        {
            anim.SetBool("spell2", true);
            StartCoroutine("clearSpell2");
        }
        else if (spellAnimationId == 3)
        {
            anim.SetBool("spell3", true);
            StartCoroutine("clearSpell3");
        }
    }


    public void setDesiredLocation(Vector3 position) {
        targetPosition = position;
        isMoving = true;
        sentStopAnimation = false;
        isAttacking = false;
    }

    public void setPositionFromServer(Vector3 currentPositionFromServer)
    {
        distanceToTarget = Vector3.Distance(currentPositionFromServer, character.transform.position);
        if (distanceToTarget >= distanceBeforeSnappingHeroes)
        {
            //Debug.Log("Character is too far away from where it should be, teleport it!");
            //character.transform.position = currentPositionFromServer;
        }
    }

    public void stopMove() {
        //Debug.Log("Setting target position to current position : " + character.transform.position.x + " x " + character.transform.position.z);
        targetPosition = character.transform.position;
        isAttacking = false;
        isMoving = false;
    }





    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    LobbyCommunication getLobbyCommunication() {
        return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
    }


    GameLogic getGameLogic() {
        if (GameObject.Find("GameLogicObject") != null) {
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        } else {
            return null;
        }
    }

}
