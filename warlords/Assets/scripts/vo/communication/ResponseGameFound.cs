using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;
using System;

[System.Serializable]
public class ResponseGameFound {
	public string response_type = "GAME_FOUND_RESPONSE";
	public string server_id;
	public string server_ip;
	public int server_port;
	public int hero_id;
	public string game_id;

	public ResponseGameFound(){}

	public String toString(){
		return "server_id = " + server_id + ", server_ip = " + server_ip + ", game_id = " + game_id;
	}

}
