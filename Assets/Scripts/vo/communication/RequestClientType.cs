using System;
using LitJson;

[System.Serializable]
public class RequestClientType {
	public String request_type;
	public int type = 0;

	public RequestClientType(int clientType) {
		type = clientType;
		setRequestType("CLIENT_TYPE_RESPONSE");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}

	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}
}