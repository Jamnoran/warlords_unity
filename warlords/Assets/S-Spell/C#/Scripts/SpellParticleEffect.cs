/*

Spell Particle Effect script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellParticleEffect : MonoBehaviour {
	
	public GameObject Player;
	public int Damage;
	
	public bool  DamageOnce = true;
	public bool  DestroyOnDamage = false;
	[HideInInspector]
	public bool  DidDamage = false;
	[HideInInspector]
	public GameObject ParentObj;
	public GameObject[] DamagedTargets;
	
	void  Start (){
		DamagedTargets = new GameObject[0];
		DidDamage = false;
	}
	
	void  OnParticleCollision ( GameObject other  ){
		//Adding damage points:
		if((DamageOnce == true && DidDamage == false) || DamageOnce == false) //if the particle effect can produce damage once and it hasn't done that yet or if it can damage enemies multiple times.
		{
			if(other.gameObject.GetComponent<SpellDamage>())
			{
				//If the particle effect has done damage to this enemy then we won't do damage again.
				if(DamagedTargets.Length > 0)
				{
					for(int i = 0; i < DamagedTargets.Length; i++)
					{
						if(DamagedTargets[i] == other.gameObject)
						{
							return;
						}
					}
				}
				//Apply damage to enemy:
				other.gameObject.GetComponent<SpellDamage>().AddHealth(-Damage);
				
				//Destroy on first damage?
				if(DestroyOnDamage == true)
				{
					Destroy(ParentObj);
				}

				List<GameObject> TempContent = new List<GameObject>(DamagedTargets);
				TempContent.Remove(other.gameObject);
				DamagedTargets = TempContent.ToArray();

				DidDamage = true;
			}
		}
	}
}