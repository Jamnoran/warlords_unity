using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.scripts.vo;

public class Target : MonoBehaviour {
    #region private variables
    private Vector3 targetPosition;
    private GameObject typeOftarget;
    const int left_mouse_button = 0;
    private List<Hero> listOfHeroes = new List<Hero>();
    #endregion
    #region public variables
    public float MinTargetDistance = 3.0f;
    #endregion

    void Start()
    {
        listOfHeroes = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getHeroes();
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(left_mouse_button))
        {
            getPosition();
            click();
            
        }
        if (Input.GetMouseButtonUp(left_mouse_button))
        {
            findStairs();
        }
    }

   

    /// <summary>
    /// Raycast and save the information hit in our private variables above
    /// </summary>
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
            //save our type of target so we can check what we have clicked on.
            typeOftarget = hit.transform.gameObject;
        }
    }
    
    /// <summary>
    /// Connect to our serverobject so we can communicate with it
    /// </summary>
    /// <returns>ServerCommunication object</returns>
    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    /// <summary>
    /// We got a click on a point on map, here we need to handle if its friendly or enemy target or just ground
    /// </summary>
    /// <returns> Bool (True if target found, False otherwise)</returns>
    public bool click()
    {
        float closestDistanse = 300.0f;
        foreach (var minion in ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinions())
        {
            Vector3 minionPosition = new Vector3(minion.minionTransform.position.x, 0.1f, minion.minionTransform.position.z);
            float dist = Vector3.Distance(minionPosition, targetPosition);

            if ((dist < closestDistanse) && dist <= MinTargetDistance)
            {
                print("Minion is closes at a distance at: " + dist);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setHeroTargetEnemy(minion.id);
                closestDistanse = dist;
            }
        }
        foreach (var hero in (listOfHeroes))
        {
            Vector3 heroPosition = new Vector3(hero.trans.position.x, 0.1f, hero.trans.position.z);
            float dist = Vector3.Distance(heroPosition, targetPosition);
            //Debug.Log("Class: " + hero.class_type + " Distance from click [" + targetPosition.x + "x"  + targetPosition.z + "] is: " + dist);
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

    /// <summary>
    /// Check to see if we find stairs down, If we do we must counter numbers of players
    /// who have clicked it and handle logic accordingly
    /// </summary>
    private void findStairs()
    {
        int numberOfheroes = listOfHeroes.Count;
        if (typeOftarget.transform.root.name == "StairsDown(Clone)")
        {
            
            foreach (var hero in ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getHeroes())
            {
                 FieldOfView field = ((FieldOfView)hero.trans.Find("Warrior").GetComponent(typeof(FieldOfView)));
                field.isPortalInRange();
                getCommunication().heroHasClickedPortal(hero.id);
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Hello World!");
    }
}
