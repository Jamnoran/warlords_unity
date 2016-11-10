using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseAbilities {
	public string response_type = "ABILITIES";
    public List<Ability> abilities;

    public ResponseAbilities(){}


}
