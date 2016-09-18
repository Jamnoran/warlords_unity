using UnityEngine;
using System.Collections;

public class clickToMove : MonoBehaviour {
    
    private Vector3 targetPosition;             //where are we moving?
    const int left_mouse_button = 0;            //move with left mouse button        
    public Transform character;
    public bool isMyHero = false;
    public int heroId = 0;

    // Use this for initialization
    void Start () {
        //start at our current position, standing still
        targetPosition = character.position;
        
	}
	
	// Update is called once per frame
	void Update () {
        if (isMyHero && Input.GetMouseButton(left_mouse_button)) {                 //look to see if the player is clicking left mouse button
            getPosition();                                       //where did the player click?
            MovePlayer();
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
        Debug.Log("Sending moveplayer to : " + targetPosition);
        getAnimation().setDesiredLocation(targetPosition);
    }


    WarriorAnimations getAnimation()
    {
        return (WarriorAnimations)GetComponent(typeof(WarriorAnimations));
    }
}
