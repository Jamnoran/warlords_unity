using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseHeroBuff
{
    public string response_type = "HERO_BUFF";
    public int heroId;
    public int minionId;
    public int type;
    public float value;
    public int durationMillis;
    public long millisBuffStarted;

    public ResponseHeroBuff() { }
    
    public string ToString() {
        return "HeroBuffResponse{" +
                "response_type='" + response_type + '\'' +
                ", heroId=" + heroId +
                ", minionId=" + minionId +
                ", type=" + type +
                ", value=" + value +
                ", durationMillis=" + durationMillis +
                '}';
    }
}
