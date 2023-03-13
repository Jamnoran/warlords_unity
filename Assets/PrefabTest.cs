using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTest : MonoBehaviour
{
    public GameObject prefabToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        GameObject warrior = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
       
        HeroInfo heroInfo = (HeroInfo)warrior.GetComponent(typeof(HeroInfo));
        Hero hero = new Hero();
        hero.id = 10;
        heroInfo.setHero(hero);
        heroInfo.setAlive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
