using System;
using System.Collections.Generic;

[System.Serializable]
public class ResponseLobbys
{
	public List<Server> lobby;

	public ResponseLobbys() { }

	public void setLobbys(List<Server> servers){
	    lobby = servers;
	}

	public List<Server> getLobbys(){
	    return lobby;
	}

}
