using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseGetHeroes
{
    public string response_type = "GAME_STATUS";
    public List<Hero> heroes;

    public ResponseGetHeroes() { }
}
