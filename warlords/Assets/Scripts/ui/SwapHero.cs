using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapHero : MonoBehaviour {

    public GameObject warrior;
    public GameObject priest;
    public GameObject warlock;

    private GameObject tempWarrior;
    private GameObject tempWarlock;
    private GameObject tempPriest;

    public void SetWarrior()
    {
        if (tempWarlock)
        {
            Destroy(tempWarlock);
        }

        if (tempPriest)
        {
            Destroy(tempPriest);
        }

        if (!tempWarrior)
        {
            tempWarrior = Instantiate(warrior);
        }
    }

    public void SetWarlock()
    {
        if (tempWarrior)
        {
            Destroy(tempWarrior);
        }

        if (tempPriest)
        {
            Destroy(tempPriest);
        }
        if (!tempWarlock)
        {
            tempWarlock = Instantiate(warlock);
        }
    }

    public void SetPriest()
    {
        if (tempWarrior)
        {
            Destroy(tempWarrior);
        }

        if (tempWarlock)
        {
            Destroy(tempWarlock);
        }
        if (!tempPriest)
        {
            tempPriest = Instantiate(priest);
        }
    }

}
