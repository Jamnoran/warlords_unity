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
    public String username = "bosse";
    public String password = "losen";
    public String email = "bosse@gmail.com";



    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting server.");
        connectToServer();
    }


    // Update is called once per frame
    void Update()
    {
        String response = readSocket();
        if (response != null && response != "")
        {
            // Parse response to from json to object
            parseJson(response);
        }

        if (Input.GetKeyUp("q"))
        {
            print("q key was pressed  joining a server as userid: " + userId);
            writeSocket("{\"request_type\": \"JOIN_SERVER\", user_id:\"" + userId + "\"}");
        }
        if (Input.GetKeyUp("w"))
        {
            print("w key was pressed getting server status (what is happening with minions/other heroes)");
            writeSocket("{\"request_type\": \"GET_STATUS\", user_id:\"" + userId + "\"}");
        }
        if (Input.GetKeyUp("e"))
        {
            print("e key was pressed creating a hero for a userid: " + userId);
            writeSocket("{\"request_type\": \"CREATE_HERO\", user_id:\"" + userId + "\", class_type:\"WARRIOR\"}");
        }
        if (Input.GetKeyUp("r"))
        {
            print("r key was pressed creaing the user (here we need to gather username + email + password)");
            writeSocket("{\"request_type\": \"CREATE_USER\", email:\"" + email + "\", username:\"" + username + "\", password: \"" + password + "\"}");
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
                Debug.Log("Response game status : " + responseGameStatus + " Minions: ");


            }
        }
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
