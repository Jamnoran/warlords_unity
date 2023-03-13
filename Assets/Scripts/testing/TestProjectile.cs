using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour {

    public GameObject projectilePrefab;
    public GameObject spawnPoint;
    public GameObject target;
    public float timeToTarget = 1.0f;
    public GameObject spellPrefab;
    public Vector3 posToLookAt;

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
            //GameObject spell = Instantiate(spellPrefab, spawnPoint.transform.position, Quaternion.identity);
            //GameObject spell = Instantiate(spellPrefab, target.transform.position, Quaternion.identity);
            //FollowGameObject followGameOjbect = ((FollowGameObject)spell.GetComponent(typeof(FollowGameObject)));
            // For projectile
            //followGameOjbect.setObjectToLookAt(target.transform);
            // For cleave
            //followGameOjbect.setObjectToFollow(target.transform);

            Transform animationTransform = handleSpellPositioningAndRotation(spellPrefab.transform, spawnPoint.transform, false, false, true, false, true);
        }
	}





    // prefab: Prefab to animate
    // gameanimation: request to handle
    private Transform handleSpellPositioningAndRotation(Transform prefab, Transform source, bool followTarget, bool lookAt, bool spawnSource, bool friendly, bool towardsPosition)
    {
        Transform animationTransform = null;
        Vector3 targetPos = new Vector3();
        if (!towardsPosition)
        {
            targetPos = target.transform.position;
            if (spawnSource)
            {
                Debug.Log("Spawn source");
                animationTransform = Instantiate(prefab, source.transform.position, Quaternion.identity);
            }
            else if (!spawnSource && !towardsPosition)
            {
                Debug.Log("Spawn target pos");
                animationTransform = Instantiate(prefab, targetPos, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("Spawn transform pos");
            animationTransform = Instantiate(prefab, source.position, source.rotation);
        }

        FollowGameObject followGameOjbect = ((FollowGameObject)animationTransform.GetComponent(typeof(FollowGameObject)));
        if (followTarget)
        {
            Debug.Log("Follow target");
            followGameOjbect.setObjectToFollow(target);
        }
        if (lookAt)
        {
            Debug.Log("Look at");
            followGameOjbect.setObjectToLookAt(target.transform);
        }
        if (towardsPosition)
        {
            Debug.Log("Setting position to Look at");
            followGameOjbect.setPositionToLookAt(posToLookAt);
        }
        return animationTransform;
    }
}
