using UnityEngine;
using System.Collections;

public class SpellDamage : MonoBehaviour {
	
	
	//Eneym health:
	public int Health;
	public int MaxHealth = 100;
	
	//health text:
	public TextMesh HPText;

	[HideInInspector]
	SpellManager Manager;
	
	void  Start (){
		Health = MaxHealth;
		Manager = FindObjectOfType (typeof(SpellManager)) as SpellManager;
	}
	
	void  Update (){
		
	}
	
	public void  AddHealth ( int Points  ){
		if(Health+Points > MaxHealth)
		{
			Health = MaxHealth;
		}
		else
		{
			Health += Points;
		}
		
		if(HPText) HPText.text = Health.ToString()+"/"+MaxHealth.ToString();
		
		if(Health <= 0)
		{
			Die();
		}
	}
	
	public void  Die (){
		if(HPText) HPText.text = "Dead!";

		if(Manager.EnableTargetingEnemies == true)
		{
			if(Manager.SelectedEnemy == this.gameObject) Manager.SelectedEnemy = null;
		}
	}
	
	void OnMouseDown ()
	{
		if(Manager.EnableTargetingEnemies == true)
		{
			Manager.SelectedEnemy = this.gameObject;
		}
	}
}