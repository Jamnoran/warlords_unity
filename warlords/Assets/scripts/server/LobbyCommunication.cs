using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using Assets.scripts.vo;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters;
using System.Net.Configuration;
using LitJson;
using System.Net;
using UnityEngine.UI;

public class LobbyCommunication : MonoBehaviour {
	private SocketConnection socketConnection;
	public string webserviceUrl = "http://www.warlord.ga/warlords_webservice/lobbys.json";

	public int userId = 1;
	public int heroId = 1;

    public Boolean local = false;
    public int portForLocal = 2080;

	public Text errorMessageHolder;

	private ResponseLobbys responseLobbys;

   // Use this for initialization
    void Start(){
        Debug.Log("LobbyScreen loaded");
        if (!local)
		{
			if (errorMessageHolder != null) {	
				errorMessageHolder.text = "";
			}
            StartCoroutine(getLobbyListFromWebservice(new WWW(webserviceUrl)));
        }
    }

    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update() {
        handleCommunication();
    }



	// 				Requests 


	// User pressed joining custom game, ask server for a solo game
	public void findCustomGame(){
		Debug.Log ("Sending to server to find a custom game");
		sendRequest (new RequestFindGame(heroId, "CUSTOM", userId));
	}

	// User pressed joining custom game, ask server for a solo game
	public void findQuickGame(){
		Debug.Log ("Sending to server to find a quick game");
		sendRequest (new RequestFindGame(heroId, "QUICK", userId));
	}

	// Send a request to lobby to join a server for joining a dungeon
	public void getHeroes()
	{
		print("Getting heroes for user: " + userId);
		sendRequest (new RequestGetHeroes(userId));
	}

	// Send request to lobby to create a hero. This must be done after a user has been created or it will crash
	public void createHero(String classType) {
		sendRequest (new RequestCreateHero (userId, classType));
		getHeroes();
	}

	// Send request to lobby to create a user. Primary request to be done before other becouse we will need user_id to be able to do the other requests
	public void createUser(string username, string email, string password)
	{
		print("Creaing the user (here we need to gather username + email + password)");
		sendRequest (new RequestCreateUser(email, password, username));
	}

	public void loginUser(string email, string password)
	{
		print("Login the user (here we need to gather email + password)");
		print("{\"request_type\": \"LOGIN_USER\", email:\"" + email + "\", password: \"" + password + "\", username:\"\"}");
		sendRequest (new RequestLogin(email, password));
	}

    public void sendStartGame(LFG lfg) {
        RequestLFG requestLFG = new RequestLFG();
        requestLFG.lfg = lfg;
        requestLFG.request_type = "START_GAME";
        sendRequest(requestLFG);
    }

    public void sendCancelGameSearch(LFG lfg) {
        RequestLFG requestLFG = new RequestLFG();
        requestLFG.lfg = lfg;
        requestLFG.request_type = "CANCEL_SEARCH_GAME";
        sendRequest(requestLFG);
    }


    // 			Response

