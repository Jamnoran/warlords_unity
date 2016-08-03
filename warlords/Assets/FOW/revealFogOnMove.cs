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
        if (((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero() != null && ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero().transform != null) {
            TerrainFoW.Current.PaintTerrain(((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero().transform.position, TerrainFoW.Current.ExplorationSize, CurrentColor);
        }
         
	}

    public Transform getHero()
    {
        return hero;
    }
    public void setHero(Transform newHero)
    {
        hero = newHero;
    }
}

