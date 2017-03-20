
using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour {
	
	public string Name;
	public GameObject Player;
	public GameObject AudioSourceObj;

	public bool EnableTargetingEnemies = false;
	public Vector3 SelectedEnemyPos;

	[System.Serializable]
	public class SpellPEVars
	{
		public float Duration = 0.5f;
		public float Delay = 0;
		public float Timer;
		
		public int MinDamage = 10;
		public int MaxDamage = 20;
		public bool  DamageOnce = true;
		public bool  DestroyOnDamage = false;
		
		public GameObject Prefab; //Particle effect prefab:
		public GameObject[] CollisionParticles; //The particle effect that will result in damage on collision with the enemy object.
		public float PrefabDestroyTimer = 2;
		public Transform LaunchPosition;
		public Vector3 MvtVector;
		public GameObject Obj;
		
		//Particle effect movement:
		public float Speed = 3;
		public bool  Move = true;
		
		//Follow object:
		public bool  Follow = false;
		public GameObject ObjToFollow;
		[HideInInspector]
		public Vector3 Velocity = Vector3.zero;
		public float SmoothTime = 1;
		
		public bool  Created = false;
	}
	public SpellPEVars[] ParticleEffects;
	public int PEProgress = 0;
	public bool  PEDone = false;

	[System.Serializable]
	public class SpellAudioVars 
	{
		public float Duration = 1.0f;
		public float Delay = 0.0f;
		public float Timer;
		
		public AudioClip Clip;
		public bool  Created = false;
		public AudioSource CurrentSC;
		
		public bool  Loop = false;
	}
	public SpellAudioVars[] AudioEffects;
	public int AudioProgress = 0;
	public bool AudioDone = false;
	
	//Animations:
	[System.Serializable]
	public class AnimationVars
	{
		public float Duration = 1.0f;
		public float Delay = 0.0f;
		public float Timer;
		
		public AnimationClip Clip;
		public bool  Created = false;
	}
	public AnimationVars[] AnimationEffects;
	public int AnimProgress = 0;
	public bool  AnimDone = false;
	
	[HideInInspector]
	public SpellEvents CustomEvents;
	
	void  Start (){
		CustomEvents = FindObjectOfType(typeof(SpellEvents)) as SpellEvents;
		
		//Initiating particle effects for this spell:
		PEProgress = 0;
		PEDone = false;
		if(ParticleEffects.Length == 0) 
		{
			PEDone = true;
		}
		else
		{
			//First particle effect:
			ParticleEffects[PEProgress].Timer = ParticleEffects[PEProgress].Duration + ParticleEffects[PEProgress].Delay;
			
			if(ParticleEffects[PEProgress].Prefab && ParticleEffects[PEProgress].Timer == ParticleEffects[PEProgress].Duration)
			{
				CreateParticleEffect ();
			}
		}
		
		
		if(AnimationEffects.Length == 0) 
		{
			AnimDone = true;
		}
		else
		{    
			//Play audio effects:
			AnimationEffects[AnimProgress].Timer = AnimationEffects[AnimProgress].Duration + AnimationEffects[AnimProgress].Delay;
			
			if(AnimationEffects[AnimProgress].Clip && AnimationEffects[AnimProgress].Timer == AnimationEffects[AnimProgress].Duration)
			{
				CreateAnimationEffect ();
			}
		}
		
		if(AudioEffects.Length == 0) 
		{
			AudioDone = true;
		}
		else
		{    
			//Play audio effects:
			AudioEffects[AudioProgress].Timer = AudioEffects[AudioProgress].Duration + AudioEffects[AudioProgress].Delay;
			
			if(AudioEffects[AudioProgress].Clip && AudioEffects[AudioProgress].Timer == AudioEffects[AudioProgress].Duration)
			{
				CreateAudioEffect ();
			}
		}
	}
	
	void  Update (){
		//Particle effects:
		if(PEDone == false)
		{
			if(ParticleEffects[PEProgress].Timer > 0)
			{
				ParticleEffects[PEProgress].Timer -= Time.deltaTime;
				
				if(ParticleEffects[PEProgress].Timer < ParticleEffects[PEProgress].Duration)
				{
					if(ParticleEffects[PEProgress].Created == false)
					{
						Debug.Log(ParticleEffects[PEProgress].Timer);
						CreateParticleEffect ();
					} 
					if(ParticleEffects[PEProgress].Obj) 
					{
						if(ParticleEffects[PEProgress].Follow == true)
						{
							ParticleEffects[PEProgress].Obj.transform.position = Vector3.SmoothDamp(ParticleEffects[PEProgress].Obj.transform.position, ParticleEffects[PEProgress].ObjToFollow.transform.position, ref ParticleEffects[PEProgress].Velocity, ParticleEffects[PEProgress].SmoothTime);
						}
						else if(ParticleEffects[PEProgress].Move == true)
						{
							ParticleEffects[PEProgress].Obj.transform.Translate((ParticleEffects[PEProgress].MvtVector) * Time.deltaTime * ParticleEffects[PEProgress].Speed, Space.Self);
						}
					}
				}
			}
			else
			{
				if(PEProgress == ParticleEffects.Length-1)
				{
					PEDone = true;
				}
				else
				{
					PEProgress++;
					ParticleEffects[PEProgress].Timer = ParticleEffects[PEProgress].Duration + ParticleEffects[PEProgress].Delay;
					
					if(ParticleEffects[PEProgress].Prefab && ParticleEffects[PEProgress].Timer == ParticleEffects[PEProgress].Duration)
					{
						CreateParticleEffect ();
					}
				}
			}
		}
		
		//Audio Effects:
		if(AudioDone == false)
		{
			if(AudioEffects[AudioProgress].Timer > 0)
			{
				AudioEffects[AudioProgress].Timer -= Time.deltaTime;
				
				if(AudioEffects[AudioProgress].Timer < AudioEffects[AudioProgress].Duration)
				{
					if(AudioEffects[AudioProgress].Created == false)
					{
						CreateAudioEffect ();
					}
				}
			}
			else
			{
				Destroy(AudioEffects[AudioProgress].CurrentSC);
				if(AudioProgress == AudioEffects.Length-1)
				{
					AudioDone = true;
				}
				else
				{
					AudioProgress++;
					AudioEffects[AudioProgress].Timer = AudioEffects[AudioProgress].Duration + AudioEffects[AudioProgress].Delay;
					
					if(AudioEffects[AudioProgress].Clip && AudioEffects[AudioProgress].Timer == AudioEffects[AudioProgress].Duration)
					{
						CreateAudioEffect ();
					}
				}
			}
		}
		
		//Animation Effects:
		if(AnimDone == false)
		{
			if(AnimationEffects[AnimProgress].Timer > 0)
			{
				AnimationEffects[AnimProgress].Timer -= Time.deltaTime;
				
				if(AnimationEffects[AnimProgress].Timer < AnimationEffects[AnimProgress].Duration)
				{
					if(AnimationEffects[AnimProgress].Created == false)
					{
						CreateAnimationEffect ();
					}
				}
			}
			else
			{
				if(AnimProgress == AnimationEffects.Length-1)
				{
					AnimDone = true;
				}
				else
				{
					AnimProgress++;
					AnimationEffects[AnimProgress].Timer = AnimationEffects[AnimProgress].Duration + AnimationEffects[AnimProgress].Delay;
					
					if(AnimationEffects[AnimProgress].Clip && AnimationEffects[AnimProgress].Timer == AnimationEffects[AnimProgress].Duration)
					{
						CreateAnimationEffect ();
					}
				}
			}
		}
		
		if(PEDone == true && AudioDone == true && AnimDone == true)
		{
			if(CustomEvents) CustomEvents.OnSpellCastEnd(Name);
			Destroy(this.gameObject);
		}
	}
	
	void  CreateParticleEffect (){
		ParticleEffects[PEProgress].Obj = (GameObject) Instantiate (ParticleEffects[PEProgress].Prefab, ParticleEffects[PEProgress].LaunchPosition.position, ParticleEffects[PEProgress].Prefab.transform.rotation);
		Destroy(ParticleEffects[PEProgress].Obj, ParticleEffects[PEProgress].PrefabDestroyTimer);
		
		if(ParticleEffects[PEProgress].CollisionParticles.Length > 0)
		{
			for(int i = 0; i < ParticleEffects[PEProgress].CollisionParticles.Length; i++)
			{
				
				//Searching for the collision particle:
				if(ParticleEffects[PEProgress].Prefab != ParticleEffects[PEProgress].CollisionParticles[i])
				{
					int ChildPos = -1;
					for (int j = 0; j < ParticleEffects[PEProgress].Prefab.transform.childCount; j++)
					{
						if(ParticleEffects[PEProgress].Prefab.transform.GetChild(j).gameObject == ParticleEffects[PEProgress].CollisionParticles[i])
						{
							ChildPos = j;
						}
					}
					
					if(ChildPos >= 0)
					{
						ParticleEffects[PEProgress].CollisionParticles[i] = ParticleEffects[PEProgress].Obj.transform.GetChild(ChildPos).gameObject;
					}
					else //Invalid collision particle object:
					{
						Debug.LogError("Invalid particle collision object");
					}
				}
				else
				{
					ParticleEffects[PEProgress].CollisionParticles[i] = ParticleEffects[PEProgress].Obj;
				} 
				
				//Adding the particle effect damage script to the collision particle:
				ParticleEffects[PEProgress].CollisionParticles[i].AddComponent<SpellParticleEffect>();
				ParticleEffects[PEProgress].CollisionParticles[i].GetComponent<SpellParticleEffect>().Player = Player;
				ParticleEffects[PEProgress].CollisionParticles[i].GetComponent<SpellParticleEffect>().Damage = Random.Range(ParticleEffects[PEProgress].MinDamage, ParticleEffects[PEProgress].MaxDamage);
				ParticleEffects[PEProgress].CollisionParticles[i].GetComponent<SpellParticleEffect>().DamageOnce = ParticleEffects[PEProgress].DamageOnce;
				ParticleEffects[PEProgress].CollisionParticles[i].GetComponent<SpellParticleEffect>().DestroyOnDamage = ParticleEffects[PEProgress].DestroyOnDamage;
				ParticleEffects[PEProgress].CollisionParticles[i].GetComponent<SpellParticleEffect>().ParentObj = ParticleEffects[PEProgress].Obj;
				
			}
		}
		
		
		ParticleEffects[PEProgress].Created = true;    
		
	}
	
	void  CreateAudioEffect (){
		if(AudioSourceObj != null)
		{
			//Add the audio source component:
			AudioSourceObj.gameObject.AddComponent<AudioSource>();
			AudioSource AudioSC = AudioSourceObj.gameObject.GetComponent<AudioSource>();
			
			//Play the audio clip:
			AudioSC.loop = AudioEffects[AudioProgress].Loop;
			AudioSC.clip = AudioEffects[AudioProgress].Clip;
			AudioSC.Play ();
			
			AudioEffects[AudioProgress].CurrentSC = AudioSC;
		}
		
		AudioEffects[AudioProgress].Created = true;    
	}
	
	void  CreateAnimationEffect (){
		Animation Anim= Player.gameObject.GetComponent<Animation>();
		
		Anim.clip = AnimationEffects[AnimProgress].Clip;
		Anim.Play();
		
		AnimationEffects[AnimProgress].Created = true;    
	}
}