using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class Cooldown : MonoBehaviour
{

    public Image cooldown;
    public bool coolingDown;
    public float waitTime = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p") && !coolingDown && cooldown != null)
        {
            cooldown.fillAmount = 1.0f;
            coolingDown = true;
        }
        if (coolingDown) { 
            cooldown.fillAmount -= 1.0f / waitTime * Time.deltaTime;
        }
        if (cooldown != null && cooldown.fillAmount == 0.0f)
        {
            coolingDown = false;
        }
    }
}