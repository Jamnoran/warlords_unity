using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public GameObject self;
    public GameObject target;
    public Vector3 targetPosition;
    public GameObject source;
    public int minionId;
    public int targetHeroId;
    public float speed = 1.0f;
    public bool fixedHight = false;
    public float timeToReachGoal = 0.0f;
    float t;

    // Use this for initialization
    void Start () {
        if (source != null)
        {
            transform.position = source.transform.position;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (minionId > 0)
        {
            Minion min = getGameLogic().getMinion(minionId);
            if (!min.alive)
            {
                Destroy(self);
            }
            targetPosition = min.minionTransform.position;
        }
        if (target != null || targetPosition != null)
        {
            Vector3 pos;
            if (target != null)
            {
                pos = target.transform.position;
            }
            else
            {
                pos = targetPosition;
            }
            if (timeToReachGoal > 0)
            {
                t += Time.deltaTime / timeToReachGoal;
                transform.position = Vector3.Lerp(source.transform.position, pos, t);
            }
            else
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, pos, step);
            }
            
        }
    }

    public void setTarget(GameObject tar)
    {
        target = tar;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            // Check if correct enemy
            Destroy(self);
        }

    }


    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
