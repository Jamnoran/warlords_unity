using System;
using LitJson;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RequestMove {
	public String request_type;
	public int hero_id = 0;
    public float positionX;
    public float postionY;
    public float positionZ;
    public float desiredPositionX;
    public float desiredPositionY;
    public float desiredPositionZ;

    public RequestMove(int hId, float positionX, float positionY, float positionZ, float desiredPositionX, float desiredPositionY, float desiredPositionZ)
    {
        setRequestType("MOVE");
        hero_id = hId;
        this.positionX = positionX;
        this.postionY = positionY;
        this.positionZ = positionZ;
        this.desiredPositionX = desiredPositionX;
        this.desiredPositionY = desiredPositionY;
        this.desiredPositionZ = desiredPositionZ;
}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}