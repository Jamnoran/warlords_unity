using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using LitJson;
using Assets.scripts.vo;

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
    public String username = "lasse";
    public String password = "losen";
    public String email = "lasse@gmail.com";
    public Int32 gameId = -1;



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

        
        if (Input.GetKeyUp("a"))
        {
            attackMinion();
        }
    }

    // Send a request to lobby to join a server for joining a dungeon
    public void joinServer()
    {
        print("q key was pressed  joining a server with this hero: " + heroId);
        writeSocket("{\"request_type\": \"JOIN_SERVER\", hero_id:\"" + heroId + "\"}");
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
    public void createHero()
    {
        print("e key was pressed creating a hero for a userid: " + userId);
        writeSocket("{\"request_type\": \"CREATE_HERO\", user_id:\"" + userId + "\", class_type:\"WARRIOR\"}");
    }

    // Send request to lobby to create a user. Primary request to be done before other becouse we will need user_id to be able to do the other requests
    public void createUser()
    {
        print("r key was pressed creaing the user (here we need to gather username + email + password)");
        writeSocket("{\"request_type\": \"CREATE_USER\", email:\"" + email + "\", username:\"" + username + "\", password: \"" + password + "\"}");
    }

    public void attackMinion()
    {
        // Here you will need to check the id of the minion focused to send up to server.
        // This is a basic attack
        int minonId = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero().targetEnemy;
        if (minonId > 0)
        {
            print("Trying to attack minion " + minonId);
            writeSocket("{\"request_type\": \"ATTACK\", user_id:\"" + userId + "\", minion_id: \"" + minonId + "\"}");
        }
        else
        {
            print("You have no target!!! click on something");
        }
        
    }

    public void sendMoveRequest(float positionX, float positionZ, float desiredPositionX, float desiredPositionZ)
    {
        //print("Send move request");
        // This will send to server the players hero location (positionX,positionY) and also the desired position the players hero wants to move to
        // Make sure this does not get sent too often (every update) because then it will spam server (have a check that handles if the hero has moved more than for example 0.5 then send request)
        writeSocket("{\"request_type\": \"MOVE\", user_id:\"" + userId + "\", position_x: \"" + positionX + "\", position_z: \"" + positionZ + "\", desired_position_x: \"" + desiredPositionX + "\", desired_position_z: \"" + desiredPositionZ+ "\"}");
    }



    // Code for parsing responses sent from server to client
    void parseJson(string json)
    {
        Debug.Log("Trying to parse this string to json object: " + json);

        // Do simple string split get response_type and go to next " and then parse the response to that format later.
        String responseType = getTypeOfResponseFromJson(json);
        Debug.Log("Request type : " + responseType);
        // Handle different type of request_names
        if (responseType != null)
        {
            if (responseType == "SERVER_INFO")
            {
                ResponseServerInfo responseServerInfo = JsonMapper.ToObject<ResponseServerInfo>(json);
                Debug.Log("Server information: " + responseServerInfo.clients);
            }
            else if (responseType == "GAME_STATUS")
            {
                ResponseGameStatus responseGameStatus = JsonMapper.ToObject<ResponseGameStatus>(json);
                Debug.Log("Response game status : " + responseGameStatus + " Heroes: " + responseGameStatus.heroes.Count + " Minions: " + responseGameStatus.minions.Count);
                if (responseGameStatus.gameAnimations.Count > 0)
                {
                    ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).updateAnimations(responseGameStatus.gameAnimations);
                }
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).updateListOfMinions(responseGameStatus.minions);
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).updateListOfHeroes(responseGameStatus.heroes);
            }
            else if (responseType == "CREATE_USER") {
                ResponseCreateUser responseCreateUser = JsonMapper.ToObject<ResponseCreateUser>(json);
                Debug.Log("User created with this id to store on device: " + responseCreateUser.user_id);
                userId = responseCreateUser.user_id;
            }
            else if (responseType == "WORLD")
            {
                ResponseWorld responseWorld = JsonMapper.ToObject<ResponseWorld>(json);
                Debug.Log("Creating world: " );
                ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).createWorld(responseWorld);
            }
        }
    }

    public void sendSpell(int spellId, int targetEnemy, int targetFriendly, Vector3 vector3)
    {
        Debug.Log("Sending spell request spell id: " + spellId);
        writeSocket("{\"request_type\": \"SPELL\", user_id:\"" + userId + "\", spell_id: " + spellId + ", target_enemy: " + targetEnemy + ", target_friendly: " + targetFriendly + ", target_position_x: \"" + vector3.x + "\", target_position_z: \"" + vector3.z + "\"}");
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
            mySocket = new TcpClient(Host, Port);
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
