using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using LitJson;
using Assets.scripts.vo;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.scripts.util;

public class ServerCommunication : MonoBehaviour {

    private SocketConnection socketConnection;

    // Use this for initialization
    void Start()
    {
        if (getLobbyCommunication().local)
        {
            connectToServer("127.0.0.1", getLobbyCommunication().portForLocal, "0");
        }
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
    

    public int getHeroId()
    {
        return getLobbyCommunication().heroId;
    }




    // Send a request to lobby to join a server for joining a dungeon
    public void joinGame(string gameId) {
        print("Joining game with this hero : " + getHeroId() + " game id: " + gameId);
        sendRequest(new RequestJoinGame(getHeroId(), gameId));
        Debug.Log(" Created socket and sending join game after: " + (DeviceUtil.getMillis() - millisStarted));
    }

    public void endGame() {
        print("End game request was sent: " + getHeroId());
        writeSocket("{\"request_type\": \"END_GAME\", hero_id:\"" + getHeroId() + "\"}");
        ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).endGame();
    }

    // Send request to dungeon to get status of minions/heroes
    public void getStatus() {
        print("Getting server status (what is happening with minions/other heroes)");
        writeSocket("{\"request_type\": \"GET_STATUS\", hero_id:\"" + getHeroId() + "\"}");
    }

    public void sendSpawnPoints(List<Point> points) {
        print("Sending spawn locations");
        sendRequest(new RequestSpawnPoints(getHeroId(), points));
    }

    public void sendAutoAttack(int minionId) {
        if (minionId > 0) {
            //print("Trying to attack minion " + minionId);
            var time = DeviceUtil.getMillis();
            sendRequest(new RequestAttack(getHeroId(), minionId, time));
        } else {
            print("You have no target!!! click on something");
        }
    }


    public void restartLevel()  {
        print("Sending restart level");
        writeSocket("{\"request_type\": \"RESTART_LEVEL\", hero_id:\"" + getHeroId() + "\"}");
    }

    public void sendMinionHasTargetInRange(int minionId, int heroTargetId)  {
        if (heroTargetId != 0) {
            //print("Minion has hero in range " + minionId);
            writeSocket("{\"request_type\": \"MINION_TARGET_IN_RANGE\", hero_id:\"" + getHeroId() + "\", minion_id: \"" + minionId + "\", hero_id: \"" + heroTargetId + "\"}");
        }
    }

    public void sendMoveRequest(float positionX, float postionY, float positionZ, float desiredPositionX, float desiredPositionY, float desiredPositionZ)  {
        //print("Send move request to server");
        // This will send to server the players hero location (positionX,positionY) and also the desired position the players hero wants to move to
        // Make sure this does not get sent too often (every update) because then it will spam server (have a check that handles if the hero has moved more than for example 0.5 then send request)
        sendRequest(new RequestMove(getHeroId(), positionX,postionY, positionZ, desiredPositionX, desiredPositionY, desiredPositionZ));
    }

    public void sendStopHero(int heroId) {
        //print("Sending stop hero");
        sendRequest(new RequestStopHero(heroId));
    }

    public void sendSpell(int heroId, int spellId, List<int> targetEnemy, List<int> targetFriendly, Vector3 vector3)
    {
        sendSpell(heroId, spellId, targetEnemy, targetFriendly, vector3, false);
    }

    public void sendSpell(int heroId, int spellId, List<int> targetEnemy, List<int> targetFriendly, Vector3 vector3, bool initialCast) {
        var time = DeviceUtil.getMillis();
        Debug.Log("Sending spell request spell id: " + spellId + " At time: " + time);
		sendRequest(new RequestSpell(heroId, spellId, targetEnemy, targetFriendly, vector3, time, initialCast));
    }

    public void sendMinionAggro(int minionId, int heroId) {
        //Debug.Log("Sending minion aggro : " + minionId + " on hero: " + heroId);
        writeSocket("{\"request_type\": \"MINION_AGGRO\", \"hero_id\":" + heroId + ", \"minion_id\": " + minionId + "}");
    }

    public void heroHasClickedPortal(int heroId) {
        Debug.Log("The hero: " + heroId + " has clicked the portal");
        writeSocket("{\"request_type\": \"CLICKED_PORTAL\", \"hero_id\":" + heroId + "}");
    }

