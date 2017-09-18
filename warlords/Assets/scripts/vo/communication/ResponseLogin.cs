using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;
using System;

[System.Serializable]
public class ResponseLogin
{
	public string response_type;
	public string code;
	public string message;
    public Int32 user_id;

	public ResponseLogin(){}
   
}
