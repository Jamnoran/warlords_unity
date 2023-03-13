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

    public void sendMinionHasTargetInRange(int minionId, int heroTargetId)  
    {
        if (heroTargetId != 0) {
            writeSocket("{\"request_type\": \"MINION_TARGET_IN_RANGE\", hero_id:\"" + getHeroId() + "\", minion_id: \"" + minionId + "\", hero_id: \"" + heroTargetId + "\"}");
        }
    }

    public void sendMoveRequest(float positionX, float postionY, float positionZ, float desiredPositionX, float desiredPositionY, float desiredPositionZ)  
    {
        //print("Send move request to server");
        // This will send to server the players hero location (positionX,positionY) and also the desired position the players hero wants to move to
        // Make sure this does not get sent too often (every update) because then it will spam server (have a check that handles if the hero has moved more than for example 0.5 then send request)
        sendRequest(new RequestMove(getHeroId(), positionX,postionY, positionZ, desiredPositionX, desiredPositionY, desiredPositionZ));
    }

    public void sendStopHero(int heroId) 
    {
        //print("Sending stop hero");
        sendRequest(new RequestStopHero(heroId));
    }

    public void sendSpell(int heroId, int spellId, List<int> targetEnemy, List<int> targetFriendly, Vector3 vector3)
    {
        sendSpell(heroId, spellId, targetEnemy, targetFriendly, vector3, false);
    }

    public void sendSpell(int heroId, int spellId, List<int> targetEnemy, List<int> targetFriendly, Vector3 vector3, bool initialCast) 
    {
        var time = DeviceUtil.getMillis();
        Debug.Log("Sending spell request spell id: " + spellId + " At time: " + time);
		sendRequest(new RequestSpell(heroId, spellId, targetEnemy, targetFriendly, vector3, time, initialCast));
    }

    public void sendMinionAggro(int minionId, int heroId) 
    {
        writeSocket("{\"request_type\": \"MINION_AGGRO\", \"hero_id\":" + heroId + ", \"minion_id\": " + minionId + "}");
    }

    public void heroHasClickedPortal(int heroId) 
    {
        Debug.Log("The hero: " + heroId + " has clicked the portal");
        writeSocket("{\"request_type\": \"CLICKED_PORTAL\", \"hero_id\":" + heroId + "}");
    }

	public void getAbilities(int heroId) 
    {
        Debug.Log("Get all abilities");
		writeSocket("{\"request_type\": \"GET_ABILITIES\", \"user_id\":" + heroId + "}");
    }

    public void sendMessage(Message message) 
    {
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
    public void ParseJson(string json)
    {
        string responseType = JsonUtil.GetTypeOfResponseFromJson(json);

        if (string.IsNullOrEmpty(responseType))
        {
            responseType = JsonUtil.GetTypeOfRequestFromJson(json);
        }

        if (string.IsNullOrEmpty(responseType))
        {
            Debug.Log($"Could not find correct method for this json [{json}]");
            return;
        }

        switch (responseType)
        {
            case "GAME_STATUS":
                HandleGameStatusResponse(json);
                break;
            case "WORLD":
                HandleWorldResponse(json);
                break;
            case "CLEAR_WORLD":
                HandleClearWorldResponse();
                break;
            case "TELEPORT_HEROES":
                HandleTeleportHeroesResponse(json);
                break;
            case "COOLDOWN":
                HandleCooldownResponse(json);
                break;
            case "ABILITIES":
                HandleAbilitiesResponse(json);
                break;
            case "ABILITY_STATUS":
                HandleAbilityStatusResponse(JsonUtility.FromJson<ResponseAbilityStatus>(json));
                break;
            case "STOP_HERO":
                HandleStopHeroResponse(JsonUtility.FromJson<ResponseStopHero>(json));
                break;
            case "TALENTS":
                HandleTalentsResponse(JsonUtility.FromJson<ResponseTalents>(json));
                break;
            case "UPDATE_MINION_POSITION":
                HandleUpdateMinionPositionResponse();
                break;
            case "ROTATE_TARGET":
                HandleRotateTargetResponse(JsonUtility.FromJson<ResponseRotateTarget>(json));
                break;
            case "COMBAT_TEXT":
                HandleCombatTextResponse(JsonUtility.FromJson<ResponseCombatText>(json));
                break;
            case "HERO_ITEMS":
                HandleHeroItemsResponse(JsonUtility.FromJson<ResponseItems>(json));
                break;
            case "MESSAGE":
                HandleMessageResponse(JsonUtility.FromJson<ResponseMessage>(json));
                break;
            default:
                Debug.Log($"Have type but did not match any of the ones we have [{responseType}]");
                break;
        }
    }

    private void HandleGameStatusResponse(string json)
    {
        ResponseGameStatus responseGameStatus = JsonUtility.FromJson<ResponseGameStatus>(json);

        // Debug.Log($"Parsed gameStatus: {responseGameStatus}");

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

    private void HandleWorldResponse(string json)
    {
        ResponseWorld responseWorld = JsonUtility.FromJson<ResponseWorld>(json);

        if (responseWorld?.world != null)
        {
            Debug.Log($"Creating world with seed {responseWorld.world.seed}");

            getGameLogic().createWorld(responseWorld);
        }
        else
        {
            Debug.Log("We did not receive a world, or the seed, check this out why is this happening.");
        }

        Debug.Log($"We got world after millis: {(DeviceUtil.getMillis() - millisStarted)}");
    }

    private void HandleClearWorldResponse()
    {
        getGameLogic().clearWorld();
    }

    private void HandleTeleportHeroesResponse(string json)
    {
        ResponseTeleportHeroes responseTeleportHeroes = JsonUtility.FromJson<ResponseTeleportHeroes>(json);

        getGameLogic().teleportHeroes(responseTeleportHeroes.heroes);
    }

    private void HandleCooldownResponse(string json)
    {
        ResponseCooldown responseCooldown = JsonUtility.FromJson<ResponseCooldown>(json);

        getGameLogic().updateCooldown(responseCooldown.ability);
    }

    private void HandleAbilitiesResponse(string json)
    {
        ResponseAbilities responseAbilities = JsonUtility.FromJson<ResponseAbilities>(json);

        getGameLogic().setAbilities(responseAbilities.abilities);
    }

    // Handle response for ability status update
    private void HandleAbilityStatusResponse(ResponseAbilityStatus responseAbilityStatus)
    {
        getGameLogic().updateAbilityInformation(responseAbilityStatus.ability);
    }

    // Handle response for stopping a hero
    private void HandleStopHeroResponse(ResponseStopHero responseStopHero)
    {
        getGameLogic().stopHero(responseStopHero.hero);
    }

    // Handle response for talents update
    private void HandleTalentsResponse(ResponseTalents responseTalents)
    {
        getGameLogic().setTalents(responseTalents);
    }

    // Handle response for updating minion positions
    private void HandleUpdateMinionPositionResponse()
    {
        getGameLogic().sendMinionPostionsToServer();
    }

    // Handle response for rotating target
    private void HandleRotateTargetResponse(ResponseRotateTarget responseRotateTarget)
    {
        getGameLogic().rotateTarget(responseRotateTarget);
    }

    // Handle response for displaying combat text
    private void HandleCombatTextResponse(ResponseCombatText responseCombatText)
    {
        getGameLogic().combatText(responseCombatText);
    }

    // Handle response for updating hero items
    private void HandleHeroItemsResponse(ResponseItems responseItems)
    {
        Debug.Log("Got these many items : " + responseItems.getItems().Count);
        getGameLogic().updateHeroItems(responseItems.getItems());
    }

    // Handle response for displaying a message
    private void HandleMessageResponse(ResponseMessage responseMessage)
    {
        Debug.Log("Someone wrote: " + responseMessage.message.message);
        getChat().addMessage(responseMessage.message);
    }

    void handleCommunication() {
        // Handle communication sent from server to client (this can be a response of a request we have sent or status message etc.)
        if (socketConnection != null && socketConnection.isConnected)
        {
            socketConnection.Update();
        }
    }

    public void sendRequest(object request) {
        String reqJson = JsonUtility.ToJson(request);;
        if (reqJson != null && (!reqJson.Contains("UPDATE_MINION_POSITION") && !reqJson.Contains("\"request_type\":\"MOVE\"") && !reqJson.Contains("UPDATE_MINION_POSITION")) && !reqJson.Contains("\"request_type\":\"STOP_HERO"))
        {
            Debug.Log("Sending this request: " + reqJson);
        }
        socketConnection.writeSocket(reqJson);
    }

    public void writeSocket(string request)
    {
        if (!request.Contains("\"request_type\":\"MOVE\"") && !request.Contains("\"request_type\": \"MINION_TARGET_IN_RANGE\""))
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
