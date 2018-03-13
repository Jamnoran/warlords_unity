using UnityEngine;
using UnityEditor;

[System.Serializable]
public class RequestUpdateUsername
{
    public string username;

    public RequestUpdateUsername(string uname)
    {
        username = uname;
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