using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using LitJson;
using Assets.scripts.vo;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ServerCommunication : MonoBehaviour {

    private TcpClient mySocket;
    private NetworkStream theStream;
    private StreamWriter theWriter;
    private StreamReader theReader;
    public String Host = "127.0.0.1";
    public Int32 Port = 2055;
    internal Boolean isConnected = false;

    public String userId = "1";
    public String heroId = "1";
    //public String username = "lasse";
    //public String password = "losen";
    //public String email = "lasse@gmail.com";
    public Int32 gameId = -1;

    public Boolean local = true;


    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting server.");
        connectToServer();
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        handleCommunication();
    }


    void handleCommunication() {

        // Handle communication sent from server to client (this can be a response of a request we have sent or status message etc.)
        String response = readSocket();
        if (response != null && response != "")
        {
            // Parse response to from json to object
            parseJson(response);
        }

    }

    // Send a request to lobby to join a server for joining a dungeon
    public void joinServer()
    {
        print("q key was pressed  joining a server with this hero: " + heroId);
        writeSocket("{\"request_type\": \"JOIN_SERVER\", hero_id:\"" + heroId + "\"}");
    }
    // Send a request to lobby to join a server for joining a dungeon
    public void getHeroes()
    {
        print("Getting heroes for user: " + userId);
        writeSocket("{\"request_type\": \"GET_HEROES\", user_id:\"" + userId + "\"}");
    }
    

    public void endGame()
    {
        print("End game request was sent: " + userId);
        writeSocket("{\"request_type\": \"END_GAME\", hero_id:\"" + userId + "\"}");
        ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).endGame();

    }

    // Send request to dungeon to get status of minions/heroes
    public void getStatus()
    {
        print("w key was pressed getting server status (what is happening with minions/other heroes)");
        writeSocket("{\"request_type\": \"GET_STATUS\", user_id:\"" + userId + "\"}");
    }

    // Send request to lobby to create a hero. This must be done after a user has been created or it will crash
    public void createHero(String classType)
    {
        print("e key was pressed creating a hero for a userid: " + userId);
        writeSocket("{\"request_type\": \"CREATE_HERO\", user_id:\"" + userId + "\", class_type:\""+ classType + "\"}");
        getHeroes();
    }

    // Send request to lobby to create a user. Primary request to be done before other becouse we will need user_id to be able to do the other requests
    public void createUser(string username, string email, string password)
    {
        print("Creaing the user (here we need to gather username + email + password)");
        writeSocket("{\"request_type\": \"CREATE_USER\", email:\"" + email + "\", username:\"" + username + "\", password: \"" + password + "\"}");
    }

    public void loginUser(string email, string password)
    {
        print("Login the user (here we need to gather email + password)");
        print("{\"request_type\": \"LOGIN_USER\", email:\"" + email + "\", password: \"" + password + "\", username:\"\"}");
        writeSocket("{\"request_type\": \"LOGIN_USER\", email:\"" + email + "\", password: \"" + password + "\", username:\"\"}");
    }

    public void sendAutoAttack(int minionId) {
        if (minionId > 0)
        {
            print("Trying to attack minion " + minionId);
            writeSocket("{\"request_type\": \"ATTACK\", user_id:\"" + userId + "\", minion_id: \"" + minionId + "\"}");
        }
        else
        {
            print("You have no target!!! click on something");
        }
    }

    public void sendMinionHasTargetInRange(int minionId, int heroTargetId)
    {
        if (heroTargetId != 0)
        {
            print("Minion has hero in range " + minionId);
        }
        writeSocket("{\"request_type\": \"MINION_TARGET_IN_RANGE\", user_id:\"" + userId + "\", minion_id: \"" + minionId + "\", hero_id: \"" + heroTargetId + "\"}");
    }

    public void sendMoveRequest(float positionX, float positionZ, float desiredPositionX, float desiredPositionZ)
    {
        //print("Send move request to server");
        // This will send to server the players hero location (positionX,positionY) and also the desired position the players hero wants to move to
        // Make sure this does not get sent too often (every update) because then it will spam server (have a check that handles if the hero has moved more than for example 0.5 then send request)
        writeSocket("{\"request_type\": \"MOVE\", user_id:\"" + userId + "\", position_x: \"" + positionX + "\", position_z: \"" + positionZ + "\", desired_position_x: \"" + desiredPositionX + "\", desired_position_z: \"" + desiredPositionZ+ "\"}");
    }


    public void sendSpell(int spellId, List<int> targetEnemy, List<int> targetFriendly, Vector3 vector3)
    {
        var dt = DateTime.Now;
        var time = (dt.Hour * 60 * 1000) + (dt.Second * 1000) + dt.Millisecond;
        Debug.Log("Sending spell request spell id: " + spellId + " At time: " + time);
        var targets = "[]";
        if (targetEnemy.Count > 0)
        {
            targets = "[" + targetEnemy[0] + "]";
        }
        
        var friendly = "[]";
        if (targetFriendly.Count > 0)
        {
            friendly = "[" + targetFriendly[0] + "]";
        }
        Debug.Log("{\"request_type\": \"SPELL\", user_id:\"" + userId + "\", spell_id: " + spellId + ", time: " + time + ", target_enemy: " + targets + ", target_friendly: " + friendly + ", target_position_x: \"" + vector3.x + "\", target_position_z: \"" + vector3.z + "\"}");
        writeSocket("{\"request_type\": \"SPELL\", user_id:\"" + userId + "\", spell_id: " + spellId + ", time: " + time + ", target_enemy: " + targets + ", target_friendly: " + friendly + ", target_position_x: \"" + vector3.x + "\", target_position_z: \"" + vector3.z + "\"}");
    }


    public void sendMinionAggro(int minionId, int heroId)
    {
        Debug.Log("Sending minion aggro : " + minionId + " on hero: " + heroId);
        writeSocket("{\"request_type\": \"MINION_AGGRO\", \"hero_id\":" + heroId + ", \"minion_id\": " + minionId + "}");
    }

    public void heroHasClickedPortal(int heroId)
    {
        Debug.Log("The hero: " + heroId + " has clicked the portal");
        writeSocket("{\"request_type\": \"CLICKED_PORTAL\", \"hero_id\":" + heroId + "}");
    }

    public void getAbilities()
    {
        Debug.Log("Get all abilities");
        writeSocket("{\"request_type\": \"GET_ABILITIES\", \"user_id\":" + userId + "}");
    }

































    // Code for parsing responses sent from server to client
    void parseJson(string json)
    {
        //Debug.Log("Trying to parse this string to json object: " + json);

        // Do simple string split get response_type and go to next " and then parse the response to that format later.
        String responseType = getTypeOfResponseFromJson(json);
        if (responseType != null && responseType != "GAME_STATUS") {
            Debug.Log("Request type : " + responseType);
        }

        // Handle different type of request_names
        if (responseType != null)
        {
            if (responseType == "SERVER_INFO")
            {
                ResponseServerInfo responseServerInfo = JsonMapper.ToObject<ResponseServerInfo>(json);
                Debug.Log("Server information: " + responseServerInfo.clients);
                if (Int32.Parse(userId) > 0)
                {
                    Debug.Log("Sending request to get heroes");
                    getHeroes();
                }
            }
            else if (responseType == "HEROES")
            {
                ResponseGetHeroes responseGetHeroes = JsonMapper.ToObject<ResponseGetHeroes>(json);
                Debug.Log("Got these many heroes: " + responseGetHeroes.heroes.Count);
                ((Lobby)GameObject.Find("LobbyLogic").GetComponent(typeof(Lobby))).updateHeroes(responseGetHeroes.heroes);
            }
            else if (responseType == "GAME_STATUS")
            {
                ResponseGameStatus responseGameStatus = JsonMapper.ToObject<ResponseGameStatus>(json);
                if (responseGameStatus.gameAnimations.Count > 0)
                {
                    ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).updateAnimations(responseGameStatus.gameAnimations);
                }
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).updateListOfMinions(responseGameStatus.minions);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).updateListOfHeroes(responseGameStatus.heroes);
            }
            else if (responseType == "CREATE_USER")
            {
                ResponseCreateUser responseCreateUser = JsonMapper.ToObject<ResponseCreateUser>(json);
                Debug.Log("User created with this id to store on device: " + responseCreateUser.user_id);
                userId = responseCreateUser.user_id;
                PlayerPrefs.SetString("USER_ID", userId);
                SceneManager.LoadScene("Lobby");
            }
            else if (responseType == "LOGIN_USER")
            {
                ResponseCreateUser responseCreateUser = JsonMapper.ToObject<ResponseCreateUser>(json);
                Debug.Log("User logged in with this id to store on device: " + responseCreateUser.user_id);
                userId = responseCreateUser.user_id;
                PlayerPrefs.SetString("USER_ID", userId);
                SceneManager.LoadScene("Lobby");
            }
            else if (responseType == "WORLD")
            {
                ResponseWorld responseWorld = JsonMapper.ToObject<ResponseWorld>(json);
                Debug.Log("Creating world: ");
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).createWorld(responseWorld);
            }
            else if (responseType == "CLEAR_WORLD")
            {
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).clearWorld();
            }
            else if (responseType == "TELEPORT_HEROES")
            {
                ResponseTeleportHeroes responseTeleportHeroes = JsonMapper.ToObject<ResponseTeleportHeroes>(json);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).teleportHeroes(responseTeleportHeroes.heroes);
            }
            else if (responseType == "COOLDOWN")
            {
                ResponseCooldown responseCooldown = JsonMapper.ToObject<ResponseCooldown>(json);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).updateCooldown(responseCooldown.ability);
            }
            else if (responseType == "ABILITIES")
            {
                ResponseAbilities responseAbilities = JsonMapper.ToObject<ResponseAbilities>(json);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).setAbilities(responseAbilities.abilities);
            }
        }
    }















    public void sendRequest(Request request)
    {
        String reqJson = JsonMapper.ToJson(request);
        Debug.Log("Sending this request: " + reqJson);
        writeSocket(reqJson);
    }

    public void connectToServer()
    {
        msg("Client Started");
        setupSocket();
    }
    



    private static String getTypeOfResponseFromJson(String json)
    {
        String responseTypeString = "\"response_type\":\"";
        String newJson = json.Substring(json.IndexOf(responseTypeString) + responseTypeString.Length);
        return newJson.Substring(0, newJson.IndexOf("\""));
    }

    void msg(string mesg)
    {
        Debug.Log("Debug: " + mesg);
    }



    public void setupSocket()
    {
        try
        {
            if (local)
            {
                mySocket = new TcpClient("127.0.0.1", Port);
            }
            else
            {
                mySocket = new TcpClient(Host, Port);
            }
            
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            isConnected = true;
            msg("SocketReady : " + isConnected);
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
    }


    public void writeSocket(string theLine)
    {
        if (!isConnected)
            return;
        String foo = theLine + "\r\n";
        theWriter.Write(foo);
        theWriter.Flush();
    }

    public String readSocket()
    {
        if (!isConnected)
        {
            return "";
        }
        if (theStream != null && theStream.DataAvailable)
        {
            return theReader.ReadLine();
        }
        return "";
    }

    public void closeSocket()
    {
        if (!isConnected)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        isConnected = false;
    }

    public string getHeroId()
    {
        return heroId;
    }
}
