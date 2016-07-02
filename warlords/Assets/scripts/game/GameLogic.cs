using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {
    //list of minions currently alive in our universe.
    private List<Minion> minions = new List<Minion>();
    //hold our prefab for the first mob
    public Transform mob1;
    int currentMinionInList;

	// Use this for initialization
	void Start () {
        Debug.Log("Game logic has started");
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void updateListOfMinions(List<Minion> newMinions) {
        foreach (var newMinion in newMinions) {
            bool found = false;
            foreach (var minion in minions)
            {
                if (newMinion.id == minion.id) {
                    found = true;
                    if (minion.hp != newMinion.hp) { 
                        minion.hp = newMinion.hp;
                        Debug.Log("Minions new hp = " + minion.hp);
                    }
                }
            }
            if (!found) {
                // Initiate minion here
                Debug.Log("Initiate minion");
                Instantiate(mob1, new Vector3(newMinion.desiredPositionX, 0, newMinion.desiredPositionZ), Quaternion.identity);
                minions.Add(newMinion);
            }
        }
    }
    

}
