using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_ScaledEnvironment : MonoBehaviour
{
    public GameObject startLocationsParent; // where are the start locations nested?

    private GameObject listLargePlayerStarts; // create an empty objectlist that can be grabbed by any ExperimentTask subclss
    private GameObject listSmallPlayerStarts;


    // Start is called before the first frame update
    void Awake()
    {
        listLargePlayerStarts = new GameObject(); // create a new empty gameobject
        listLargePlayerStarts.name = "ListLargePlayerStarts";
        listLargePlayerStarts.transform.parent = GameObject.Find("InitializeExperiment").transform;
        listLargePlayerStarts.AddComponent<ObjectList>(); // add an objectlist to our gameobject
        listLargePlayerStarts.GetComponent<ObjectList>().EndListBehavior = EndListMode.Loop; // loop the list
        listLargePlayerStarts.GetComponent<ObjectList>().shuffle = true;
        listLargePlayerStarts.GetComponent<ObjectList>().parentObject = startLocationsParent;
        

        listSmallPlayerStarts = new GameObject(); // create new gameobject
        listSmallPlayerStarts.name = "ListSmallPlayerStarts";
        listSmallPlayerStarts.transform.parent = GameObject.Find("InitializeExperiment").transform;
        listSmallPlayerStarts.AddComponent<ObjectList>(); // add an objectlist component
        listSmallPlayerStarts.GetComponent<ObjectList>().EndListBehavior = EndListMode.Loop;
        listSmallPlayerStarts.GetComponent<ObjectList>().shuffle = true;
        listSmallPlayerStarts.GetComponent<ObjectList>().parentObject = GameObject.Find("NavigationStartingLocations"); // these are a necessary part of the Landmarks prefab (LM_Environment)

    }
}
