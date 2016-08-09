using UnityEngine;
using System.Collections;

public class WarriorAnimations : MonoBehaviour {
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
        if (Input.GetKeyDown("1"))
        {
            print("Slash!");
            anim.Play("sword_and_shield_slash", -1, 0f);
        }
        if (Input.GetMouseButton(0))
        {                 //look to see if the player is clicking left mouse button
            SetTargetPosition();                                       //where did the player click?
        }

        if (Input.GetKeyDown("2"))
        {
            anim.Play("sword_and_shield_run_inPlace", -1, 0f);
            //rbody.velocity += transform.forward * moveSpeed;
        }

        float moveX = targetPosition.x * 20f * Time.deltaTime;
        float moveZ = targetPosition.z * 20f * Time.deltaTime;

        //rbody.velocity = new Vector3(moveX, 0f, moveZ);


        //start moving the player towards the desired position
        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        //if we are at the desired position we must stop moving
        if (transform.position == targetPosition)
        {
            isMoving = false;
        }

        if (!isMoving && !idleAnimationRunning)
        {
            idleAnimationRunning = true;
            Debug.Log("Starting idle animation again");
            anim.Play("sword_and_shield_idle", -1, 0f);
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
        anim.Play("sword_and_shield_walk_inPlace", -1, 0f);
    }

}
