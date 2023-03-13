using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;


[System.Serializable]
public class RequestTalents {
    public string request_type;
    public int hero_id = 0;
    public List<Talent> talents;

    public RequestTalents(int hId, List<Talent> list) {
        request_type = "UPDATE_TALENTS";
        hero_id = hId;
        talents = list;
    }
    public RequestTalents() { }

}
