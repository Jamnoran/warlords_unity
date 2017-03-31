using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseTeleportHeroes
{
    public string response_type = "TELEPORT_HEROES";
    public List<Hero> heroes;

    public ResponseTeleportHeroes() { }
}
