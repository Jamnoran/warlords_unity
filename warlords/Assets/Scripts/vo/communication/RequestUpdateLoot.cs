using System;
using LitJson;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class RequestUpdateLoot {
	public String request_type;
	public int hero_id = 0;
    public int item_id;
    public int new_position;
    public bool equipped;


    public RequestUpdateLoot(int hId, int itemId, int newPosition, bool equipped) {
        hero_id = hId;
		setRequestType("UPDATE_ITEM_POSITION");
        this.item_id = itemId;
        this.new_position = newPosition;
        this.equipped = equipped;
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}