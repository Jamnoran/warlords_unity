using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class RequestGameStatus {
	public string response_type = "GAME_STATUS";
    public List<Minion> minions;
    private List<Champion> champions;
    private List<GameAnimation> animations;

    public RequestGameStatus (){}

    
}
