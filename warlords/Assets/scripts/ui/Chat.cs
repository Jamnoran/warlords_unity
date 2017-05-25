using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chat : MonoBehaviour {

    public GameObject chatCanvas;
    public Text historyText;
    public GameObject inputHolder;
    public InputField inputField;
    private bool historyVisible = false;
    public bool inputVisible = false;
    
    private List<Message> historyList = new List<Message>();

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp("enter") || Input.GetKeyUp("return")) {
            inputVisible = !inputVisible;
            if (inputVisible) {
                inputHolder.SetActive(true);
                EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
                inputField.OnPointerClick(new PointerEventData(EventSystem.current));
            } else {

                if (inputField.text != "" && !inputField.text.Equals("")) {
                    Debug.Log("Message to send: " + inputField.text);
                    getCommunication().sendMessage(new Message(0, inputField.text));
                }

                inputField.text = "";
                inputHolder.SetActive(false);
            }

            showHistory();
        }
    }

    public void setMessages(List<Message> messages) {
        historyList = messages;
        showHistory();
    }

    public void addMessage(Message message) {
        historyList.Add(message);
        showHistory();
    }

    void showHistory() {
        historyVisible = true;
        if (historyVisible) {
            string history = "";
            foreach (var value in historyList) {
                history = history + "\n" + value.message;
            }
            historyText.text = history;
        } else {
            historyText.text = "";
        }
    }


    ServerCommunication getCommunication() {
        if (GameObject.Find("Communication") != null) {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        } else {
            return null;
        }
    }
}
