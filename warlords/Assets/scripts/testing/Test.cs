using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour {
    public Transform minion1;

    // Use this for initialization
    void Start () {
        initiateMinon();

    }

    // Update is called once per frame
    void Update() {
    }


    void initiateMinon() {
        Minion newMinion = new Minion();
        newMinion.desiredPositionX = -1.5f;
        newMinion.desiredPositionZ = 3.5f;

        Transform minionTransform = (Transform)Instantiate(minion1, new Vector3(newMinion.desiredPositionX, 1.0f, newMinion.desiredPositionZ), Quaternion.identity);
        newMinion.setTransform(minionTransform);
        newMinion.initBars();
        MinionAnimations minionAnimations = (MinionAnimations)minionTransform.GetComponent(typeof(MinionAnimations));
        minionAnimations.setDesiredLocation(new Vector3(newMinion.desiredPositionX, 1.0f, newMinion.desiredPositionZ));
        FieldOfView fieldOfView = ((FieldOfView)minionTransform.Find("mob").GetComponent(typeof(FieldOfView)));
    }
}