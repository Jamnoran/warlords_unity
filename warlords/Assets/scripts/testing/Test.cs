using Assets.scripts.util;
using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour {

    public long timeStarted = 0;

    // Use this for initialization
    void Start () {
        timeStarted = DeviceUtil.getMillis();
        SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update() {
    }


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }


}