using Assets.scripts.vo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCollissionListener : MonoBehaviour {

    public float timeUntilCheck = 1.5f;
    public float duration = 0.5f;
    public List<int> targets = new List<int>();
    public Ability ability;
    public bool test = false;

	// Use this for initialization
	void Start ()
    {
        if (!test)
        {
            StartCoroutine(ThreadForCollider());
        }
    }

    public void setAbility(Ability abi)
    {
        ability = abi;
    }

    IEnumerator ThreadForCollider()
    {
        yield return new WaitForSeconds(timeUntilCheck);
        setMeshCollider(true);
        yield return new WaitForSeconds(duration);
        setMeshCollider(false);
        Debug.Log("We found this many targets " + targets.Count);
        if (ability != null)
        {
            if (ability.targetType == "CONE" || ability.targetType == "AOE")
            {
                getGameLogic().sendSpell(ability.id, targets, null, new Vector3(0f,0f,0f), false);
            }
        }
        else
        {
            Debug.Log("No ability was set");
        }
        Destroy(gameObject);
    }
    

    private void setMeshCollider(bool enable)
    {
        MeshCollider collider = GetComponent<MeshCollider>();
        collider.enabled = enable;
    }
    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "mob1" || collision.gameObject.name == "skelmob")
        {
            ResetScript resetScript = collision.gameObject.GetComponent<ResetScript>();
            MinionInfo mInfo = resetScript.parent.GetComponent<MinionInfo>();
            if (!targets.Contains(mInfo.minionId))
            {
                Debug.Log("Minion: " + mInfo.minionId + " is in range");
                targets.Add(mInfo.minionId);
            }
        }
        else
        {
            Debug.Log("We got unkown collider");
        }
        if (test)
        {
            StartCoroutine(Clear());
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name.StartsWith("mob1") || collision.gameObject.name.StartsWith("skelmob"))
        {
            //ResetScript resetScript = collision.gameObject.GetComponent<ResetScript>();
            MinionInfo mInfo = collision.GetComponent<MinionInfo>();
            if (!targets.Contains(mInfo.minionId))
            {
                Debug.Log("Minion: " + mInfo.minionId + " is in range");
                targets.Add(mInfo.minionId);
            }
        }
        if (test)
        {
            StartCoroutine(Clear());
        }
    }

    IEnumerator Clear()
    {
        yield return new WaitForSeconds(2.0f);
        targets.Clear();
    }

    // Update is called once per frame
    void Update () {
        if (test)
        {
            setMeshCollider(true);
        }
	}


    GameLogic getGameLogic()
    {
        if (GameObject.Find("GameLogicObject") != null)
        {
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        }
        else
        {
            return null;
        }
    }
}
