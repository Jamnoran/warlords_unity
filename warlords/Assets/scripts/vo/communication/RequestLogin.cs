using System;
using LitJson;

[System.Serializable]
public class RequestLogin {
	public String request_type;
	public String email;
	public String password;

	public RequestLogin(String pEmail, String pPassword) {
		email = pEmail;
		password = pPassword;
		setRequestType("LOGIN_USER");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}