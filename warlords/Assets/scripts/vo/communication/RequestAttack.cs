using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

public class RequestAttack {
    public String request_type;
    public long time;
    public int hero_id = 0;
    public int minion_id = 0;

    public RequestAttack(int hId, int mId, long t) {
        minion_id = mId;
        hero_id = hId;
        time = t;
        setRequestType("ATTACK");
    }

    public void setRequestType(String requestType) {
        this.request_type = requestType;
    }

    public String getRequestType() {
        return request_type;
    }
}