using UnityEngine;
using System.Collections;

public class SpellSlot : MonoBehaviour {
	
	//Spell slot type:
	public bool SpellBookSlot = false;
	public bool  SpellBarSlot = false;
	
	//Spell ID in the spell manager spells array:
	public int ID = -1;
	
	[HideInInspector]
	public SpellManager Manager;
	[HideInInspector]
	public SpellBar BarManager;
	
	void  Start (){
		Manager = FindObjectOfType(typeof(SpellManager)) as SpellManager;
		BarManager = FindObjectOfType(typeof(SpellBar)) as SpellBar;
	}
	
	
	public void  StartDragging (){
		if(SpellBookSlot == true && Manager.Spells[ID].Locked == false)
		{
			//Drag settings for this spell:
			Manager.SpellBook.DraggingSpell = true;
			Manager.SpellBook.DragID = ID;

			Manager.SpellBook.DragSlot.transform.SetParent(Manager.SpellBook.SpellBookTrans.transform, true);
			Manager.SpellBook.DragSlot.gameObject.SetActive(true);
			Manager.SpellBook.DragSlot.sprite = Manager.Spells[ID].SpellSprite;
		}
		else if(SpellBarSlot == true)
		{
			if(BarManager.Slots[ID].IsTaken == true) //Only if the spell bar slot is taken.
			{
				//Drag settings for this spell:
				BarManager.DraggingSpell = true;
				BarManager.DragID = ID;

				Manager.SpellBook.DragSlot.transform.SetParent(BarManager.UICanvasTrans.transform, true);
				BarManager.DragSlot.gameObject.SetActive(true);
				BarManager.DragSlot.sprite = Manager.Spells[BarManager.Slots[ID].SlotID].SpellSprite;
				
				BarManager.DragDestination = ID;
			}
		}
	}
	public void  StopDragging (){
		if(SpellBookSlot == true && Manager.SpellBook.DraggingSpell == true)
		{
			//Stop dragging spell:
			Manager.SpellBook.DraggingSpell = false;
			Manager.SpellBook.DragID = -1;
			
			Manager.SpellBook.DragSlot.gameObject.SetActive(false);
			
			if(Manager.SpellBook.DragDestination != -1)
			{
				//Move spell from the spell book to the spell bar slot.
				BarManager.MoveSlots(ID, Manager.SpellBook.DragDestination, false);
			}
			
			Manager.SpellBook.DragDestination = -1;
		}
		else if(SpellBarSlot == true && BarManager.DraggingSpell == true)
		{
			//Stop dragging spell.
			BarManager.DraggingSpell = false;
			BarManager.DragID = -1;
			
			BarManager.DragSlot.gameObject.SetActive(false);
			
			if(BarManager.DragDestination != -1)
			{
				//Move spell from one spell bar slot to another.
				BarManager.MoveSlots(ID, BarManager.DragDestination, true);
			}
			else
			{
				//Remove spell from spell bar.
				BarManager.RemoveSpell(ID);
			}
			
			BarManager.DragDestination = -1;
		}
	}
	
	public void  SetDragDestination (){
		if(SpellBarSlot == true)
		{
			//When the mouse enters one of the spell bar slots, set it as a drag destination.
			if(Manager.SpellBook.DraggingSpell == true)
			{
				Manager.SpellBook.DragDestination = ID;
			}
			else if(BarManager.DraggingSpell == true)
			{
				BarManager.DragDestination = ID;
			}
		}
	}
	
	public void  RemoveDragDestination (){
		if(SpellBarSlot == true)
		{
			//Mouse leaving the spell bar slot so it's no longer a drag destination.
			if(Manager.SpellBook.DraggingSpell == true)
			{
				Manager.SpellBook.DragDestination = -1;
			}
			else if(BarManager.DraggingSpell == true)
			{
				BarManager.DragDestination = -1;
			}
		}
	}

	public void CastSpell ()
	{
		if(BarManager.Slots[ID].IsTaken == true && BarManager.ClickToCast == true)
		{
			//Can the player use this spell? Checking if the player has the required amount of mana or if he can multiple spells while having active spell.
			if(Manager.ManaPoints.Mana >= Manager.Spells[BarManager.Slots[ID].SlotID].ManaNeeded && Manager.SpellCastTimer == 0 && Manager.Spells[BarManager.Slots[ID].SlotID].RechargeTimer == 0 && Manager.Spells[BarManager.Slots[ID].SlotID].Casting == false)
			{
				BarManager.Slots[ID].Icon.color = Color.white;
				if((Manager.EnableTargetingEnemies == true && Manager.SelectedEnemy != null) || (Manager.Spells[BarManager.Slots[ID].SlotID].RequireEnemy == false))
				{
					if(Manager.Spells[BarManager.Slots[ID].SlotID].CastDelay > 0) //Cast delay:
					{
						//Start the timer:
						Manager.Spells[BarManager.Slots[ID].SlotID].CastDelayTimer = Manager.Spells[BarManager.Slots[ID].SlotID].CastDelay;
						Manager.Spells[BarManager.Slots[ID].SlotID].Casting = true;
					}
					else
					{
						Manager.CallSpell(BarManager.Slots[ID].SlotID);
					}
				}
				
				if(Manager.EnableTargetingEnemies == true && Manager.SelectedEnemy == null && Manager.Spells[BarManager.Slots[ID].SlotID].RequireEnemy == true) Debug.Log("Please select an enemy");
				
			}   
		}
	}
	

}