using UnityEngine;
using UnityEditor;

[System.Serializable]
public class RequestUpdateUsername
{
    public string request_type;
    public string username;
    public int user_id = 0;

    public RequestUpdateUsername(string uname, int uId)
    {
        username = uname;
        user_id = uId;
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

    public string toString()
    {
        return "UpdateUsernameRequest{" +
                "username='" + username + '\'' +
                '}';
    }
}