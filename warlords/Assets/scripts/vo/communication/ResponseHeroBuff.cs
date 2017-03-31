using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseHeroBuff
{
    private string responseType = "HERO_BUFF";
    public int heroId;
    public int minionId;
    public int type;
    public float value;
    public long durationMillis;

    public ResponseHeroBuff() { }
}
