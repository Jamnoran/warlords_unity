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



    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting server.");
        connectToServer();
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





        // Code for handling input 
        if (Input.GetKeyUp("q"))
        {
            joinServer();
        }
        if (Input.GetKeyUp("w"))
        {
            getStatus();            
        }
        if (Input.GetKeyUp("e"))
        {
            createHero();
        }
        if (Input.GetKeyUp("r"))
        {
            createUser();
        }
        if (Input.GetKeyUp("a"))
        {
            attackMinion();
        }
    }

    // Send a request to lobby to join a server for joining a dungeon
    void joinServer()
    {
        print("q key was pressed  joining a server with this hero: " + heroId);
        writeSocket("{\"request_type\": \"JOIN_SERVER\", hero_id:\"" + heroId + "\"}");
    }

    // Send request to dungeon to get status of minions/heroes
    void getStatus()
    {
        print("w key was pressed getting server status (what is happening with minions/other heroes)");
        writeSocket("{\"request_type\": \"GET_STATUS\", user_id:\"" + userId + "\"}");
    }

    // Send request to lobby to create a hero. This must be done after a user has been created or it will crash
    void createHero()
    {
        print("e key was pressed creating a hero for a userid: " + userId);
        writeSocket("{\"request_type\": \"CREATE_HERO\", user_id:\"" + userId + "\", class_type:\"WARRIOR\"}");
    }

    // Send request to lobby to create a user. Primary request to be done before other becouse we will need user_id to be able to do the other requests
    void createUser()
    {
        print("r key was pressed creaing the user (here we need to gather username + email + password)");
        writeSocket("{\"request_type\": \"CREATE_USER\", email:\"" + email + "\", username:\"" + username + "\", password: \"" + password + "\"}");
    }

    int minionCount = 0;
    void attackMinion()
    {
        // Here you will need to check the id of the minion focused to send up to server.
        // This is a basic attack
        minionCount = minionCount + 1;
        print("Trying to attack minion " + minionCount);
        writeSocket("{\"request_type\": \"ATTACK\", user_id:\"" + userId + "\", minion_id: \"" + minionCount + "\"}");
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
                //((GameLogic)GameObject.Find("Game").GetComponent(typeof(GameLogic))).handleGameStatus(JsonMapper.ToObject<RequestGameStatus>(request.data));
            }
            else if (responseType == "GAME_STATUS")
            {
                ResponseGameStatus responseGameStatus = JsonMapper.ToObject<ResponseGameStatus>(json);
                Debug.Log("Response game status : " + responseGameStatus + " Heroes: " + responseGameStatus.heroes.Count + " Minions: " + responseGameStatus.minions.Count);
            }
            else if (responseType == "CREATE_USER") {
                ResponseCreateUser responseCreateUser = JsonMapper.ToObject<ResponseCreateUser>(json);
                Debug.Log("User created with this id to store on device: " + responseCreateUser.user_id);
                userId = responseCreateUser.user_id;
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

}
