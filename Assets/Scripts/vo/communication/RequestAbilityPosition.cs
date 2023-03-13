using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

public class RequestAbilityPosition {
    public String request_type;
    public int ability_id;
    public int hero_id = 0;
    public int position;

    public RequestAbilityPosition(int hId, int aId, int pos) {
        setRequestType("UPDATE_ABILITY_POSITION");
        hero_id = hId;
        ability_id = aId;
        position = pos;
    }

    public void setRequestType(String requestType) {
        this.request_type = requestType;
    }

    public String getRequestType() {
        return request_type;
    }
}