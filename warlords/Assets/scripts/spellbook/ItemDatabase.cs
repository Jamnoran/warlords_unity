using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDatabase : MonoBehaviour {
    //create list of items
    private List<Item> itemDatabase = new List<Item>();
    private JsonData itemData;

    /**
     * Fetch the Json file and convert it to a item, boot up other functions
     **/
    void Start()
    {
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json")); //fetches a dictionary of items
        constructItemDatabase();
        Debug.Log(fetchItemByID(0).Description);
    }
    /**
     * Get an item from the database from ID
     * @param id The items ID you are looking for
     * @return Item The item matching the param ID if successful, null otherwise
     **/
    public Item fetchItemByID(int id)
    {
        for (int i = 0; i < itemDatabase.Count; i++)
        {
            if (id == itemDatabase[i].ID)
            {
                return itemDatabase[i];
            }
        }
        return null;
    }
    /**
     * Constructs an item database by iterating trough the Json object
    **/
    void constructItemDatabase()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            itemDatabase.Add(new Item((int)itemData[i]["id"], itemData[i]["title"].ToString(), (int)itemData[i]["value"], itemData[i]["description"].ToString(), (double)itemData[i]["cooldown"], itemData[i]["slug"].ToString()));
        }
    }
}

public class Item
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Description { get; set;}
    public double Cooldown { get; set; }
    public int Value { get; set; }
    public string Slug { get; set; }
    public Sprite Sprite { get; set; }

    /**
     * Constructor for Item, sets the fields to the value when created
     **/
    public Item(int id, string title, int value, string description, double cooldown, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Description = description;
        this.Cooldown = cooldown;
        this.Value = value;
        this.Sprite = Resources.Load<Sprite>("Sprites/Spellbook/" + slug);

    }
    /**
     * Overloaded constructor if something went horribly wrong
     **/
    public Item()
    {
        this.ID = -1;
    }
}
