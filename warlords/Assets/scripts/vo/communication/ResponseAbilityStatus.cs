using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseAbilityStatus {
	public string response_type = "ABILITY_STATUS";
    public Ability ability;

    public ResponseAbilityStatus(){}


}
