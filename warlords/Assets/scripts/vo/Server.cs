using System;
using LitJson;

[System.Serializable]
public class Server {
	public string id;
	public string ip;
	public string port;
	public string version;


	public string getId() {
		return id;
	}

	public void setId(string id) {
		this.id = id;
	}

	public string getIp() {
		return ip;
	}

	public void setIp(string ip) {
		this.ip = ip;
	}

	public string getPort() {
		return port;
	}

	public void setPort(string port) {
		this.port = port;
	}

	public int getIntPort(){
		return int.Parse (port);
	}

	public string getVersion() {
		return version;
	}

	public void setVersion(string version) {
		this.version = version;
	}


	public string toString() {
		return "Server{" +
				"id='" + id + '\'' +
				", ip='" + ip + '\'' +
				", port='" + port + '\'' +
				", version='" + version + '\'' +
				'}';
	}
}
