using System;
using LitJson;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class RequestUpdateMinionPosition {
	public String request_type;
	public int hero_id = 0;
	public List<Minion> minions;

	public RequestUpdateMinionPosition(int hId, List<Minion> updatedMinions) {
        hero_id = hId;
		setRequestType("UPDATE_MINION_POSITION");
		minions = updatedMinions;
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}