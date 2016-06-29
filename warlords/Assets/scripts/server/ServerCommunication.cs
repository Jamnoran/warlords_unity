using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using LitJson;

public class ServerCommunication : MonoBehaviour {

    private TcpClient mySocket;
    private NetworkStream theStream;
    private StreamWriter theWriter;
    private StreamReader theReader;
    public String Host = "127.0.0.1";
    public Int32 Port = 2055;
    internal Boolean isConnected = false;

    public Boolean isServer;



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
            print("q key was pressed");
            //sendRequest(new Request());
            //edit to create character when needed
            writeSocket("{\"request_type\": \"JOIN_SERVER\", character_id:\"1\"}");
        }
        if (Input.GetKeyUp("w"))
        {
            print("w key was pressed");
            //sendRequest(new Request());
            writeSocket("{\"request_type\": \"GET_STATUS\", character_id:\"1\"}");
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
                ResponseServerInfo responseServerInfo = JsonMapper.ToObject<ResponseServerInfo>(json);

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
