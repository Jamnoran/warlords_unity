using System;
using LitJson;

[System.Serializable]
public class RequestGetHeroes {
	public String request_type;
	public String game_type;
	public int user_id = 0;

	public RequestGetHeroes(int uId) {
		user_id = uId;
		setRequestType("GET_HEROES");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}