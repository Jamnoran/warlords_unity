using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Connect : MonoBehaviour {

    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public bool autoLogin = true;

    public int inputFieldFocused = 0;
    
    

    // Use this for initialization
    void Start () {
        int userId = PlayerPrefs.GetInt("USER_ID");

        if (autoLogin) {
            PlayerPrefs.SetInt("AUTO_LOGIN", 1);
        } else {
            PlayerPrefs.SetInt("AUTO_LOGIN", 0);
        }

        // Set keyboard input state to LOBBY
        if (getKeyboardInput() != null)
        {
            getKeyboardInput().setState(KeyboardInput.LOBBY);
        }

        string email = PlayerPrefs.GetString("EMAIL");
        if (email != null && !email.Equals("")) {
            emailInput.text = email;
            EventSystem.current.SetSelectedGameObject(passwordInput.gameObject, null);
            passwordInput.OnPointerClick(new PointerEventData(EventSystem.current));
            inputFieldFocused = 2;
        } else {
            EventSystem.current.SetSelectedGameObject(emailInput.gameObject, null);
            emailInput.OnPointerClick(new PointerEventData(EventSystem.current));
            inputFieldFocused = 1;
        }

        if (autoLogin && userId > 0) {   
            Debug.Log("Auto login, user id : " + userId);
            Debug.Log("Email: " + PlayerPrefs.GetString("EMAIL"));
            Debug.Log("Password: " + PlayerPrefs.GetString("PASSWORD"));
			getLobbyCommunication().userId = userId;
            SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Debug.Log("Tab pressed");
            if (inputFieldFocused == 0) {
                EventSystem.current.SetSelectedGameObject(emailInput.gameObject, null);
                emailInput.OnPointerClick(new PointerEventData(EventSystem.current));
            } else if(inputFieldFocused == 1) {
                EventSystem.current.SetSelectedGameObject(passwordInput.gameObject, null);
                passwordInput.OnPointerClick(new PointerEventData(EventSystem.current));
            } else if (inputFieldFocused == 2) {
                EventSystem.current.SetSelectedGameObject(emailInput.gameObject, null);
                emailInput.OnPointerClick(new PointerEventData(EventSystem.current));
                inputFieldFocused = 0;
            }
            inputFieldFocused++;
        } else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            Debug.Log("Enter pressed");
            login();
        }
    }
    

    public void reset() {
        PlayerPrefs.SetString("EMAIL", null);
        PlayerPrefs.SetString("USER_ID", null);
        PlayerPrefs.SetString("PASSWORD", null);
        PlayerPrefs.SetString("USERNAME", null);
    }

	public void login() {
        sendLogin("", emailInput.text, passwordInput.text);
        passwordInput.text = "";
    }

	public void startRegisterScreen(){
        SceneManager.LoadScene("CreateAccount", LoadSceneMode.Additive);
    }

	public void startLoginScreen(){
		SceneManager.LoadScene("Connect", LoadSceneMode.Additive);
	}

	public void register() {
        sendRegister(usernameInput.text, emailInput.text, passwordInput.text);
    }

    void sendRegister(string username, string email, string password) {
        Debug.Log("Register with email : " + email);
        PlayerPrefs.SetString("EMAIL", email);
        PlayerPrefs.SetString("PASSWORD", password);
        PlayerPrefs.SetString("USERNAME", username);

		getLobbyCommunication().createUser(username, email, password);

    }

    void sendLogin(string username, string email, string password)     {
        Debug.Log("Login with email : " + email);
        PlayerPrefs.SetString("EMAIL", email);
        PlayerPrefs.SetString("PASSWORD", password);
        Debug.Log("Login");
        if (getLobbyCommunication() != null) {
            getLobbyCommunication().loginUser(email, password);
        } else {
            Debug.Log("No lobby communication object");
        }
    }




    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

	LobbyCommunication getLobbyCommunication() {
        return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
	}

    KeyboardInput getKeyboardInput()
    {
        if (GameObject.Find("GameLogicObject") != null && GameObject.Find("GameLogicObject").GetComponent(typeof(KeyboardInput)) != null)
        {
            return ((KeyboardInput)GameObject.Find("GameLogicObject").GetComponent(typeof(KeyboardInput)));
        } else
        {
            return null;
        }
    }
}
