﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Connect : MonoBehaviour {

    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public Button registerButton;
    public Button loginButton;
    public bool autoLogin = true;

	// Use this for initialization
	void Start () {
        registerButton.onClick.AddListener(register);
        loginButton.onClick.AddListener(login);
        string userId = PlayerPrefs.GetString("USER_ID");
        if (autoLogin && userId != null && !userId.Equals(""))
        {
            Debug.Log("Auto login");
            getCommunication().userId = userId;
            SceneManager.LoadScene("Lobby");
        }
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    void login()
    {
        sendLogin(usernameInput.text, emailInput.text, passwordInput.text);
    }

    void register()
    {
        sendRegister(usernameInput.text, emailInput.text, passwordInput.text);
    }

    void sendRegister(string username, string email, string password)
    {
        Debug.Log("Register with email : " + email);
        PlayerPrefs.SetString("EMAIL", email);
        PlayerPrefs.SetString("PASSWORD", password);
        PlayerPrefs.SetString("USERNAME", username);

        getCommunication().createUser(username, email, password);

    }

    void sendLogin(string username, string email, string password)
    {
        Debug.Log("Login with email : " + email);

        string userId = PlayerPrefs.GetString("USER_ID");
        if (userId != null && !userId.Equals(""))
        {
            PlayerPrefs.SetString("EMAIL", email);
            PlayerPrefs.SetString("PASSWORD", password);
            PlayerPrefs.SetString("USERNAME", username);
            Debug.Log("Login");
            getCommunication().userId = userId;
            SceneManager.LoadScene("Lobby");
        }else
        {
            Debug.Log("You need to register first");
        }
    }






    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

}
