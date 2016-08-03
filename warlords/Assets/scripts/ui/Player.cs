using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField]
    private Stat health;

	private void Awake()
    {
        health.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void setCurrentVal(float newValue)
    {
        health.CurrentVal = newValue;
    }
    public void setMaxValue(float newValue)
    {
        health.MaxVal = newValue;
        health.Initialize();
    }
}
