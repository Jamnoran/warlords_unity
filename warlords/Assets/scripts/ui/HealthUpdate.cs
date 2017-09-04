using UnityEngine;
using System.Collections;

public class HealthUpdate : MonoBehaviour {

    [SerializeField]
    private Stat health;
    public Canvas healthCanvas = null;
   
    private void Awake()
    {
        this.health.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void setCurrentVal(float newValue)
    {
        this.health.CurrentVal = newValue;
        if (newValue <= 0)
        {
            this.health.hideBar();
        }
    }
    public void setMaxValue(float newValue)
    {
        this.health.MaxVal = newValue;
        this.health.Initialize();
    }

    public void hideBar()
    {
        healthCanvas.enabled = false;
    }
    
}
