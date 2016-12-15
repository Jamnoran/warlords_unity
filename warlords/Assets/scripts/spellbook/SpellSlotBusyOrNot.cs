using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellSlotBusyOrNot : MonoBehaviour {

    private List<string> itemOccupyingSlot = new List<string>();
    private List<Vector3> lockedPosition = new List<Vector3>();

    public void addToList(string addThisToList)
    {
        itemOccupyingSlot.Add(addThisToList);
    }

    public bool isInList(string isThisInList)
    {
        return itemOccupyingSlot.Contains(isThisInList);
    }

    public void removeFromList(string nameToRemove)
    {
        itemOccupyingSlot.Remove(nameToRemove);
        Debug.Log("Succesfully removed item " + nameToRemove);
    }

    //---Cordinates----

    public void lockPosition(Vector3 lockThisPosition)
    {
        lockedPosition.Add(lockThisPosition);
        Debug.Log("Locked position:" + lockThisPosition);
    }

    public bool isLocked(Vector3 isThisInList)
    {
        Debug.Log("is locked?: " + isThisInList);
        return lockedPosition.Contains(isThisInList);
    }

    public bool checkIfInActionBar(Vector3 checkIfThisIsInActionBar)
    {
        if (lockedPosition.Contains(checkIfThisIsInActionBar))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void unLockPosition(Vector3 unLockThisPosition)
    {
        lockedPosition.Remove(unLockThisPosition);
    }

}
