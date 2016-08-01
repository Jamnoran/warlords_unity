using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.scripts.vo;

public class Lobby : MonoBehaviour {
    public Button changeHero = null;
    public Button quickGame = null;
    public Button topLeftButton = null;
    public Button topRightButton = null;
    public Button botLeftButton = null;
    public Button botRightButton = null;

    private Hero currentHero = null;

    // Use this for initialization
    void Start () {
        changeHero.onClick.AddListener(() => { showheroDialog(); });
        quickGame.onClick.AddListener(() => { startQuickGame(); });
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void showheroDialog() {
        Debug.Log("Showing hero dialog");
    }

    void startQuickGame()
    {
        Debug.Log("Starting a quick game");
    }
}
