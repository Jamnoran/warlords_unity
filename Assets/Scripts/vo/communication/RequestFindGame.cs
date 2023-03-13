using System;
using LitJson;

[System.Serializable]
public class RequestFindGame : Request{
	public String game_type;
	public int hero_id = 0;
	public int user_id = 0;

	public RequestFindGame(int heroId, String gameType, int uId) {
		hero_id = heroId;
		game_type = gameType;
		user_id = uId;
		setRequestType("JOIN_GAME");
	}

	public void setRequestType(String requestType) {
		this.request_type = requestType;
	}

	public String getRequestType() {
		return request_type;
	}

	public int getHeroId(){
		return hero_id;
	}

	public void setHeroId(int heroId){
		hero_id = heroId;
	}
}