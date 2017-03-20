
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellBar : MonoBehaviour {

	[System.Serializable]
	public class SpellBarSlotVars
	{
		public GameObject Obj;
		public Image Icon;
		public KeyCode TriggerKey = KeyCode.Alpha0;
		[HideInInspector]
		public bool  IsTaken = false;
		[HideInInspector]
		public int SlotID = -1;
	}
	public SpellBarSlotVars[] Slots;

	public bool ClickToCast = true; //Enable or disable clicking on spell bar slots to cast spells.
	
	//UI vars:
	public RectTransform UICanvasTrans;
	
	public Image DragSlot;
	[HideInInspector]
	public bool  DraggingSpell = false;
	[HideInInspector]
	public int DragID = -1;
	[HideInInspector]
	public int DragDestination = -1;
	
	[HideInInspector]
	public SpellManager Manager;
	
	void  Start (){
		Manager = FindObjectOfType(typeof(SpellManager)) as SpellManager;
		LoadSpellBar ();
		RefreshSlots();
	}
	
	void  Update (){
		//Dragging slot position:
		if(DraggingSpell)
		{
			Vector3 TempPos = Input.mousePosition - UICanvasTrans.localPosition;
			DragSlot.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x+DragSlot.GetComponent<RectTransform>().rect.width/2,TempPos.y-DragSlot.GetComponent<RectTransform>().rect.height,0);
		}
		
		//Trigger skill items keys:
		for(int i = 0; i < Slots.Length; i++) //Starting a loop in the skill bar slots.
		{
			if(Slots[i].IsTaken == true)
			{
				//Can the player use this spell? Checking if the player has the required amount of mana or if he can multiple spells while having active spell.
				if(Manager.ManaPoints.Mana >= Manager.Spells[Slots[i].SlotID].ManaNeeded && Manager.SpellCastTimer == 0 && Manager.Spells[Slots[i].SlotID].RechargeTimer == 0 && Manager.Spells[Slots[i].SlotID].Casting == false)
				{
					Slots[i].Icon.color = Color.white;
					if(Input.GetKeyDown(Slots[i].TriggerKey)) //Calling the trigger key.
					{
                        Debug.Log(Slots[i].Icon.sprite.name); 
					}
				} 
				else
				{
					Slots[i].Icon.color = Color.grey;
				}   
			}
		}
	}
	
	public void  RefreshSlots (){
		for(int i = 0; i<Slots.Length;i++)
		{
			if(Slots[i].Obj.gameObject.GetComponent<SpellSlot>().ID == -1)
			{
				Slots[i].Obj.gameObject.GetComponent<SpellSlot>().ID = i; //Set spell ID.
			}
			
			if(Slots[i].IsTaken == true)
			{
				Slots[i].Icon.gameObject.SetActive(true);
				Slots[i].Icon.sprite = Manager.Spells[Slots[i].SlotID].SpellSprite;
			}
			else
			{
				Slots[i].Icon.gameObject.SetActive(false);
				Slots[i].Icon.color = Color.white;
			}
			
		}
	}
	
	public void  MoveSlots ( int Spell ,   int Destination ,   bool FromOtherSlot  ){
		if(Destination == Spell && Slots[Destination].IsTaken == true && FromOtherSlot == true) return;
		
		if(FromOtherSlot == false) //Adding a spell from the spell book to the spell bar:
		{
			Slots[Destination].IsTaken = true;
			Slots[Destination].SlotID = Spell;
		}
		else //Moving spell from one spell bar slot to another.
		{
			if(Slots[Destination].IsTaken == true) //If the destination slot is already taken:
			{
				//Swap both spells:
				int TempID = Slots[Destination].SlotID;
				
				Slots[Destination].SlotID = Slots[Spell].SlotID;
				Slots[Spell].SlotID = TempID;
			}
			else
			{
				//Simply move the spell only:
				Slots[Spell].IsTaken = false;
				Slots[Destination].IsTaken = true;
				
				Slots[Destination].SlotID = Slots[Spell].SlotID;
			}
		}
		
		SaveSpellBar();
		RefreshSlots();
	}
	
	public void  RemoveSpell ( int ID  ){
		Slots[ID].IsTaken = false;
		
		SaveSpellBar();
		RefreshSlots();
	}
	
	public void  SaveSpellBar (){
		for(int i = 0; i < Slots.Length;i++)
		{  
			if(Slots[i].IsTaken == true)
			{
				PlayerPrefs.SetInt("SpellBarSlotLocked"+i.ToString(),1);
				PlayerPrefs.SetInt("SpellBarSlotID"+i.ToString(),Slots[i].SlotID);
			}
			else
			{
				PlayerPrefs.SetInt("SpellBarSlotLocked"+i.ToString(),0);
			}
		}
	}
	
	public void  LoadSpellBar (){
		for(int i = 0; i < Slots.Length;i++)
		{  
			if(PlayerPrefs.GetInt("SpellBarSlotLocked"+i.ToString(),0) == 1)
			{
				Slots[i].IsTaken = true;
				Slots[i].SlotID = PlayerPrefs.GetInt("SpellBarSlotID"+i.ToString(),-1);
			}
		}
	}
	
}