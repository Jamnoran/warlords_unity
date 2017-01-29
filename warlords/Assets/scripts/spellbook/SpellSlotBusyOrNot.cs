using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellSlotBusyOrNot : MonoBehaviour {

    private Dictionary<string, string> listOfBusySlots = new Dictionary<string, string>();

    public void Update()
    {
      
    }

    public void addToList(string slotNameToSetToBusy, string spellToAdd)
    {
        listOfBusySlots.Add(slotNameToSetToBusy, spellToAdd);
    }

    public void removeFromList(string slotNameToRemoveFromList)
    {
        Debug.Log("Removed: " + slotNameToRemoveFromList);
        listOfBusySlots.Remove(slotNameToRemoveFromList);

    }

    public bool isSlotTaken(string slotToCheck)
    {

        if (listOfBusySlots.ContainsKey(slotToCheck))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

}
