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
    private GameObject currentHordeLevel;

    private void Start() {
        DungeonGenerator = GetComponentInChildren<RuntimeDungeon>();
        DungeonGenerator.Generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
    }

    private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status) {
        if (status != GenerationStatus.Complete)
            return;
    }

    public void GenerateRandom(int seed, int typeOfLevel) {
        Debug.Log("Type of level : " + typeOfLevel);
        if (currentHordeLevel != null)
        {
            Destroy(currentHordeLevel);
        }
        if (typeOfLevel == 1)
        {
            DungeonGenerator.Generator.Seed = seed;
            DungeonGenerator.Generate();
        }
        else if (typeOfLevel == 2)
        {
            DungeonGenerator.Clear();
            currentHordeLevel = Instantiate(hordeLevel, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    private void Update() {

    }
    
}
