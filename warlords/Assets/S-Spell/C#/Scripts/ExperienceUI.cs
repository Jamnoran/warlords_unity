// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExperienceUI : MonoBehaviour {
	
	public RectTransform FullBar;
	public RectTransform Empty;
	public Text LevelText;
	
	[HideInInspector]
	public float BarWidth = 0;
	[HideInInspector]
	public float PosX = 0;
	[HideInInspector]
	public ExperienceManager XPManager;
	
	public void  SetXPBarUI (){
		if(XPManager == null)
		{
			XPManager = FindObjectOfType(typeof(ExperienceManager)) as ExperienceManager;
			
			BarWidth = FullBar.rect.width;
			PosX = FullBar.localPosition.x;
		}
		FullBar.sizeDelta = new Vector2 ((XPManager.XP/(XPManager.Level*XPManager.Level1XP)) * BarWidth,FullBar.sizeDelta.y);
		FullBar.localPosition = new Vector2 (-(BarWidth - FullBar.rect.width) / 2 + PosX, FullBar.localPosition.y);
		
		LevelText.text = "Level: " + XPManager.Level.ToString();
	}
	
}