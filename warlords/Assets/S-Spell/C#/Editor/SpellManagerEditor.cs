using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(SpellManager))] 
public class SpellManagerEditor : Editor {
	
	public SerializedProperty SpellsArray;

	public bool PEOpen = false;
	public bool AudioOpen = false;
	public bool AnimOpen = false;

	public int SpellID = 0;
	public Rect MyRect;
	public bool showChildren;
	
	//Quest custom editor: makes creating quests easy and simple.
	public override void OnInspectorGUI() 
	{
		EditorGUILayout.LabelField("Spell Manager:");

		SpellManager MyTarget = (SpellManager) target;
		
		EditorGUIUtility.LookLikeInspector();
		EditorGUIUtility.LookLikeControls();
		EditorGUILayout.Space();

		MyTarget.Player = EditorGUILayout.ObjectField ("Player Object:", MyTarget.Player, typeof(GameObject)) as GameObject;
		MyTarget.AnimController = EditorGUILayout.ObjectField ("Animation Controller:", MyTarget.AnimController, typeof(Animation)) as Animation;
		MyTarget.AudioSourceObj = EditorGUILayout.ObjectField ("Audio Source", MyTarget.AudioSourceObj, typeof(GameObject)) as GameObject;
		MyTarget.SpellCastReload = EditorGUILayout.FloatField ("Casting Spells Reload Time (seconds): ", MyTarget.SpellCastReload);
		EditorGUILayout.LabelField ("If enabled, this will require the player to select an enemy before casting a spell that can do damage.");
		MyTarget.EnableTargetingEnemies = EditorGUILayout.Toggle("Enable Targeting Enemies:", MyTarget.EnableTargetingEnemies);

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Spells:");

		EditorGUILayout.Space();

		SpellsArray = serializedObject.FindProperty("Spells");

		
		SpellsArray.arraySize = EditorGUILayout.IntField("Spells Count:", SpellsArray.arraySize);
		serializedObject.ApplyModifiedProperties();
		if(SpellsArray.arraySize < 1) SpellsArray.arraySize = 1; serializedObject.ApplyModifiedProperties();

		EditorGUILayout.Space();

		if (SpellID + 1 > SpellsArray.arraySize)
		{
			SpellID = SpellsArray.arraySize - 1;
		}

		EditorGUILayout.LabelField("Navigate between spells:");

		if(GUILayout.Button("<<") && SpellID > 0)
		{
			SpellID--;
		}

		if(GUILayout.Button(">>") && SpellID+1 < SpellsArray.arraySize)
		{
			SpellID++;
		}

		
		EditorGUILayout.LabelField("Spell ID: " + SpellID.ToString());

		EditorGUILayout.Space();
		
		if(GUILayout.Button("Remove Spell"))
		{
			if(SpellsArray.arraySize > 1)
			{
				if(SpellID < SpellsArray.arraySize-1)
				{
					for(int i = SpellID; i < SpellsArray.arraySize-1; i++)
					{
						MyTarget.Spells[i] = MyTarget.Spells[i+1]; 
					}
				}
				
				SpellsArray.arraySize = SpellsArray.arraySize-1; serializedObject.ApplyModifiedProperties();
			}
		}
		
		EditorGUILayout.Space();

		MyTarget.Spells[SpellID].Name = EditorGUILayout.TextField("Name: ", MyTarget.Spells[SpellID].Name); 
		MyTarget.Spells[SpellID].Description = EditorGUILayout.TextField("Description: ", MyTarget.Spells[SpellID].Description); 
		MyTarget.Spells[SpellID].SpellSprite = EditorGUILayout.ObjectField("Icon: ", MyTarget.Spells[SpellID].SpellSprite, typeof(Sprite)) as Sprite;
		MyTarget.Spells[SpellID].ManaNeeded = EditorGUILayout.IntField("Mana Points Needed: ", MyTarget.Spells[SpellID].ManaNeeded); 
		MyTarget.Spells[SpellID].CastDelay = EditorGUILayout.FloatField("Spell Cast Delay: ", MyTarget.Spells[SpellID].CastDelay);
		MyTarget.Spells[SpellID].Recharge = EditorGUILayout.FloatField("Recharge Time (seconds): ", MyTarget.Spells[SpellID].Recharge);
		MyTarget.Spells[SpellID].RequireEnemy = EditorGUILayout.Toggle("Target enemy before using spell?", MyTarget.Spells[SpellID].RequireEnemy);
	
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Trigger Key, Particle Effects, Audio Effects & Animation Effects:");
		serializedObject.Update();
		EditorGUILayout.PropertyField(SpellsArray.GetArrayElementAtIndex(SpellID),GUIContent.none, true);
		serializedObject.ApplyModifiedProperties();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("UI:");
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Spell Image: will display the spell's icon in the spell book.");
		MyTarget.Spells[SpellID].SpellImage = EditorGUILayout.ObjectField("UI Image:", MyTarget.Spells[SpellID].SpellImage, typeof(Image)) as Image;
		EditorGUILayout.LabelField("Spell Description Text: will display the spell's description in the spell book.");
		MyTarget.Spells[SpellID].SpellImage = EditorGUILayout.ObjectField("UI Textfield:", MyTarget.Spells[SpellID].SpellImage, typeof(Image)) as Image;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Spell Status:");
		EditorGUILayout.Space();
		MyTarget.Spells[SpellID].UnlockedByDefault = EditorGUILayout.Toggle("Spell Unlocked by Default:", MyTarget.Spells[SpellID].UnlockedByDefault);
		MyTarget.Spells[SpellID].XPToUnlock = EditorGUILayout.IntField("XP To Unlock: ", MyTarget.Spells[SpellID].XPToUnlock); 
		MyTarget.Spells[SpellID].LevelToUnlock = EditorGUILayout.IntField("Level To Unlock: ", MyTarget.Spells[SpellID].LevelToUnlock); 

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("SpellBook"), true);
		serializedObject.ApplyModifiedProperties();
	}
}