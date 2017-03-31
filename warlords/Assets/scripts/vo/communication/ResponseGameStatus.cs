using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseGameStatus {
	public string response_type = "GAME_STATUS";
    public List<Minion> minions;
    public List<Hero> heroes;
    public List<GameAnimation> gameAnimations;

    public ResponseGameStatus(){}


}
