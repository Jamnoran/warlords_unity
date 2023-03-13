using System;
using LitJson;
using UnityEditor;
using UnityEngine;

public class JsonUtil
{
	public static string GetTypeOfResponseFromJson(string json)
	{
		try
		{
			var obj = JsonUtility.FromJson<ResponseBase>(json);
			return obj.response_type;
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("Failed to parse response type from json: " + e.Message);
			return null;
		}
	}

	public static string GetTypeOfRequestFromJson(string json)
	{
		try
		{
			var obj = JsonUtility.FromJson<RequestBase>(json);
			return obj.request_name;
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("Failed to parse request name from json: " + e.Message);
			return null;
		}
	}

	public static int getStatusCodeFromJson(String json)
	{
		String responseTypeString = "\"code\":\"";
		String newJson = json.Substring(json.IndexOf(responseTypeString) + responseTypeString.Length);
		return Int32.Parse(newJson.Substring(0, newJson.IndexOf("\"")));
	}

	public static String getMessageFromJson(String json)
	{
		String responseTypeString = "\"message\":\"";
		String newJson = json.Substring(json.IndexOf(responseTypeString) + responseTypeString.Length);
		return newJson.Substring(0, newJson.IndexOf("\""));
	}
	public static String getTypeOfRequestFromJson(String json)
	{
		String responseTypeString = "\"request_type\":\"";
		String newJson = json.Substring(json.IndexOf(responseTypeString) + responseTypeString.Length);
		return newJson.Substring(0, newJson.IndexOf("\""));
	}
}



[System.Serializable]
public class ResponseBase
{
	public string response_type;
}

[System.Serializable]
public class RequestBase
{
	public string request_name;
}