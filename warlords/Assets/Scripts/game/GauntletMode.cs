using Assets.scripts.util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletMode : MonoBehaviour {

	public bool currentMode = false;
    public long startTime = 0;
    public long timeLimit = 30000;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(currentMode && !enabled){
            if (DeviceUtil.getMillis() >= (startTime + timeLimit))
            {
                // Team lost, show failed message
                getNotificationhandler().showNotification(1, "You lost, restart to continue!");
            }
            else
            {
                getNotificationhandler().showNotification(1, "Time left : " + getPrettyTime((startTime + timeLimit) - DeviceUtil.getMillis()));
            }
		}
	}

    private long getPrettyTime(long time)
    {
        return time / 1000;
    }

    public void startTimer()
    {
        startTime = DeviceUtil.getMillis();
    }
    

    GameLogic getGameLogic() {
		return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
	}


    NotificationHandler getNotificationhandler()
    {
        return ((NotificationHandler)GameObject.Find("GameLogicObject").GetComponent(typeof(NotificationHandler)));
    }
}
