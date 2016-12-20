using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellSlotBusyOrNot : MonoBehaviour {

    private List<string> listOfBusySlots = new List<string>();

    public void Update()
    {
      
    }

    public void addToList(string slotNameToSetToBusy)
    {
        listOfBusySlots.Add(slotNameToSetToBusy);
        for (int i = 0; i < listOfBusySlots.Count; i++)
        {
            Debug.Log(listOfBusySlots[i]);
        }
    }

}
