using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseStopHero
{
    public string response_type = "STOP_HERO";
    public int hero;

    public ResponseStopHero() { }
}
