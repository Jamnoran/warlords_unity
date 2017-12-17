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
        Debug.Log("Test screen is now started at: " + DeviceUtil.getMillis());
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp("y"))
        {
            timeStarted = DeviceUtil.getMillis();
            //SceneManager.LoadScene("Connect", LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync("DemoDemo", LoadSceneMode.Single);
            //getGenerator().GenerateRandom(1234567897, 1);
        }
        if (Input.GetKeyUp("u"))
        {
            timeStarted = DeviceUtil.getMillis();
            SceneManager.LoadScene("CreateHero", LoadSceneMode.Single);
            //getGenerator().GenerateRandom(1234567897, 1);
        }
        if (Input.GetKeyUp("i"))
        {
            timeStarted = DeviceUtil.getMillis();
            SceneManager.LoadScene("Connect", LoadSceneMode.Single);
            //getGenerator().GenerateRandom(1234567897, 1);
        }
    }


    void Awake()
    {
        //DontDestroyOnLoad(transform.gameObject);
    }

    DunGenerator getGenerator()
    {
        return ((DunGenerator)GameObject.Find("Generator").GetComponent(typeof(DunGenerator)));
    }

}