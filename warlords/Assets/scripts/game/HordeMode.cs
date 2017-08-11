using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordeMode : MonoBehaviour {

	public bool currentMode = false;
	private GameObject[] objectsToEnableWhenLevelIsComplete = null;
	public bool enabled = false;
	public int totalMinionsLeft = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(currentMode && !enabled){

			if(objectsToEnableWhenLevelIsComplete == null){
				objectsToEnableWhenLevelIsComplete = GameObject.FindGameObjectsWithTag("LevelCompleteObjects");;
				foreach (GameObject gObject in objectsToEnableWhenLevelIsComplete) {
					gObject.SetActive(false);	
				}
			}

			if(checkIfLevelIsComplete()){
				Debug.Log ("Level is done, open up next level! Activating this many objects : " + objectsToEnableWhenLevelIsComplete.Length);
				foreach(GameObject gObject in objectsToEnableWhenLevelIsComplete){
					Debug.Log ("Setting gameobject to active");
					gObject.SetActive(true);
				}
				enabled = true;
			}
		}
	}

	bool checkIfLevelIsComplete(){
		int minionsLeft = getGameLogic ().getMinions ().Count;
		if (minionsLeft == 0 && totalMinionsLeft == 0) {
			return true;
		}
		return false;
	}



	GameLogic getGameLogic() {
		return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
	}
}
