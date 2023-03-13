using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResponseRotateTarget
{
    public String response_type = "ROTATE_TARGET";
    public bool friendly = true;
    public int idOfTarget = 0;
    public Vector3 targetPosition;
    public int sourceId = 0;

    public ResponseRotateTarget() { }

    public String getResponse_type()
    {
        return response_type;
    }

    public void setResponse_type(String response_type)
    {
        this.response_type = response_type;
    }

    public bool isFriendly()
    {
        return friendly;
    }

    public void setFriendly(bool friendly)
    {
        this.friendly = friendly;
    }

    public int getIdOfTarget()
    {
        return idOfTarget;
    }

    public void setIdOfTarget(int idOfTarget)
    {
        this.idOfTarget = idOfTarget;
    }

    public Vector3 getTargetPosition()
    {
        return targetPosition;
    }

    public void setTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public int getSourceId()
    {
        return sourceId;
    }

    public void setSourceId(int source)
    {
        this.sourceId = source;
    }

    public String toString()
    {
        return "RotateTargetResponse{" +
                "response_type='" + response_type + '\'' +
                ", friendly=" + friendly +
                ", idOfTarget=" + idOfTarget +
                ", targetPosition=" + getTargetPosition() +
                '}';
    }

}
