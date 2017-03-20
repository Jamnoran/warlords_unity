
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour {
	
	public GameObject Player;
	
	//Particle effects:
	[System.Serializable]
	public class ParticleEffectsVars
	{
		public float Duration = 0.5f;
		public float Delay = 0;
		
		public int MinDamage = 10;
		public int MaxDamage = 20;
		public bool  DamageOnce = true;
		public bool  DestroyOnDamage = false;
		
		public GameObject Prefab; //Particle effect prefab:
		public GameObject[] CollisionParticles; //The particle effect that will result in damage on collision with the enemy object.
		public float PrefabDestroyTimer = 2;
		//Movement direction:
		public Transform LaunchPosition;
		public Transform FrontPosition;
		
		//Follow object:
		public bool  Follow = false;
		public GameObject ObjToFollow;
		[HideInInspector]
		public Vector3 Velocity = Vector3.zero;
		public float SmoothTime = 1;
		
		//Particle effect movement:
		public bool  Move = true;
		public float Speed = 3;   
	}

	public ParticleEffectsVars[] ParticleEffects;
	
	//Audio clips: Audio clips to be played during the spell cast
	[System.Serializable]
	public class AudioVars 
	{
		public float Duration = 1.0f;
		public float Delay = 0.0f;
		
		public AudioClip Clip;
		
		public bool  Loop = false;
	}
	
	//Animations:
	[System.Serializable]
	public class AnimVars
	{
		public float Duration = 1.0f;
		public float Delay = 0.0f;
		
		public AnimationClip Clip;
	}

	[System.Serializable]
	public class SpellsVars
	{
		[HideInInspector]
		public bool  Active = false;
		[HideInInspector]
		public string Name;
		[HideInInspector]
		public string Description;
		[HideInInspector]
		public Sprite SpellSprite;
		[HideInInspector]
		public int ManaNeeded = 10; //mana points needed to trigger the spell.
		[HideInInspector]
		public float CastDelay = 0.5f; //Time (in seconds) before the spell is casted.
		[HideInInspector]
		public float CastDelayTimer = 0;
		[HideInInspector]
		public bool  Casting = false;
		[HideInInspector]
		public float Recharge = 3; //Recharge time in secodns.
		[HideInInspector]
		public float RechargeTimer = 0;
		public bool  UseTriggerKey = true; //Use a key to trigger the spell?
		public KeyCode TriggerKey = KeyCode.Alpha1; //Spell trigger key.
		[HideInInspector]
		public bool RequireEnemy = false; //If the spell does damage to enemies and spells follow enemies directly ("EnableTargetingEnemies" is true), set this to true.
		
		//UI:
		[HideInInspector]
		public Image SpellImage;
		[HideInInspector]
		public Text SpellDescriptionText;
		[HideInInspector]
		public bool  UnlockedByDefault = true;
		[HideInInspector]
		public 	bool  Locked = false;
		[HideInInspector]
		public int XPToUnlock = -1;
		[HideInInspector]
		public int LevelToUnlock = -1;

		public ParticleEffectsVars[] ParticleEffects;
		public AudioVars[] AudioEffects;
		
		public AnimVars[] AnimationEffects;
		
		
	}  
	public SpellsVars[] Spells;
	
	
	
	public float SpellCastReload = 3;
	[HideInInspector]
	public float SpellCastTimer = 0;

	public bool EnableTargetingEnemies = false; //This means that you have to select an enemy to cast a spell at and the spell will automatically pass through this enemy.
	[HideInInspector]
	public GameObject SelectedEnemy;
	
	public GameObject AudioSourceObj;
	public Animation AnimController;
	
	//Spellbook vars:
	[System.Serializable]
	public class SpellBookVars
	{
		public KeyCode SpellBookKey = KeyCode.H;
		[HideInInspector]
		public bool  ShowSpellBook = false;
		
		//Drag and drop Spellbook UI panel:
		public bool  IsMovable = true; //Can the player change the position of the inventory GUI in game?
		[HideInInspector]
		public bool  Dragging = false;
		public GameObject PanelDragPos;
		
		[HideInInspector]
		public 	int PosDiffX;
		[HideInInspector]
		public int PosDiffY;
		
		//UI vars:
		public RectTransform UICanvasTrans;
		public RectTransform SpellBookTrans;
		
		public Image DragSlot;
		[HideInInspector]
		public bool  DraggingSpell = false;
		[HideInInspector]
		public int DragID = -1;
		[HideInInspector]
		public int DragDestination = -1;
	}
	public SpellBookVars SpellBook;

	
	
	[HideInInspector]
	public PlayerMana ManaPoints;
	[HideInInspector]
	public ExperienceManager XPManager;
	[HideInInspector]
	public SpellEvents CustomEvents;
	
	void  Start (){
		SpellCastTimer = 0;
		ManaPoints = FindObjectOfType(typeof(PlayerMana)) as PlayerMana;
		XPManager = FindObjectOfType(typeof(ExperienceManager)) as ExperienceManager;
		CustomEvents = FindObjectOfType(typeof(SpellEvents)) as SpellEvents;
		
		RefreshSpellBook ();
	}
	
	
	void  Update (){
		if(Input.GetKeyDown(SpellBook.SpellBookKey)) //Check if the player pressed the I key which enables/disables the inventory.
		{
			ToggleSpellBook();
		}
		
		for(int i = 0; i < Spells.Length; i++)
		{
			//Recharging spell:
			if(Spells[i].RechargeTimer > 0)
			{
				Spells[i].RechargeTimer -= Time.deltaTime;
			}
			if(Spells[i].RechargeTimer < 0)
			{
				Spells[i].RechargeTimer = 0;
			}
			
			//Casting delay timer:
			if(Spells[i].CastDelayTimer > 0)
			{
				Spells[i].CastDelayTimer -= Time.deltaTime;
			}
			if(Spells[i].CastDelayTimer < 0)
			{
				Spells[i].CastDelayTimer = 0;
				Spells[i].Casting = false;
				
				CallSpell(i); 
			}
			
			//Can the player use this spell? Checking if the player has the required amount of mana or if he can multiple spells while having active spell.
			if(ManaPoints.Mana >= Spells[i].ManaNeeded && SpellCastTimer == 0 && Spells[i].RechargeTimer == 0 && Spells[i].Casting == false)
			{
				if(Spells[i].UseTriggerKey == true) //Can the player use a trigger key to call the spell.
				{
					if(Input.GetKeyDown(Spells[i].TriggerKey)) //Calling the trigger key.
					{
						if((EnableTargetingEnemies == true && SelectedEnemy != null) || (Spells[i].RequireEnemy == false))
						{
							if(Spells[i].CastDelay > 0) //Cast delay:
							{
								//Start the timer:
								Spells[i].CastDelayTimer = Spells[i].CastDelay;
								Spells[i].Casting = true;
							}
							else
							{
								CallSpell(i); 
							}
						}
						
						if(EnableTargetingEnemies == true && SelectedEnemy == null && Spells[i].RequireEnemy == true) Debug.Log("Please select an enemy");
					}
				}    
			}
		}
		
		//Casting spells timer, if set to 0, then the player doesn't have to wait to call a spell after casting another one.
		if(SpellCastTimer > 0)
		{
			SpellCastTimer -= Time.deltaTime;
		}
		if(SpellCastTimer < 0)
		{
			SpellCastTimer = 0;
		}
		
		
		//Dragging slot position:
		if(SpellBook.DraggingSpell)
		{
			Vector3 TempPos = Input.mousePosition - SpellBook.UICanvasTrans.localPosition - SpellBook.SpellBookTrans.localPosition;
			SpellBook.DragSlot.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x+SpellBook.DragSlot.GetComponent<RectTransform>().rect.width/2,TempPos.y-SpellBook.DragSlot.GetComponent<RectTransform>().rect.height,0);
		}
		
		//Dragging the spellbook panel:
		if(SpellBook.Dragging == true)
		{
			Vector3 TempPos2 = Input.mousePosition - SpellBook.UICanvasTrans.localPosition;
			SpellBook.PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos2.x-SpellBook.PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos2.y-SpellBook.PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
		}  
	}
	
	public void  CallSpell ( int ID  ){   
		//Custom events:
		if(CustomEvents) CustomEvents.OnSpellCastStart(Spells[ID].Name);
		
		//Reduce mana from the player:
		ManaPoints.AddMana(-Spells[ID].ManaNeeded);
		
		//Recharging the spell:
		Spells[ID].RechargeTimer = Spells[ID].Recharge;
		SpellCastTimer = SpellCastReload;
		
		//Creating a new empty object which will handle casting this spell:
		GameObject SpellObj= new GameObject();
		SpellObj.transform.SetParent(this.transform, true);
		
		//Assigning the spell info to this new object:
		SpellObj.AddComponent<Spell>();
		SpellObj.GetComponent<Spell>().Player = Player;
		SpellObj.GetComponent<Spell>().Name = Spells[ID].Name;
		SpellObj.GetComponent<Spell>().AudioSourceObj = AudioSourceObj;
		SpellObj.GetComponent<Spell>().EnableTargetingEnemies = EnableTargetingEnemies;
		if(SelectedEnemy != null)
		{
			SpellObj.GetComponent<Spell>().SelectedEnemyPos = SelectedEnemy.transform.position;
		}
		
		//Spell particle effects:
		SpellObj.GetComponent<Spell>().ParticleEffects = new Spell.SpellPEVars[Spells[ID].ParticleEffects.Length];
		if(SpellObj.GetComponent<Spell>().ParticleEffects .Length > 0)
		{
			for(int i = 0; i < Spells[ID].ParticleEffects.Length; i++)
			{
				SpellObj.GetComponent<Spell>().ParticleEffects[i] = new Spell.SpellPEVars();
				
				SpellObj.GetComponent<Spell>().ParticleEffects[i].Duration = Spells[ID].ParticleEffects[i].Duration;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].Delay = Spells[ID].ParticleEffects[i].Delay;
				
				SpellObj.GetComponent<Spell>().ParticleEffects[i].MinDamage = Spells[ID].ParticleEffects[i].MinDamage;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].MaxDamage = Spells[ID].ParticleEffects[i].MaxDamage;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].DamageOnce = Spells[ID].ParticleEffects[i].DamageOnce;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].DestroyOnDamage = Spells[ID].ParticleEffects[i].DestroyOnDamage;
				
				SpellObj.GetComponent<Spell>().ParticleEffects[i].Prefab = Spells[ID].ParticleEffects[i].Prefab;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].CollisionParticles = new GameObject[Spells[ID].ParticleEffects[i].CollisionParticles.Length];
				if(Spells[ID].ParticleEffects[i].CollisionParticles.Length > 0)
				{
					for(int j = 0; j < Spells[ID].ParticleEffects[i].CollisionParticles.Length; j++)
					{
						SpellObj.GetComponent<Spell>().ParticleEffects[i].CollisionParticles[j] = new GameObject(); 
						SpellObj.GetComponent<Spell>().ParticleEffects[i].CollisionParticles[j] = Spells[ID].ParticleEffects[i].CollisionParticles[j];
					}
				}
				SpellObj.GetComponent<Spell>().ParticleEffects[i].PrefabDestroyTimer = Spells[ID].ParticleEffects[i].PrefabDestroyTimer;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].LaunchPosition = Spells[ID].ParticleEffects[i].LaunchPosition;

				if(EnableTargetingEnemies == false)
				{
					if(Spells[ID].ParticleEffects[i].Move == true) SpellObj.GetComponent<Spell>().ParticleEffects[i].MvtVector = Spells[ID].ParticleEffects[i].FrontPosition.position - Spells[ID].ParticleEffects[i].LaunchPosition.position;
				}
				else
				{
					if(Spells[ID].ParticleEffects[i].Move == true) SpellObj.GetComponent<Spell>().ParticleEffects[i].MvtVector = SelectedEnemy.transform.position - Spells[ID].ParticleEffects[i].LaunchPosition.position;
					
				}
				SpellObj.GetComponent<Spell>().ParticleEffects[i].Speed = Spells[ID].ParticleEffects[i].Speed;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].Move = Spells[ID].ParticleEffects[i].Move;
				
				SpellObj.GetComponent<Spell>().ParticleEffects[i].Follow = Spells[ID].ParticleEffects[i].Follow;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].ObjToFollow = Spells[ID].ParticleEffects[i].ObjToFollow;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].SmoothTime = Spells[ID].ParticleEffects[i].SmoothTime;
				SpellObj.GetComponent<Spell>().ParticleEffects[i].Velocity = Spells[ID].ParticleEffects[i].Velocity;
			}
		}
		
		//Spell audio effects:
		SpellObj.GetComponent<Spell>().AudioEffects = new Spell.SpellAudioVars[Spells[ID].AudioEffects.Length];
		if(SpellObj.GetComponent<Spell>().AudioEffects.Length > 0)
		{
			for(int i = 0; i < Spells[ID].AudioEffects.Length; i++)
			{
				SpellObj.GetComponent<Spell>().AudioEffects[i] = new Spell.SpellAudioVars();
				
				SpellObj.GetComponent<Spell>().AudioEffects[i].Duration = Spells[ID].AudioEffects[i].Duration;
				SpellObj.GetComponent<Spell>().AudioEffects[i].Delay = Spells[ID].AudioEffects[i].Delay;
				
				SpellObj.GetComponent<Spell>().AudioEffects[i].Clip = Spells[ID].AudioEffects[i].Clip;
				SpellObj.GetComponent<Spell>().AudioEffects[i].Loop = Spells[ID].AudioEffects[i].Loop;
			}
		}
		
		//Animation effects:
		SpellObj.GetComponent<Spell>().AnimationEffects = new Spell.AnimationVars[Spells[ID].AnimationEffects.Length];
		if(SpellObj.GetComponent<Spell>().AnimationEffects.Length > 0)
		{
			for(int i = 0; i < Spells[ID].AnimationEffects.Length; i++)
			{
				SpellObj.GetComponent<Spell>().AnimationEffects[i] = new Spell.AnimationVars();
				
				SpellObj.GetComponent<Spell>().AnimationEffects[i].Duration = Spells[ID].AnimationEffects[i].Duration;
				SpellObj.GetComponent<Spell>().AnimationEffects[i].Delay = Spells[ID].AnimationEffects[i].Delay;
				
				SpellObj.GetComponent<Spell>().AnimationEffects[i].Clip = Spells[ID].AnimationEffects[i].Clip;
			}
		}
	}
	
	//Spell book UI:
	
	public void  RefreshSpellBook (){
		for(int i = 0; i < Spells.Length; i++)
		{
			if(Spells[i].SpellImage != null) //If spell book activation is enabled for this spell:
			{
				if(Spells[i].UnlockedByDefault == true)
				{
					Spells[i].Locked = false;
				}
				else
				{
					Spells[i].Locked = true;
					if(Spells[i].XPToUnlock > 0)
					{
						if(Spells[i].XPToUnlock <= XPManager.TotalXP)
						{
							Spells[i].Locked = false;
						}
					} 
					if(Spells[i].LevelToUnlock > 0)
					{
						if(Spells[i].LevelToUnlock <= XPManager.Level)
						{
							Spells[i].Locked = false;
						}
					} 
				}
				
				if(Spells[i].SpellImage.gameObject.GetComponent<SpellSlot>().ID == -1)
				{
					Spells[i].SpellImage.gameObject.GetComponent<SpellSlot>().ID = i; //Set spell ID.
				}
				
				//Refresh spells info in the spell book:
				Spells[i].SpellImage.sprite = Spells[i].SpellSprite;
				if(Spells[i].Locked == false)
				{
					Spells[i].SpellDescriptionText.text = Spells[i].Name+": "+Spells[i].Description;
					Spells[i].SpellImage.color = Color.white;
				} 
				else
				{
					Spells[i].SpellImage.color = Color.grey;
					if(Spells[i].XPToUnlock > 0)
					{
						Spells[i].SpellDescriptionText.text = Spells[i].Name+" (Unlocked at "+Spells[i].XPToUnlock.ToString()+" XP) : "+Spells[i].Description;
					} 
					if(Spells[i].LevelToUnlock > 0)
					{
						Spells[i].SpellDescriptionText.text = Spells[i].Name+" (Unlocked at level "+Spells[i].LevelToUnlock.ToString()+") : "+Spells[i].Description;
					} 
				}
			}
		}
	}
	
	public void  ToggleSpellBook (){
		SpellBook.ShowSpellBook = !SpellBook.ShowSpellBook; //Hide/Show the inventory.
		
		if(SpellBook.ShowSpellBook == false)
		{
			SpellBook.SpellBookTrans.gameObject.SetActive(false);
		} 
		else
		{
			SpellBook.SpellBookTrans.gameObject.SetActive(true);
		}   
	}
	
	//Dragging and dropping inventory window:
	public void  DragStarted (){
		if(SpellBook.IsMovable == true && SpellBook.Dragging == false)
		{
			SpellBook.Dragging = true;
			SpellBook.PanelDragPos.gameObject.SetActive(true);
			Vector3 TempPos = Input.mousePosition - SpellBook.UICanvasTrans.localPosition;
			SpellBook.PanelDragPos.GetComponent<RectTransform>().localPosition = new Vector3(TempPos.x-SpellBook.PanelDragPos.GetComponent<RectTransform>().rect.width/2,TempPos.y-SpellBook.PanelDragPos.GetComponent<RectTransform>().rect.height/2,0);
			SpellBook.SpellBookTrans.gameObject.transform.SetParent(SpellBook.PanelDragPos.transform, true);
		}
	}
	
	public void  DragEnded (){
		if(SpellBook.IsMovable == true)
		{
			SpellBook.Dragging = false;
			SpellBook.PanelDragPos.gameObject.SetActive(false);
			SpellBook.SpellBookTrans.gameObject.transform.SetParent(SpellBook.UICanvasTrans.gameObject.transform, true);
		}
	}
	
}