    // Code for parsing responses sent from server to client
    void parseJson(string json) {
		Debug.Log("Trying to parse this string to json object: " + json);

		// Do simple string split get response_type and go to next " and then parse the response to that format later.
		String responseType = JsonUtil.getTypeOfResponseFromJson(json);
		if (responseType != null && !responseType.Equals("") && responseType != "GAME_STATUS") {
			Debug.Log ("Type : " + responseType);
		} else {
			Debug.Log ("Type : " + responseType);
			responseType = JsonUtil.getTypeOfRequestFromJson(json);
		}

		// Handle different type of request_names
		if (responseType != null) {
#pragma warning disable CS0436 // Type conflicts with imported type
            if (responseType == "CLIENT_TYPE") {
				Debug.Log("Lobby needs to know what type of client connected");
				sendRequest (new RequestClientType(3));
                if (PlayerPrefs.GetInt("AUTO_LOGIN") == 1) {
                    getHeroes();
                }
			} else if (responseType == "SERVER_INFO") {
                ResponseServerInfo responseServerInfo = JsonMapper.ToObject<ResponseServerInfo>(json);
                Debug.Log("Server information: " + responseServerInfo.clients);
			} else if (responseType == "LOGIN_USER") {
				int responseCode = JsonUtil.getStatusCodeFromJson (json);
				if (responseCode == 608) {
					Debug.Log ("Show error for user, wrong email or password");
					errorMessageHolder.text = JsonUtil.getMessageFromJson (json);

				} else {	
					ResponseLogin responseCreateUser = JsonMapper.ToObject<ResponseLogin> (json);
					Debug.Log ("User logged in with this id to store on device: " + responseCreateUser.user_id);
					userId = responseCreateUser.user_id;
					PlayerPrefs.SetInt ("USER_ID", userId);
					SceneManager.LoadScene ("Lobby");
					Debug.Log ("Getting heroes for user");
					getHeroes ();
					errorMessageHolder.text = "";
				}
			} else if (responseType == "HEROES") {
				ResponseGetHeroes responseGetHeroes = JsonMapper.ToObject<ResponseGetHeroes>(json);
				Debug.Log("Got these many heroes: " + responseGetHeroes.heroes.Count);
				//((Lobby)GameObject.Find("LobbyLogic").GetComponent(typeof(Lobby))).updateHeroes(responseGetHeroes.heroes);
                ((LobbyLogic)GameObject.Find("LobbyLogic").GetComponent(typeof(LobbyLogic))).updateHeroes(responseGetHeroes.heroes);
            } else if (responseType == "GAME_FOUND_RESPONSE"){
                getLobbyLogic().showLoadingText();

                ResponseGameFound responseGameFound = JsonMapper.ToObject<ResponseGameFound> (json);
				Debug.Log ("Response game found: " + responseGameFound.toString ());
                if (!local)
                {
                    getCommunication().connectToServer(responseGameFound.server_ip, responseGameFound.server_port, responseGameFound.game_id);
                }
                else
                {
                    getCommunication().connectToServer("127.0.0.1", portForLocal, "0");
                }
                
            } else if (responseType == "LFG_RESPONSE") {
                ResponseLFG responseLFG = JsonMapper.ToObject<ResponseLFG>(json);
                Debug.Log("Response game found: " + responseLFG.ToString());
                getLobbyLogic().setGroup(responseLFG.lfg);
            }
 #pragma warning restore CS0436 // Type conflicts with imported type
        }
    }







	void handleCommunication() {
		// Handle communication sent from server to client (this can be a response of a request we have sent or status message etc.)
		if(socketConnection != null){
			String response = socketConnection.readSocket();
			if (response != null && response != "") {
				// Parse response to from json to object
				parseJson(response);
			}
		}
	}

	public void sendRequest(object request) {
		String reqJson = JsonMapper.ToJson(request);
		//Debug.Log("Sending this request: " + reqJson);
		socketConnection.writeSocket(reqJson);
	}


	// Handle connection 
	IEnumerator getLobbyListFromWebservice(WWW www){

		//Debug.Log ("Getting lobbys from url " + webserviceUrl);
		yield return www;
		if (www.error == null || www.error == ""){
			responseLobbys = JsonMapper.ToObject<ResponseLobbys>(www.text);
			Debug.Log ("Json data: " + responseLobbys.getLobbys().Count);
			chooseLobby();
		}
		else
		{
			Debug.Log("ERROR: [" + www.error + "] We did not get any lobby");
		}        
	}


	void chooseLobby(){
		String ip = null;
		int port = 0;
		foreach(Server server in responseLobbys.getLobbys()){
			Debug.Log("Server : " + server.getIp() + ":" + server.getPort());
			ip = server.getIp ();
			port = server.getIntPort ();
		}
		connectToServer (ip, port);
	}

	public void connectToServer(String ip, int port) {
		Debug.Log("Client Started");
		socketConnection = new SocketConnection (ip, port, false);
	}


    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    LobbyLogic getLobbyLogic() {
        return ((LobbyLogic)GameObject.Find("LobbyLogic").GetComponent(typeof(LobbyLogic)));
    }
}