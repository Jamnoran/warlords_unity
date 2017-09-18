using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;


[System.Serializable]
public class ResponseMessage {
    public string response_type = "MESSAGE";
    public Message message;

    public ResponseMessage() { }

}
