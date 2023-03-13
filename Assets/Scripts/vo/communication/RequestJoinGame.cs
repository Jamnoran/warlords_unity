using System;
using LitJson;

[System.Serializable]
public class RequestJoinGame {
	public String request_type;
	public int hero_id = 0;
    public string game_id;

    public RequestJoinGame(int hId, string gameId) {
        hero_id = hId;
        game_id = gameId;
		setRequestType("JOIN_GAME");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}