using UnityEngine;
using System.Collections;

public class SpellEvents : MonoBehaviour {
	
	public bool DebugEvent = true;
	
	public void  OnSpellCastStart ( string SpellName  ){
		if(DebugEvent == true) Debug.Log("Spell '"+SpellName+"' cast has started.");
	}
	
	public void  OnSpellCastEnd ( string SpellName  ){
		if(DebugEvent == true) Debug.Log("Spell '"+SpellName+"' cast has ended.");
	}
}