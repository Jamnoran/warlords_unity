using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCombatText : MonoBehaviour {

    private GameObject mySct;
    private GameObject _canvas;

	// Use this for initialization
	void Start () {
        mySct = Resources.Load<GameObject>("PopupTextParent");
        _canvas = GameObject.Find("SctCanvas");
	}
	
    public void SpawnCombatText()
    {
        Debug.Log("Attempting to spawn combat text");
        var sctInstance = Instantiate(mySct);
        sctInstance.transform.SetParent(_canvas.transform);
    }
}
