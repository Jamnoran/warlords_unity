using DunGen;
using DunGen.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DunGenerator : MonoBehaviour {
    public RuntimeDungeon DungeonGenerator;

    public DungeonFlow crawlerFlow;
    public GameObject hordeLevel;
    public GameObject gauntletLevel;
    private GameObject currentLevel;

    private void Start() {
        DungeonGenerator = GetComponentInChildren<RuntimeDungeon>();
        DungeonGenerator.Generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
        DungeonGenerator.Generator.Seed = 123456;
        DungeonGenerator.Generate();
    }

    private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status) {
        if (status != GenerationStatus.Complete)
        {
            //Debug.Log("World is generated");
            return;
        }
    }

    public void GenerateRandom(int seed, int typeOfLevel) {
        Debug.Log("Type of level : " + typeOfLevel);
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        if (typeOfLevel == 1)
        {
            DungeonGenerator.Generator.Seed = seed;
            DungeonGenerator.Generate();
        }
        else if (typeOfLevel == 2)
        {
            DungeonGenerator.Clear();
            currentLevel = Instantiate(hordeLevel, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (typeOfLevel == 3)
        {
            DungeonGenerator.Clear();
            currentLevel = Instantiate(gauntletLevel, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    private void Update() {

    }
    
}
