using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class Test_UIItemSlot_Assign : MonoBehaviour {
	
	private UIItemSlot slot;
    private int assignItem;

    void Awake()
	{
		if (this.slot == null)
			this.slot = this.GetComponent<UIItemSlot>();
	}
	
	void Start()
	{
		if (this.slot == null || UIItemDatabase.Instance == null)
		{
			this.Destruct();
			return;
		}

        string slotName = this.slot.name;
        this.assignItem = int.Parse(slotName.Substring(6, (slotName.IndexOf(")") - 6)));
        

        //this.slot.Assign(UIItemDatabase.Instance.GetByID(this.assignItem));
		//this.Destruct();
	}

    public void assignItemMethod(int assignItem)
    {

    }

    public void getFreshItemFromDatabase()
    {
        if (this.slot == null || UIItemDatabase.Instance == null)
        {
            this.Destruct();
            return;
        }
        if (UIItemDatabase.Instance.GetByID(this.assignItem) != null)
        {
            Debug.Log("getFreshItemFromDatabase item " + UIItemDatabase.Instance.GetByID(this.assignItem).Name + " To pos " + assignItem);
        }
        this.slot.Assign(UIItemDatabase.Instance.GetByID(this.assignItem));
    }

    public void Assign(int assignItem)
    {
        Debug.Log("Assign called: " + assignItem);
        this.assignItem = assignItem;
        if (this.slot == null || UIItemDatabase.Instance == null)
        {
            //this.Destruct();
            return;
        }
        Debug.Log("Assigned item in inventory: " + UIItemDatabase.Instance.GetByID(this.assignItem).Name);
        this.slot.Assign(UIItemDatabase.Instance.GetByID(this.assignItem));
        //this.Destruct();
    }

    private void Destruct()
	{
		DestroyImmediate(this);
	}
}
