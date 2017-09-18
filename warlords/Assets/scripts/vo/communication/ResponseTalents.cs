using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;


[System.Serializable]
public class ResponseTalents {
    public string response_type = "TALENTS";
    public List<Talent> talents;
    public int totalTalentPoints = 0;

    public ResponseTalents() { }

}
