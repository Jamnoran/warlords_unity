using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class BarScript : MonoBehaviour {

    [SerializeField] //make visible in editor
    private float fillAmmount; //handle our fillammount to increase/decrease the bar

    [SerializeField]
    private float lerpSpeed = 2;

    [SerializeField]
    private Image content;
    // Use this for initialization

    public float MaxValue { get; set; } //property so we can update maximum health

    public float Value
    {
        set
        {
            fillAmmount = Map(value, 0, MaxValue, 0, 1);
        }
    }



    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        HandleBar();
	}

    private void HandleBar()
    {
        if(fillAmmount != content.fillAmount)
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmmount, Time.deltaTime * lerpSpeed);
        }
    }

    //take heroes min health and max health (inMin and inMax) and translate it to a scale between 0-1 to change healthbar size.
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        // (current health - min health) * (outMax-outMin) / (maxhealth) + outmin
        // example:
        // Current health = 80
        // max health = 100
        // min health = 0
        //(80-0) * (1-0) / (100-0) + 0
        // = 80 * 1 / 100 +0 = 0.8
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
