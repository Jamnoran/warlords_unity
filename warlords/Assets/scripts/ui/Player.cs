using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField]
    private Stat health;
   
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
    }
    public void setMaxValue(float newValue)
    {
        this.health.MaxVal = newValue;
        this.health.Initialize();
    }
}
