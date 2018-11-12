using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpell : MonoBehaviour {
    public GameObject gameobjectToActivate;
    public float spawnTime = 0.50f;
    // Use this for initialization
    void Start()
    {
        StartCoroutine("setActive");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator setActive()
    {
        yield return new WaitForSeconds(spawnTime);
        gameobjectToActivate.SetActive(true);
    }
}
