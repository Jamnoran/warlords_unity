using UnityEngine;
using System.Collections.Generic;
using Assets.scripts.vo;

public class HideWalls : MonoBehaviour
{
    //The camera to shoot the ray from
    public Transform camera;

    //List of all objects that we have hidden.
    public List<Transform> hiddenObjects;

    //Layers to hide
    public LayerMask layerMask;

    private void Start()
    {
        //Initialize the list
        hiddenObjects = new List<Transform>();
    }

    void Update()
    {
        foreach (Hero hero in getGameLogic().getHeroes())
        {

            if (hero.trans != null)
            {
                //Find the direction from the camera to the player
                Vector3 direction = hero.trans.position - camera.position;

                //The magnitude of the direction is the distance of the ray
                float distance = direction.magnitude;

                //Raycast and store all hit objects in an array. Also include the layermaks so we only hit the layers we have specified
                RaycastHit[] hits = Physics.RaycastAll(camera.position, direction, distance, layerMask);

                //Go through the objects
                for (int i = 0; i < hits.Length; i++)
                {
                    Transform currentHit = hits[i].transform;

                    //Only do something if the object is not already in the list
                    if (!hiddenObjects.Contains(currentHit))
                    {
                        //Add to list and disable renderer
                        hiddenObjects.Add(currentHit);
                        if (currentHit.GetComponent<Renderer>() != null)
                        {
                            currentHit.GetComponent<Renderer>().enabled = false;
                        }
                    }
                }

                //clean the list of objects that are in the list but not currently hit.
                for (int i = 0; i < hiddenObjects.Count; i++)
                {
                    bool isHit = false;
                    //Check every object in the list against every hit
                    for (int j = 0; j < hits.Length; j++)
                    {
                        if (hits[j].transform == hiddenObjects[i])
                        {
                            isHit = true;
                            break;
                        }
                    }

                    //If it is not among the hits
                    if (!isHit)
                    {
                        //Enable renderer, remove from list, and decrement the counter because the list is one smaller now
                        Transform wasHidden = hiddenObjects[i];
                        if (wasHidden.GetComponent<Renderer>() != null)
                        {
                            wasHidden.GetComponent<Renderer>().enabled = true;
                            hiddenObjects.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }
    }

    public void clearHiddenObjects()
    {
        hiddenObjects.Clear();
        hiddenObjects = new List<Transform>();
    }


    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

}