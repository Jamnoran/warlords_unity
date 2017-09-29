using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthUpdate : MonoBehaviour {

    [SerializeField]
    private Stat health;
    public Canvas healthCanvas = null;
    [SerializeField]
    private Stat resource;
    public Canvas resourceCanvas = null;
    public GameObject resourceContent = null;
    public Sprite mana;
    public Sprite rage;
   
    private void Awake()
    {
        this.health.Initialize();
        if (this.resource != null)
        {
            this.resource.Initialize();
        }
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void setCurrentVal(float newValue)
    {
        this.health.CurrentVal = newValue;
        if (newValue <= 0)
        {
            //this.health.hideBar();
        }
    }
    public void setCurrentResourceVal(float newValue)
    {
        this.resource.CurrentVal = newValue;
        if (newValue <= 0)
        {
            //this.resource.hideBar();
        }
    }

    public void setResourceType(int type)
    {
        if (type == 1)
        {
            resourceContent.GetComponent<Image>().sprite = mana;
        }
        else if (type == 2)
        {
            resourceContent.GetComponent<Image>().sprite = rage;
        }
    }

    public void setMaxValue(float newValue)
    {
        this.health.MaxVal = newValue;
        this.health.Initialize();
    }

    public void setMaxResourceValue(float newValue)
    {
        this.resource.MaxVal = newValue;
        this.resource.Initialize();
    }

    public void hideBar()
    {
        healthCanvas.enabled = false;
    }

    public void hideResource()
    {
        resourceCanvas.enabled = false;
    }
    
}
