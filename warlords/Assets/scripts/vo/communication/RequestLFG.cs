using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;


[System.Serializable]
public class RequestLFG {
    public string request_type = "START_GAME";
    public LFG lfg;

    public RequestLFG() { }
    
}
