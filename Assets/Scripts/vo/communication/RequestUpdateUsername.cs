using UnityEngine;
using UnityEditor;
using LitJson;

[System.Serializable]
public class RequestUpdateUsername : Request
{
    public string username;

    public RequestUpdateUsername(string uname, string userId)
    {
        username = uname;
        user_id = userId;
        setRequestType("UPDATE_USERNAME");
    }

    public void setRequestType(string requestType)
    {
        this.request_type = requestType;
    }

    public string getRequestType()
    {
        return request_type;
    }
    public string getUsername()
    {
        return username;
    }

    public void setUsername(string username)
    {
        this.username = username;
    }

    public override void calculateSign()
    {
        sign = Assets.scripts.util.AuthenticationUtil.getCalcuatedSign(JsonMapper.ToJson(this));
    }

    public string toString()
    {
        return "UpdateUsernameRequest{" +
                "username='" + username + '\'' +
                '}';
    }
}