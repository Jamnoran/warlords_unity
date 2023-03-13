using System;
using LitJson;

[System.Serializable]
public class RequestCreateHero {
	public String request_type;
	public String class_type;
	public int user_id = 0;

	public RequestCreateHero(int uId, String classType) {
        user_id = uId;
		class_type = classType;
		setRequestType("CREATE_HERO");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}

	public int getUserId(){
		return user_id;
	}

	public void setUserId(int uId){
        user_id = uId;
	}

}