using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[System.Serializable]
public class Request{
	public string request_type = "JOIN_SERVER";
    public string user_id;
    public string sign = null;

    public Request(){}


    public virtual void calculateSign()
    {
        Debug.Log("Base method calculateSign called");
    }

}
