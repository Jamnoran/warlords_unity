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
    const int right_mouse_button = 1;
    private List<Hero> listOfHeroes = new List<Hero>();
    #endregion
    #region public variables
    public float MinTargetDistance = 3.0f;
    //Layers to skip targeting
    #endregion
    public int layer = 9;


    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public Texture2D defaultCursor;

    void Start() {
        listOfHeroes = getGameLogic().getHeroes();

        Cursor.SetCursor(defaultCursor, hotSpot, cursorMode);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(left_mouse_button)) {
            getPosition();
            click(true);
            
        }
        if (Input.GetMouseButtonUp(left_mouse_button)) {
            findStairs();
        }

        if (Input.GetMouseButton(right_mouse_button)) {
            getPosition();
			if (getGameLogic ().getMyHero () != null) {
				if (click (false)) {
					getGameLogic ().getMyHero ().setAutoAttacking (true);
				} else {
					getGameLogic ().getMyHero ().setAutoAttacking (false);
				}
			}
        }
    }

   

    /// <summary>
    /// Raycast and save the information hit in our private variables above
    /// </summary>
    void getPosition() {
        RaycastHit hit;
        //cast a ray from our camera onto the ground to get our desired position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Bit shift the index of the layer (10) to get a bit mask
        int layerMaskTemp = 1 << layer;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        LayerMask layerMaskToUse = ~layerMaskTemp;
        //if we hit our ray, save the information to our "hit" variable
        if (Physics.Raycast(ray, out hit, 10000, layerMaskToUse)) {
            //update our desired position with the coordinates clicked
            targetPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            //Debug.Log("Cliked on point : " + targetPosition);
            //save our type of target so we can check what we have clicked on.
            typeOftarget = hit.transform.gameObject;
            //Debug.Log("Clicked on type: " + hit.transform.gameObject.name);
        }
    }
    

    /// <summary>
    /// We got a click on a point on map, here we need to handle if its friendly or enemy target or just ground
    /// </summary>
    /// <returns> Bool (True if target found, False otherwise)</returns>
    public bool click(bool leftClick) {
        float closestDistanse = 300.0f;
        foreach (var minion in ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinions()) {
            if (minion.alive) {
                Vector3 minionPosition = new Vector3(minion.minionTransform.position.x, minion.minionTransform.position.y, minion.minionTransform.position.z);
                float dist = Vector3.Distance(minionPosition, targetPosition);

                if ((dist < closestDistanse) && dist <= MinTargetDistance) {
                    //print("Minion is closest at a distance at: " + dist);
                    getGameLogic().setHeroTargetEnemy(minion.id);
                    closestDistanse = dist;
                }
            }
        }
        foreach (var hero in (listOfHeroes)) {
            if (hero.alive) { 
                Vector3 heroPosition = new Vector3(hero.trans.position.x, hero.trans.position.y, hero.trans.position.z);
                float dist = Vector3.Distance(heroPosition, targetPosition);
                //Debug.Log("Class: " + hero.class_type + " Distance from click [" + targetPosition.x + "x"  + targetPosition.z + "] is: " + dist);
                if ((dist < closestDistanse) && dist <= MinTargetDistance) {
                    getGameLogic().setHeroTargetEnemy(0);
                    // print("Hero is closest at a distance at: " + dist);
                    getGameLogic().setHeroTargetFriendly(hero.id);
                    closestDistanse = dist;
                }
            }
        }
        if(closestDistanse < 300.0f) {
            return true;
        }

        if (leftClick) {
            getGameLogic().setHeroTargetFriendly(0);
            getGameLogic().setHeroTargetEnemy(0);
            
        }
        return false;
    }

    /// <summary>
    /// Check to see if we find stairs down, If we do we must counter numbers of players
    /// who have clicked it and handle logic accordingly
    /// </summary>
    private void findStairs() {
        //Debug.Log("Checking if stairs is focused : " + typeOftarget.transform.root.name);
        //if (typeOftarget.transform.root.name.Contains("EndPoint")) {
        //    Hero hero = getGameLogic().getMyHero();
        //    Debug.Log("Model name : " + hero.getModelName());
        //    FieldOfView field = ((FieldOfView) hero.trans.Find(hero.getModelName()).GetComponent(typeof(FieldOfView)));
        //    if (field.isPortalInRange()) {
        //        Debug.Log("Stair was in range");
        //    }
        //    getCommunication().heroHasClickedPortal(hero.id);
        //}
    }


    GameLogic getGameLogic() {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    /// <summary>
    /// Connect to our serverobject so we can communicate with it
    /// </summary>
    /// <returns>ServerCommunication object</returns>
    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }
}
