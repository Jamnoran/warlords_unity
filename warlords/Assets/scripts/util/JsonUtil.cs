using System;
using LitJson;

public class JsonUtil
{

	public static String getTypeOfResponseFromJson(String json)
	{
		String responseTypeString = "\"response_type\":\"";
		String newJson = json.Substring(json.IndexOf(responseTypeString) + responseTypeString.Length);
		return newJson.Substring(0, newJson.IndexOf("\""));
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

