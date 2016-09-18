using UnityEngine;
using System.Collections;

public class MinionAnimations : MonoBehaviour {
    public GameObject character;
    private Animator anim;
    public Vector3 targetPosition;
    bool isMoving;
    private Rigidbody rbody;
    public float moveSpeed = 5.0f;
    public bool idleAnimationRunning = true;

	// Use this for initialization
	void Start () {
        anim = character.GetComponent<Animator>();
        rbody = character.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {


        //start moving the player towards the desired position
        //transform.LookAt(targetPosition);
        Vector3 targetPostition = new Vector3(targetPosition.x,
                                               character.transform.position.y,
                                               targetPosition.z);
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
        if (transform.position == targetPosition)
        {
            isMoving = false;
        }

        if (!isMoving && !idleAnimationRunning)
        {
            idleAnimationRunning = true;
            Debug.Log("Starting idle animation again");
            anim.Play("zombie_idle", -1, 0f);
        }

    }


    public void attackAnimation()
    {
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
