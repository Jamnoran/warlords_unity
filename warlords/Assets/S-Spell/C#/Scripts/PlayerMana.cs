using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour {
	
	public int Mana;
	public int MaxMana = 500;
	
	public Text ManaText;
	
	void  Start (){
		if(ManaText) ManaText.text = Mana.ToString()+"/"+MaxMana.ToString();
	}
	
	void  Update (){
		
	}
	
	public void  AddMana ( int Points  ){
		if(Mana+Points > MaxMana)
		{
			Mana = MaxMana;
		}
		else
		{
			Mana += Points;
		}
		
		if(ManaText) ManaText.text = Mana.ToString()+"/"+MaxMana.ToString();
	}
}