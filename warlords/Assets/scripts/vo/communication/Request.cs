using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[System.Serializable]
public class Request{
	public string request_type = "JOIN_SERVER";
	public string request_name = null;
	public int status_code = 200;
	public string joined_as = null;
	public string clients = null;
	public string data = null;


	public Request(){}

}
