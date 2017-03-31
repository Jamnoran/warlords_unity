using System;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using System.Net.Configuration;

public class SocketConnection
{
	internal Boolean isConnected = false;
	private TcpClient mySocket;
	private NetworkStream theStream;
	private StreamWriter theWriter;
	private StreamReader theReader;
	public String Host = "127.0.0.1";
	public Int32 Port = 2055;

	public SocketConnection (string ip, Int32 p, bool local)
	{
		Host = ip;
		Port = p;

		setupSocket(local);
	
	}


	public void setupSocket(bool local) {
		try{
			if (local){
				mySocket = new TcpClient("127.0.0.1", Port);
			} else {
				mySocket = new TcpClient(Host, Port);
			}

			theStream = mySocket.GetStream();
			theWriter = new StreamWriter(theStream);
			theReader = new StreamReader(theStream);
			isConnected = true;
			Debug.Log("SocketReady : " + isConnected);
		} catch (Exception e){
			Debug.Log("Socket error: " + e);
		}
	}

	public void writeSocket(string theLine){
		if (!isConnected)
			return;
		String data = theLine + "\r\n";
		theWriter.Write(data);
		theWriter.Flush();
	}

	public String readSocket() {
		if (!isConnected){
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
		if (!isConnected) {
			return;
		}
		theWriter.Close();
		theReader.Close();
		mySocket.Close();
		isConnected = false;
	}


}
