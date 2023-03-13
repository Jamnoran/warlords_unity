using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseItems
{
    public string response_type = "HERO_ITEMS";
    public List<Item> items;

    public ResponseItems() { }

    public List<Item> getItems()
    {
        return items;
    }
}
