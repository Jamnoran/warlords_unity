using System;
using LitJson;

[System.Serializable]
public class RequestCreateUser {
	public String request_type;
	public String email;
	public String password;
	public String username;

	public RequestCreateUser(String pEmail, String pPassword, String pUsername) {
		email = pEmail;
		password = pPassword;
		username = pUsername;
		setRequestType("CREATE_USER");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}
}