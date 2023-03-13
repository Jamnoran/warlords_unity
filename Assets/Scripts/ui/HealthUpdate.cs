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
    }
    public void setCurrentResourceVal(float newValue)
    {
        this.resource.CurrentVal = newValue;
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
        GameObject background = healthCanvas.transform.Find("frame/background").gameObject;
        background.SetActive(false);
        GameObject mask = healthCanvas.transform.Find("frame/mask").gameObject;
        mask.SetActive(false);
    }

    public void hideResource()
    {
        resourceCanvas.enabled = false;
    }
    
}
