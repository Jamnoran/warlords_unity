using System;
using LitJson;
using Assets.scripts.vo;

[System.Serializable]
public class RequestSendMessage {
	public String request_type;
	public Message message;

	public RequestSendMessage(Message message) {
        this.message = message;
		setRequestType("SEND_MESSAGE");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}