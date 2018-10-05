using Assets.scripts.util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsLogic : MonoBehaviour {

    public GameObject buttons;
    public GameObject options;
    public float volume = 0.5f;
    public Slider slider;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void closeOptions()
    {
        options.SetActive(false);
        buttons.SetActive(true);
    }

    public void openOptions()
    {
        options.SetActive(true);

        GameSettings gameSettings = new GameSettings();
        slider.value = gameSettings.getVolume();

        buttons.SetActive(false);
    }

    public void saveSettings()
    {
        GameSettings gameSettings = new GameSettings();
        gameSettings.setVolume(volume);
        closeOptions();
    }
    
    public void setVolume(Slider slider)
    {
        volume = slider.value;
    }
}