	public void getAbilities(int heroId) {
        Debug.Log("Get all abilities");
		writeSocket("{\"request_type\": \"GET_ABILITIES\", \"user_id\":" + heroId + "}");
    }

    public void sendMessage(Message message) {
        Debug.Log("Sending message to team");
        sendRequest(new RequestSendMessage(message));
    }

	public void updateAbilityPosition(int heroId, int abilityId, int position) {
		sendRequest(new RequestAbilityPosition(heroId, abilityId, position));
    }

	public void updateTalents(int heroId, List<Talent> updatedListOfTalents)
    {
        Debug.Log("Sending update talents to server");
		sendRequest(new RequestTalents(heroId, updatedListOfTalents));
    }

	public void  sendUpdateMinionPosition(int heroId, List<Minion> updatedList){
		sendRequest(new RequestUpdateMinionPosition(heroId, updatedList));
	}

    public void updateItem(int heroId, int lootId, int positionId, bool equipped)
    {
        Debug.Log("Updating item : " + lootId + " pos: " + positionId);
        sendRequest(new RequestUpdateLoot(heroId, lootId, positionId, equipped));
    }
    
    public void selfDamage()
    {
        print("Sending self damage");
        writeSocket("{\"request_type\": \"SELF_DAMAGE\", hero_id:\"" + getHeroId() + "\"}");
    }























    // Code for parsing responses sent from server to client
    public void parseJson(string json)
    {
        //Debug.Log("Time[" + (DeviceUtil.getMillis() - millisStarted) + "] Trying to parse this string to json object: " + json);

        // Do simple string split get response_type and go to next " and then parse the response to that format later.
        String responseType = JsonUtil.getTypeOfResponseFromJson(json);
        if (responseType != null) {
        } else {
            responseType = JsonUtil.getTypeOfRequestFromJson(json);
        }

		//if (responseType != null && !responseType.Equals("") && responseType != "GAME_STATUS" && responseType != "UPDATE_MINION_POSITION" && responseType != "STOP_HERO") {
        //    Debug.Log("Type request: " + responseType);
        //}

        // Handle different type of request_names
        if (responseType != null && !responseType.Equals("")) {
            if (responseType.Equals("GAME_STATUS"))
            {
                //Debug.Log("Update game status After " + (DeviceUtil.getMillis() - millisStarted));
                //Debug.Log("GameStatus[" + json + "]");
                ResponseGameStatus responseGameStatus = JsonMapper.ToObject<ResponseGameStatus>(json);
                if (responseGameStatus.gameAnimations.Count > 0)
                {
                    getGameLogic().updateAnimations(responseGameStatus.gameAnimations);
                }
                getGameLogic().updateListOfMinions(responseGameStatus.minions);
                getGameLogic().updateListOfHeroes(responseGameStatus.heroes);
                if (getGameLogic().isGameMode(World.HORDE_MODE))
                {
                    getHordeMode().totalMinionsLeft = responseGameStatus.totalMinionsLeft;
                }
            }
            else if (responseType == "WORLD")
            {
                ResponseWorld responseWorld = JsonMapper.ToObject<ResponseWorld>(json);
                if (responseWorld != null && responseWorld.world != null)
                {
                    Debug.Log("Creating world with seed " + responseWorld.world.seed + " After " + (DeviceUtil.getMillis() - millisStarted));
                    getGameLogic().createWorld(responseWorld);
                }
                else
                {
                    Debug.Log("We did not recieve a world, or the seed, check this out why is this happening.");
                }

                Debug.Log("We got world after millis : " + (DeviceUtil.getMillis() - millisStarted));
            }
            else if (responseType == "CLEAR_WORLD")
            {
                getGameLogic().clearWorld();
            }
            else if (responseType == "TELEPORT_HEROES")
            {
                ResponseTeleportHeroes responseTeleportHeroes = JsonMapper.ToObject<ResponseTeleportHeroes>(json);
                getGameLogic().teleportHeroes(responseTeleportHeroes.heroes);
            }
            else if (responseType == "COOLDOWN")
            {
                ResponseCooldown responseCooldown = JsonMapper.ToObject<ResponseCooldown>(json);
                getGameLogic().updateCooldown(responseCooldown.ability);
            }
            else if (responseType == "ABILITIES")
            {
                ResponseAbilities responseAbilities = JsonMapper.ToObject<ResponseAbilities>(json);
                getGameLogic().setAbilities(responseAbilities.abilities);
            }
            else if (responseType == "ABILITY_STATUS")
            {
                ResponseAbilityStatus responseAbilityStatus = JsonMapper.ToObject<ResponseAbilityStatus>(json);
                getGameLogic().updateAbilityInformation(responseAbilityStatus.ability);
            }
            else if (responseType == "STOP_HERO")
            {
                ResponseStopHero responseStopHero = JsonMapper.ToObject<ResponseStopHero>(json);
                getGameLogic().stopHero(responseStopHero.hero);
            }
            else if (responseType == "TALENTS")
            {
                ResponseTalents response = JsonMapper.ToObject<ResponseTalents>(json);
                getGameLogic().setTalents(response);
            }
            else if (responseType == "UPDATE_MINION_POSITION")
            {
                getGameLogic().sendMinionPostionsToServer();
            }
            else if (responseType == "ROTATE_TARGET")
            {
                ResponseRotateTarget responseRotateTarget = JsonMapper.ToObject<ResponseRotateTarget>(json);
                getGameLogic().rotateTarget(responseRotateTarget);
            }
            else if (responseType == "COMBAT_TEXT")
            {
                ResponseCombatText responseCombatText = JsonMapper.ToObject<ResponseCombatText>(json);
                getGameLogic().combatText(responseCombatText);
            }
            else if (responseType == "HERO_ITEMS")
            {
                Debug.Log("Json[" + json + "]");
                ResponseItems responseItems = JsonMapper.ToObject<ResponseItems>(json);
                Debug.Log("Got these many items : " + responseItems.getItems().Count);
                getGameLogic().updateHeroItems(responseItems.getItems());
            }
            else if (responseType == "MESSAGE")
            {
                ResponseMessage response = JsonMapper.ToObject<ResponseMessage>(json);
                Debug.Log("Someone wrote: " + response.message.message);
                getChat().addMessage(response.message);
            }
            else
            {
                Debug.Log("Have type but did not match any of the ones we have [" + responseType + "]");
            }
        } else {
            Debug.Log("Could not find correct method for this json [" + json + "]");
        }
    }































































