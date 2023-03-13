using System;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using UnityEngine;

[System.Serializable]
public class RequestLogin : Request {
	public String email;
	public String password;

	public RequestLogin(String pEmail, String pPassword) {
		email = pEmail;
		password = pPassword;
		setRequestType("LOGIN_USER");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}

    public override void calculateSign()
    {
        sign = Assets.scripts.util.AuthenticationUtil.getCalcuatedSign(JsonMapper.ToJson(this));
    }
}