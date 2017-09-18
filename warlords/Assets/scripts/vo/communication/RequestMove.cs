using System;
using LitJson;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RequestMove {
	public String request_type;
	public int hero_id = 0;
    public float position_x;
    public float position_y;
    public float position_z;
    public float desired_position_x;
    public float desired_position_y;
    public float desired_position_z;

    public RequestMove(int hId, float positionX, float positionY, float positionZ, float desiredPositionX, float desiredPositionY, float desiredPositionZ)
    {
        setRequestType("MOVE");
        hero_id = hId;
        this.position_x = positionX;
        this.position_y = positionY;
        this.position_z = positionZ;
        this.desired_position_x = desiredPositionX;
        this.desired_position_y = desiredPositionY;
        this.desired_position_z = desiredPositionZ;
}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}