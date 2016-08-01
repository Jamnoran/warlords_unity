/*--------------------------------------------------------------*/
//Terrain FoW
//Created by Rafael Batista
//Control the Fog of War
/*--------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System;

public enum Cor
{
	BLUE,
	GREEN,
	RED,
	WHITE
}

public class TreeAux
{
	public int Index;
	public int Prototype;
}

public class TerrainFoW : MonoBehaviour
{
	//The exploration size to units
	public static TerrainFoW Current;
	public float ExplorationSize = 100.0f;
	public string ExploreTag1, ExploreTag2, ExploreTag3, ExploreTag4;
	private List<TreeInstance> treesInstance = new List<TreeInstance> ();
	private List<TreeInstance> treeDefault = new List<TreeInstance> ();
	private List<TreePrototype> treePrototypes = new List<TreePrototype> ();
	private List<TreeAux> treeAux = new List<TreeAux> ();
	private List<SplatPrototype> splats = null;
	private Terrain terr;
	protected int hmWidth;
	protected int hmHeight;
	protected int alphaMapWidth;
	protected int alphaMapHeight;
	protected int numOfAlphaLayers;
	protected const float DEPTH_METER_CONVERT = 0.05f;
	protected const float TEXTURE_SIZE_MULTIPLIER = 1.25f;
	private float[,] heightMapBackup;
	private float[, ,] alphaMapBackup;
	float[, ,] alphasBkp = null;
	private List<int[,]> detailBkp = null;
	private List<int[,]> detailAux = null;
	//Restores the terrain
	void OnApplicationQuit ()
	{
		if (Debug.isDebugBuild) {
			if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
				Terrain.activeTerrain.terrainData.treeInstances = treeDefault.ToArray ();
				Terrain.activeTerrain.terrainData.treePrototypes = treePrototypes.ToArray ();
			}
			if (terr != null && terr.terrainData != null) {
				terr.terrainData.SetHeights (0, 0, heightMapBackup);
				terr.terrainData.SetAlphamaps (0, 0, alphaMapBackup);
				Terrain.activeTerrain.terrainData.splatPrototypes = splats.ToArray ();
			}
		}
	}

    public GameObject hero;
    Vector3 testVector = new Vector3(0,0,0);
	//Hide the trees
	void Start ()
	{
		Current = this;
		detailBkp = new List<int[,]> ();
		detailAux = new List<int[,]> ();
		//TerrainData------------------------------
		terr = this.GetComponent<Terrain> ();
		if (terr != null && terr.terrainData != null) {
			hmWidth = terr.terrainData.heightmapWidth;
			hmHeight = terr.terrainData.heightmapHeight;
			alphaMapWidth = terr.terrainData.alphamapWidth;
			alphaMapHeight = terr.terrainData.alphamapHeight;
			numOfAlphaLayers = terr.terrainData.alphamapLayers;
			splats = terr.terrainData.splatPrototypes.Where (n => n != null && n.texture != null).ToList ();			
			int splatLenth = splats.Count + 4;			
			numOfAlphaLayers = splatLenth;			
			SplatPrototype[] test = new SplatPrototype[splatLenth];			
			for (int i = 0; i < splatLenth; i++) {				
				if (i < splats.Count) {
					test [i] = terr.terrainData.splatPrototypes [i];	
				} else {
					test [i] = new SplatPrototype ();
					
					if (i == splats.Count)
						test [i].texture = (Texture2D)Resources.Load ("tblue", typeof(Texture2D));
					else if (i == splats.Count + 1)
						test [i].texture = (Texture2D)Resources.Load ("tgreen", typeof(Texture2D));
					else if (i == splats.Count + 2) 
						test [i].texture = (Texture2D)Resources.Load ("tred", typeof(Texture2D));
					else if (i == splats.Count + 3)
						test [i].texture = (Texture2D)Resources.Load ("twhite", typeof(Texture2D));
					
					
					test [i].tileOffset = new Vector2 (0, 0); 
					test [i].tileSize = new Vector2 (15, 15);
				}
			}
			terr.terrainData.splatPrototypes = test;
			if (Debug.isDebugBuild) {
				heightMapBackup = terr.terrainData.GetHeights (0, 0, hmWidth, hmHeight);
				alphaMapBackup = terr.terrainData.GetAlphamaps (0, 0, alphaMapWidth, alphaMapHeight);   
			} 
			BlackTerrain ();			
		}
		//--------------------------------		
		if (Debug.isDebugBuild) {
			if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
				treeDefault = Terrain.activeTerrain.terrainData.treeInstances.ToList ();
				treePrototypes = Terrain.activeTerrain.terrainData.treePrototypes.ToList ();
				int nLayers = Terrain.activeTerrain.terrainData.detailPrototypes.Length;			
				for (int i = 0; i < nLayers; i++) {
					detailBkp.Add (Terrain.activeTerrain.terrainData.GetDetailLayer (0, 0, terr.terrainData.detailWidth, terr.terrainData.detailHeight, i));
				}	
			}
		}
		if (Application.isPlaying) {
			try {
				List<GameObject> gos = new List<GameObject> ();
				if (!string.IsNullOrEmpty (ExploreTag1))
					gos.AddRange (GameObject.FindGameObjectsWithTag (ExploreTag1));
				if (!string.IsNullOrEmpty (ExploreTag2))
					gos.AddRange (GameObject.FindGameObjectsWithTag (ExploreTag2));
				if (!string.IsNullOrEmpty (ExploreTag3))
					gos.AddRange (GameObject.FindGameObjectsWithTag (ExploreTag3));
				if (!string.IsNullOrEmpty (ExploreTag4))
					gos.AddRange (GameObject.FindGameObjectsWithTag (ExploreTag4));
		
				foreach (GameObject item in gos)
					item.GetComponent<Renderer>().enabled = false;
			} catch (Exception ex) {
				Debug.Log (ex.Message);
			}			
			if (Terrain.activeTerrain != null && Terrain.activeTerrain.terrainData != null) {
				int nLayers = Terrain.activeTerrain.terrainData.detailPrototypes.Length;
				int[,] hds = new int[terr.terrainData.detailWidth, terr.terrainData.detailHeight];
				for (int i = 0; i < nLayers; i++) {			
					detailAux.Add (terr.terrainData.GetDetailLayer (0, 0, terr.terrainData.detailWidth, terr.terrainData.detailHeight, i));
				}				
				for (int y = 0; y < terr.terrainData.detailWidth; y++)
					for (int x = 0; x < terr.terrainData.detailHeight; x++) {
						hds [x, y] = 0;
					}			
				for (int i = 0; i < nLayers; i++) {
					Terrain.activeTerrain.terrainData.SetDetailLayer (0, 0, i, hds);
				}
				List<TreePrototype> trees = new List<TreePrototype> (Terrain.activeTerrain.terrainData.treePrototypes);
				List<TreePrototype> prototypes = new List<TreePrototype> ();		
				TreePrototype tp = new TreePrototype ();
				tp.prefab = (GameObject)Resources.Load ("EmptyTree");
				prototypes.Add (tp);	
				foreach (TreePrototype item in trees) {			
					prototypes.Add (item);			
				}				
				Terrain.activeTerrain.terrainData.treePrototypes = prototypes.Where (n => n != null).ToArray ();
				List<TreeInstance> treesIns = new List<TreeInstance> (Terrain.activeTerrain.terrainData.treeInstances);

				treesInstance = new List<TreeInstance> (treesIns);
				for (int i = 0; i < treesIns.Count; i++) {
					TreeInstance ti = treesIns [i];
					TreeAux ta = new TreeAux ();
					ta.Index = i;
					ta.Prototype = ti.prototypeIndex + 1;	
					treeAux.Add (ta);
				}
				try {
					List<TreeInstance> tis = new List<TreeInstance> ();
					for (int i = 0; i < Terrain.activeTerrain.terrainData.treeInstances.Length; i++) {
						TreeInstance tiii = Terrain.activeTerrain.terrainData.treeInstances [i];
						tiii.prototypeIndex = 0;
						tis.Add (tiii);
					}
					Terrain.activeTerrain.terrainData.treeInstances = tis.ToArray ();
				} catch (Exception ex) {
					Debug.Log (ex.Message);
				}
			}
		}
	}
	//Convert to terrain position
	public Vector3 ConvertPosition (Vector3 position)
	{
		RaycastHit clickPosition;
		float offset = Vector3.Distance (TerrainFoW.Current.transform.position, Camera.main.transform.position);
        //if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out clickPosition, offset)) {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(hero.transform.position), out clickPosition))
        {
            //return clickPosition.point;
            return hero.transform.position;
		}
        return hero.transform.position;
    }
	//Explore an area
	public void ExploreArea (Vector3 position, float exploreSize)
	{
		Vector3 pos = ConvertPosition (position);		
		WhitenTerrain (pos, ExplorationSize);		
		try {
			List<GameObject> gos = new List<GameObject> ();
			if (!string.IsNullOrEmpty (ExploreTag1))
				gos.AddRange (GameObject.FindGameObjectsWithTag (ExploreTag1));
			if (!string.IsNullOrEmpty (ExploreTag2))
				gos.AddRange (GameObject.FindGameObjectsWithTag (ExploreTag2));
			if (!string.IsNullOrEmpty (ExploreTag3))
				gos.AddRange (GameObject.FindGameObjectsWithTag (ExploreTag3));
			if (!string.IsNullOrEmpty (ExploreTag4))
				gos.AddRange (GameObject.FindGameObjectsWithTag (ExploreTag4));
		
			foreach (GameObject item in gos) {
				float aux = Vector3.Distance (item.transform.position, pos);						
				if (aux < (ExplorationSize / 2)) {			
					item.GetComponent<Renderer>().enabled = true;
				}
			}
		} catch (Exception ex) {
			Debug.Log (ex.Message);
		}		
		List<TreeInstance> treesAux = new List<TreeInstance> ();
		for (int i = 0; i < treesInstance.Count; i++) {
			TreeInstance ti = treesInstance [i];	
			TreeInstance currentTree = ti;
			Vector3 currentTreeWorldPosition = Vector3.Scale (currentTree.position, Terrain.activeTerrain.terrainData.size) + Terrain.activeTerrain.transform.position;
			float aux = Vector3.Distance (currentTreeWorldPosition, pos);						
			if (aux < (ExplorationSize / 2)) {
				TreeAux ta = treeAux.FirstOrDefault (n => n.Index == i);
				if (ta != null) {
					ti.prototypeIndex = ta.Prototype;
				}
			}
			treesAux.Add (ti);
		}
		Terrain.activeTerrain.terrainData.treeInstances = treesAux.ToArray ();				
		treesInstance = treesAux;
	}	
	//Darkens the terrain
	protected void BlackTerrain ()
	{
		float[, ,] alphas = terr.terrainData.GetAlphamaps (0, 0, terr.terrainData.alphamapResolution, terr.terrainData.heightmapResolution - 1);
		alphasBkp = new float[terr.terrainData.alphamapResolution, terr.terrainData.heightmapResolution, numOfAlphaLayers];		
		for (int i = 0; i < terr.terrainData.alphamapResolution; i++) {
			for (int j = 0; j < terr.terrainData.heightmapResolution - 1; j++) {
				for (int layerCount = 0; layerCount < numOfAlphaLayers; layerCount++) {
					try {			
						alphasBkp [i, j, layerCount] = alphas [i, j, layerCount];
						alphas [i, j, layerCount] = -0.1f;
					} catch (Exception ex) {
						//Empty Catch not to stop the processing of other positions in the array.
					}	
				}
			}			
		}
		terr.terrainData.SetAlphamaps (0, 0, alphas);
	}
	//Lightens the terrain
	private void WhitenTerrain (Vector3 pos, float whiteArea)
	{		
		Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPosition (pos, terr, alphaMapWidth, alphaMapHeight);
		int alphaMapCraterWidth = (int)(whiteArea * (alphaMapWidth / terr.terrainData.size.x));
		int alphaMapCraterLength = (int)(whiteArea * (alphaMapHeight / terr.terrainData.size.z));
		int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - (alphaMapCraterWidth / 2));
		alphaMapStartPosX = alphaMapStartPosX > 0 ? alphaMapStartPosX : 0;
		int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - (alphaMapCraterLength / 2));
		alphaMapStartPosZ = alphaMapStartPosZ > 0 ? alphaMapStartPosZ : 0;
		int realWidth = alphaMapStartPosX + alphaMapCraterWidth <= terr.terrainData.alphamapWidth ? alphaMapCraterWidth : terr.terrainData.alphamapWidth - alphaMapStartPosX;
		int realHeight = alphaMapStartPosZ + alphaMapCraterLength <= terr.terrainData.alphamapHeight ? alphaMapCraterLength : terr.terrainData.alphamapHeight - alphaMapStartPosZ;		
		float[, ,] alphas = terr.terrainData.GetAlphamaps (alphaMapStartPosX, alphaMapStartPosZ, realWidth, realHeight);
		for (int i = 0; i < realHeight; i++) {
			for (int j = 0; j < realWidth; j++) {
				for (int layerCount = 0; layerCount < splats.Count; layerCount++) {
					try {	
						alphas [i, j, layerCount] = alphasBkp [alphaMapStartPosZ + i, alphaMapStartPosX + j, layerCount];
					} catch (Exception ex) {
						//Empty Catch not to stop the processing of other positions in the array.
					}
				}
			}
		}
		terr.terrainData.SetAlphamaps (alphaMapStartPosX, alphaMapStartPosZ, alphas);		
		int nLayers = Terrain.activeTerrain.terrainData.detailPrototypes.Length;		
		for (int i = 0; i < nLayers; i++) {		
			int[,] hds = new int[terr.terrainData.detailWidth, terr.terrainData.detailHeight];
			for (int y = 0; y < realHeight; y++)
				for (int x = 0; x < realWidth; x++) {
					try {
						hds [x, y] = detailAux [i] [alphaMapStartPosZ + x, alphaMapStartPosX + y];
					} catch {
						//Empty Catch not to stop the processing of other positions in the array.
					}
				}	
			Terrain.activeTerrain.terrainData.SetDetailLayer (alphaMapStartPosX, alphaMapStartPosZ, i, hds);			
		}		
	}
	//Paint the Terrain with an color
	public void PaintTerrain (Vector3 position, float whiteArea, Cor color)
	{
		Vector3 pos = ConvertPosition (position);
		Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPosition (pos, terr, alphaMapWidth, alphaMapHeight);
		int alphaMapCraterWidth = (int)(whiteArea * (alphaMapWidth / terr.terrainData.size.x));
		int alphaMapCraterLength = (int)(whiteArea * (alphaMapHeight / terr.terrainData.size.z));
		int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - (alphaMapCraterWidth / 2));
		alphaMapStartPosX = alphaMapStartPosX > 0 ? alphaMapStartPosX : 0;
		int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - (alphaMapCraterLength / 2));
		alphaMapStartPosZ = alphaMapStartPosZ > 0 ? alphaMapStartPosZ : 0;
		int realWidth = alphaMapStartPosX + alphaMapCraterWidth <= terr.terrainData.alphamapWidth ? alphaMapCraterWidth : terr.terrainData.alphamapWidth - alphaMapStartPosX;
		int realHeight = alphaMapStartPosZ + alphaMapCraterLength <= terr.terrainData.alphamapHeight ? alphaMapCraterLength : terr.terrainData.alphamapHeight - alphaMapStartPosZ;
					
		float[, ,] alphas = terr.terrainData.GetAlphamaps (alphaMapStartPosX, alphaMapStartPosZ, realWidth, realHeight);
		try {	
			for (int i = 0; i < realHeight; i++) {
				for (int j = 0; j < realWidth; j++) {
					switch (color) {
					case Cor.BLUE:
						alphas [i, j, splats.Count] = 0.35f;
						break;
					case Cor.GREEN:
						alphas [i, j, splats.Count + 1] = 0.35f;
						break;
					case Cor.RED:
						alphas [i, j, splats.Count + 2] = 0.35f;
						break;
					case Cor.WHITE:
						alphas [i, j, splats.Count + 3] = 0.35f;
						break;
					}
				}
			}
		} catch (Exception ex) {
			//Empty Catch not to stop the processing of other positions in the array.
		}
		terr.terrainData.SetAlphamaps (alphaMapStartPosX, alphaMapStartPosZ, alphas);
		
	}
	//Get normalized position
	protected Vector3 GetNormalizedPositionRelativeToTerrain (Vector3 pos, Terrain terrain)
	{
		Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
		Vector3 coord;
		coord.x = tempCoord.x / terr.terrainData.size.x;
		coord.y = tempCoord.y / terr.terrainData.size.y;
		coord.z = tempCoord.z / terr.terrainData.size.z;
		return coord;
	}
	//Get normalized position from a position
	protected Vector3 GetRelativeTerrainPositionFromPosition (Vector3 pos, Terrain terrain, int mapWidth, int mapHeight)
	{
		Vector3 coord = GetNormalizedPositionRelativeToTerrain (pos, terrain);
		return new Vector3 ((coord.x * mapWidth), 0, (coord.z * mapHeight));
	}
}
