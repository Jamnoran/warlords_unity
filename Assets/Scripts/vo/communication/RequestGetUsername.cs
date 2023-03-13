using System;
using LitJson;

[System.Serializable]
public class RequestGetUsername : Request
{

	public RequestGetUsername(int uId) {
        request_type = "GET_USERNAME";
        user_id = "" + uId;
    }
}