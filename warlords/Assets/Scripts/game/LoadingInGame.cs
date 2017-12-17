using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingInGame : MonoBehaviour {

    public GameObject loadingOverlay;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showLoading()
    {
        loadingOverlay.SetActive(true);
    }

    public void hideLoading()
    {
        loadingOverlay.SetActive(false);
    }

    public void setLoadingText(string loadingText)
    {

    }
}
