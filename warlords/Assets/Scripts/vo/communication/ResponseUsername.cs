using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseUsername
{
    public string response_type = "USERNAME_REPONSE";
    public string username;

    public ResponseUsername() { }


    public string getUsername()
    {
        return username;
    }
}
