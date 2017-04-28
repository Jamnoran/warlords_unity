using System;
using LitJson;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class RequestSpawnPoints {
	public String request_type;
	public int hero_id = 0;
    public List<Point> points;

    public RequestSpawnPoints(int hId, List<Point> spawnPoints) {
        hero_id = hId;
		setRequestType("SPAWN_POINTS");
        points = spawnPoints;
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}