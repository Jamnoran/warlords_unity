using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingHandler : MonoBehaviour {

    public GameObject loadingOverlay;

	// Use this for initialization
	void Start () {
        if (loadingOverlay != null)
        {
            loadingOverlay.SetActive(true);
        }
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
