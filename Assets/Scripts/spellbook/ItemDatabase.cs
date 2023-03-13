using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.scripts.vo;

public class ItemDatabase : MonoBehaviour {
    //create list of items
    private List<Ability> abilities = null;

    void Start()
    {
      
    }



    /**
     * Get an item from the database from ID
     * @param id The items ID you are looking for
     * @return Item The item matching the param ID if successful, null otherwise
     **/
    public Ability fetchItemByID(int id)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (id == abilities[i].id)
            {
                return abilities[i];
            }
        }
        return null;
    }


}


