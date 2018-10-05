using System;
using LitJson;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RequestSpell {
	public String request_type;
	public int hero_id = 0;
    public int spell_id;
    public List<int> target_enemy;
    public List<int> target_friendly;
    public float target_position_x;
    public float target_position_z;
    public float target_position_y;
    public long time;
    public bool initial_cast;

    public RequestSpell(int hId, int spellId, List<int> targetEnemy, List<int> targetFriendly, Vector3 vector3, long t, bool initialCast)
    {
        setRequestType("SPELL");
        hero_id = hId;
        spell_id = spellId;
        target_enemy = targetEnemy;
        target_friendly = targetFriendly;
        target_position_x = vector3.x;
        target_position_z = vector3.z;
        target_position_y = vector3.y;
        time = t;
        initial_cast = initialCast;
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}