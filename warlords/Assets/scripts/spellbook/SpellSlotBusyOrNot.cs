using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellSlotBusyOrNot : MonoBehaviour {

    private List<int> listOfBusySlots = new List<int>();

    public void addToList(int slotNameToSetToBusy)
    {
        listOfBusySlots.Add(slotNameToSetToBusy);
    }

    public void removeFromList(int slotNameToRemoveFromList)
    {   
        Debug.Log("Removed: " + slotNameToRemoveFromList);
        listOfBusySlots.Remove(slotNameToRemoveFromList);

    }

    public bool isSlotTaken(int slotToCheck)
    {

        if (listOfBusySlots.Contains(slotToCheck))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

}
