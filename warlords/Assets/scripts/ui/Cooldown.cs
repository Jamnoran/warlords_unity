using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.
using Assets.scripts.util;

public class Cooldown : MonoBehaviour
{
    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    public void setCooldown(int position, string timeOfCooldown, int abilityId)
    {
        long currentMillis = DeviceUtil.getMillis();
        long timeOffCd = long.Parse(timeOfCooldown);
        long timeUntillOffCd = (timeOffCd - currentMillis);
        Debug.Log("Millis left until of cd: " + timeUntillOffCd + " Position of ability : " + position);
        float timeUntillOfCdInFloat = (timeUntillOffCd / 1000);
        GameObject cooldownObject = GameObject.Find("Slot " + position);
        Transform cd = cooldownObject.transform.Find("Cooldown");
        UISlotCooldown cooldownScript = ((UISlotCooldown)cd.GetComponent(typeof(UISlotCooldown)));
        Debug.Log("Children : " + cd.name);
        cooldownScript.StartCooldown(abilityId, timeUntillOfCdInFloat);
    }
}