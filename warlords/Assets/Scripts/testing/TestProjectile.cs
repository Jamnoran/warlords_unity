using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour {

    public GameObject projectilePrefab;
    public GameObject spawnPoint;
    public GameObject target;
    public float timeToTarget = 1.0f;
    public GameObject spellPrefab;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("v"))
        {
            var projectile = (GameObject)Instantiate(
                projectilePrefab,
                spawnPoint.transform.position,
                spawnPoint.transform.rotation);
            Projectile projectileScript =  ((Projectile) projectile.GetComponent(typeof(Projectile)));
            projectileScript.setTarget(target);
            projectileScript.timeToReachGoal = timeToTarget;
            projectileScript.source = spawnPoint;
        }
        if (Input.GetKeyDown("c"))
        {
            GameObject spell = Instantiate(spellPrefab, spawnPoint.transform.position, Quaternion.identity);
            //GameObject spell = Instantiate(spellPrefab, target.transform.position, Quaternion.identity);
            FollowGameObject followGameOjbect = ((FollowGameObject)spell.GetComponent(typeof(FollowGameObject)));
            //followGameOjbect.setObjectToLookAt(target.transform);
            //followGameOjbect.setObjectToFollow(target.transform);
        }
	}
}
