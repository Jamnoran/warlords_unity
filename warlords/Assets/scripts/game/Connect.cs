using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Connect : MonoBehaviour {

    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public Button registerButton;
    public Button loginButton;
    public Button resetButton;
    public bool autoLogin = true;
    
    

    // Use this for initialization
    void Start () {
        registerButton.onClick.AddListener(register);
        loginButton.onClick.AddListener(login);
        resetButton.onClick.AddListener(reset);
        int userId = PlayerPrefs.GetInt("USER_ID");

        if (autoLogin) {
            PlayerPrefs.SetInt("AUTO_LOGIN", 1);
        } else {
            PlayerPrefs.SetInt("AUTO_LOGIN", 0);
        }

        string email = PlayerPrefs.GetString("EMAIL");
        if (email != null && !email.Equals(""))
        {
            emailInput.text = email;
        }

        if (autoLogin && userId > 0) {   
            Debug.Log("Auto login, user id : " + userId);
            Debug.Log("Email: " + PlayerPrefs.GetString("EMAIL"));
            Debug.Log("Password: " + PlayerPrefs.GetString("PASSWORD"));
			getLobbyCommunication().userId = userId;
            SceneManager.LoadScene("Lobby");
        }
    }
	
	// Update is called once per frame
	void Update () {
        
	}
    

        void reset()
    {
        PlayerPrefs.SetString("EMAIL", null);
        PlayerPrefs.SetString("USER_ID", null);
        PlayerPrefs.SetString("PASSWORD", null);
        PlayerPrefs.SetString("USERNAME", null);
    }

    void login()
    {
        sendLogin("", emailInput.text, passwordInput.text);
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

		getLobbyCommunication().createUser(username, email, password);

    }

    void sendLogin(string username, string email, string password)
    {
        Debug.Log("Login with email : " + email);
        //string userId = PlayerPrefs.GetString("USER_ID");
        //if (userId != null && !userId.Equals(""))
        //{
            PlayerPrefs.SetString("EMAIL", email);
            PlayerPrefs.SetString("PASSWORD", password);
            //PlayerPrefs.SetString("USERNAME", username);
            Debug.Log("Login");
            if (getLobbyCommunication() != null) {
                getLobbyCommunication().loginUser(email, password);
            } else {
                Debug.Log("No lobby communication object");
            }
        //}else
        //{
        //    Debug.Log("You need to register first");
        //}
    }




    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

	LobbyCommunication getLobbyCommunication() {
        return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
	}
}
