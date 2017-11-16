using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chat : MonoBehaviour {

    [SerializeField] private Demo_Chat m_Chat;
    [SerializeField] private string m_PlayerName = "Player";
    [SerializeField] private Color warriorColor = Color.white;
    [SerializeField] private Color priestColor = Color.white;
    [SerializeField] private Color warlockColor = Color.white;
    [SerializeField] private Color rogueColor = Color.white;


    public InputField inputField;
    private bool historyVisible = false;
    
    private List<Message> historyList = new List<Message>();

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp("enter") || Input.GetKeyUp("return")) {
            if (!IsInputFieldFocused()) {
                EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
                inputField.OnPointerClick(new PointerEventData(EventSystem.current));
                inputField.placeholder.GetComponent<Text>().text = "";
                Debug.Log("Enter pressed and input field is selected");
            } else {
                //TODO: If we got user name here send it!
                sendMessageInField();
            }
        }
        if (Input.GetKeyDown("escape") && IsInputFieldFocused())
        {
        //    I have not gotten this to work yet since this will get everyother scripts checking if its focused to give false positive
            inputField.text = "";
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public bool IsInputFieldFocused()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        return (obj != null && obj.tag != null && inputField != null && obj.tag == inputField.tag);
    }

    public void sendMessageInField()
    {
        if (inputField.text != "" && !inputField.text.Equals(""))
        {
            Debug.Log("Message to send: " + inputField.text);
            getCommunication().sendMessage(new Message(0, getGameLogic().getMyHero().class_type, inputField.text));
            inputField.text = "";
            inputField.placeholder.GetComponent<Text>().text = "Enter your message here...";
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void setMessages(List<Message> messages) {
        historyList = messages;
        foreach (var value in historyList)
        {
            // Maybe need to check that we dont add duplicates
            this.m_Chat.ReceiveChatMessage(1, "<color=#" + CommonColorBuffer.ColorToString(getClassColor(value.sender)) + "><b>" + value.sender + "</b></color> <color=#59524bff>said:</color>" + value.message);
        }
    }

    private Color getClassColor(string className)
    {
        if (className == "WARRIOR")
        {
            return warriorColor;
        }else if (className == "PRIEST")
        {
            return priestColor;
        }
        else if (className == "WARLOCK")
        {
            return warlockColor;
        }
        else if (className == "ROGUE")
        {
            return rogueColor;
        }
        return Color.white;
    }

    public void addMessage(Message message) {
        historyList.Add(message);
        this.m_Chat.ReceiveChatMessage(1, "<color=#" + CommonColorBuffer.ColorToString(getClassColor(message.sender)) + "><b>" + message.sender + "</b></color><color=#59524bff>:</color> " + message.message);
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    ServerCommunication getCommunication() {
        if (GameObject.Find("Communication") != null) {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        } else {
            return null;
        }
    }
}
