using UnityEngine;
using System.Collections;

public class revealFogOnMove : MonoBehaviour
{
	public static bool OverGUI = false;
	public static Cor CurrentColor = Cor.RED;
    public Transform hero;
    Vector3 testVector = new Vector3(0,0,0);
	
	void Update ()
	{
		
				TerrainFoW.Current.PaintTerrain (hero.transform.position, TerrainFoW.Current.ExplorationSize, CurrentColor);
			
           
		}
	}

