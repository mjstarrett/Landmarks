using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CONFIG_Navigation : MonoBehaviour
{

    public bool movePlayerOnStart;
    public bool movePlayerOnEnd;

    [HideInInspector] private GameObject startMoveTask;
    [HideInInspector] private GameObject endMoveTask;

    // Start is called before the first frame update
    void Awake()
    {

        //----------------------------------------------------------------------
        // This code handles alterations to spawn/start location functions
        //----------------------------------------------------------------------

        // get our movetasks within this parent object Navigation task
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "MovePlayerToStart") startMoveTask = child.gameObject;
            if (child.gameObject.name == "ReturnPlayerToStart") endMoveTask = child.gameObject;
        }

        // If we want to move the avatar at beginning AND end:
        if (movePlayerOnStart && movePlayerOnEnd)
        {
            startMoveTask.GetComponent<MoveSpawn>().skip = false;
            startMoveTask.GetComponent<MoveSpawn>().canIncrementLists = false;

            endMoveTask.GetComponent<MoveSpawn>().skip = false;
            endMoveTask.GetComponent<MoveSpawn>().canIncrementLists = true;
        }
        // If we don't want to move the avatar before OR after navigation
        else if (!movePlayerOnStart && !movePlayerOnEnd)
        {
            startMoveTask.GetComponent<MoveSpawn>().skip = true;
            startMoveTask.GetComponent<MoveSpawn>().canIncrementLists = false;

            endMoveTask.GetComponent<MoveSpawn>().skip = true;
            endMoveTask.GetComponent<MoveSpawn>().canIncrementLists = true;
        }
        // if we want to move the avatar before BUT NOT after
        else if (movePlayerOnStart & !movePlayerOnEnd)
        {
            startMoveTask.GetComponent<MoveSpawn>().skip = false;
            startMoveTask.GetComponent<MoveSpawn>().canIncrementLists = true;

            endMoveTask.GetComponent<MoveSpawn>().skip = true;
            endMoveTask.GetComponent<MoveSpawn>().canIncrementLists = false;
        }
        // if we DON'T want to move the avatar before, BUT DO want to move it after
        else if (!movePlayerOnStart && movePlayerOnEnd)
        {
            startMoveTask.GetComponent<MoveSpawn>().skip = true;
            startMoveTask.GetComponent<MoveSpawn>().canIncrementLists = false;

            endMoveTask.GetComponent<MoveSpawn>().skip = false;
            endMoveTask.GetComponent<MoveSpawn>().canIncrementLists = true;
        }




    }

}