    void handleCommunication() {
        // Handle communication sent from server to client (this can be a response of a request we have sent or status message etc.)
        if (socketConnection != null && socketConnection.isConnected)
        {
            socketConnection.Update();
        }
    }

    public void sendRequest(object request) {
        String reqJson = JsonMapper.ToJson(request);
        if (reqJson != null && (!reqJson.Contains("UPDATE_MINION_POSITION") && !reqJson.Contains("\"request_type\": \"MOVE\"") && !reqJson.Contains("UPDATE_MINION_POSITION")))
        {
            Debug.Log("Sending this request: " + reqJson);
        }
        socketConnection.writeSocket(reqJson);
    }

    public void writeSocket(string request)
    {
        if (!request.Contains("\"request_type\": \"MOVE\"") && !request.Contains("\"request_type\": \"MINION_TARGET_IN_RANGE\""))
        {
            Debug.Log("Sending this request: " + request);
        }
        socketConnection.writeSocket(request);
    }


    public long millisStarted = 0;

    public void connectToServer(String ip, int port, String gameId) {
        millisStarted = DeviceUtil.getMillis();
        if (!getLobbyCommunication().local)
        {
            Debug.Log("Changing to game scene");
            SceneManager.LoadScene("Game");
            //SceneManager.LoadScene("DemoDemo", LoadSceneMode.Single);
        }
        Debug.Log("Client Started");
        socketConnection = new SocketConnection(ip, port, getLobbyCommunication().local, false);
        joinGame(gameId);
    }

    public void closeCommunication() {
        socketConnection.closeSocket();
        socketConnection = null;
    }


    LobbyCommunication getLobbyCommunication() {
        return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
    }

    GameLogic getGameLogic() {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }


    Chat getChat() {
        return ((Chat)GameObject.Find("GameLogicObject").GetComponent(typeof(Chat)));
    }

	HordeMode getHordeMode() {
		return ((HordeMode)GameObject.Find("GameLogicObject").GetComponent(typeof(HordeMode)));
	}

}
