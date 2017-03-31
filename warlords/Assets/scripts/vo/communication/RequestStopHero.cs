using System;
using LitJson;

[System.Serializable]
public class RequestStopHero {
    public String request_type;
    public int hero_id;

    public RequestStopHero(int hId) {
        hero_id = hId;
        setRequestType("STOP_HERO");
    }

    public void setRequestType(String requestType) {
        this.request_type = requestType;
    }

    public String getRequestType() {
        return request_type;
    }
}