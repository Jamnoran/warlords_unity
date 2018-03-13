using System;
using LitJson;

[System.Serializable]
public class RequestGetUsername {
	public String request_type;
	public int user_id = 0;

	public RequestGetUsername(int uId) {
		user_id = uId;
		setRequestType("GET_USERNAME");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}