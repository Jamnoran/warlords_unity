using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Username : MonoBehaviour {

    public GameObject changeUsername;
    public GameObject showUsername;
    public InputField inputField;

    // Use this for initialization
    void Start ()
    {
        changeUsername.SetActive(false);
        showUsername.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showUsernameHolder()
    {
        showUsername.SetActive(true);
        changeUsername.SetActive(false);
    }

    public void setUsernameHolder(string username)
    {
        showUsername.SetActive(true);
        changeUsername.SetActive(false);
        GameObject.Find("UsernameText").GetComponent<Text>().text = username;
    }

    public void showChangeusername()
    {
        string oldUsername = GameObject.Find("UsernameText").GetComponent<Text>().text;
        showUsername.SetActive(false);
        changeUsername.SetActive(true);

        inputField.text = oldUsername;
        EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
        inputField.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    public void updateUsernameToServer()
    {
        string newUsername = inputField.text;
        Debug.Log("Wants to change username to: " + newUsername);

        getLobbyCommunication().sendUpdateUsername(newUsername);

    }


    LobbyCommunication getLobbyCommunication()
    {
        return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
    }

}
