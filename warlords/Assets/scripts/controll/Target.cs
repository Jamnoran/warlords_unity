using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

    private Vector3 targetPosition;
    public float MinTargetDistance = 3.0f;
    const int left_mouse_button = 0;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(left_mouse_button))
        {
            getPosition();
            click();
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

    // We got a click on a point on map, here we need to handle if its friendly or enemy target or just ground
    // Return true if we found a target
    public bool click()
    {
        float closestDistanse = 300.0f;
        foreach (var minion in ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinions())
        {
            Vector3 minionPosition = new Vector3(minion.positionX,0.1f,minion.positionZ);
            float dist = Vector3.Distance(minionPosition, targetPosition);

            if ((dist < closestDistanse) && dist <= MinTargetDistance)
            {
                print("Minion is closes at a distance at: " + dist);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetEnemy(minion.id);
                closestDistanse = dist;
            }
        }
        foreach (var hero in ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getHeroes())
        {
            Vector3 heroPosition = new Vector3(hero.positionX, 0.1f, hero.positionZ);
            float dist = Vector3.Distance(heroPosition, targetPosition);
            Debug.Log("Class: " + hero.class_type + " Distance from click [" + targetPosition.x + "x"  + targetPosition.z + "] is: " + dist);
            if ((dist < closestDistanse) && dist <= MinTargetDistance)
            {
                print("Hero is closes at a distance at: " + dist);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetFriendly(hero.id);
                closestDistanse = dist;
            }
        }
        if(closestDistanse < 300.0f) {
            return true;
        }
        
        ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetEnemy(0);
        ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetFriendly(0);
        return false;
    }
}
