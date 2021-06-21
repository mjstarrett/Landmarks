/*
    LM_CompareLists
    Takes one game object (parent of objects you want listed and compared), and
    a string input for the name of the target to compare. If the string does not 
    exist, it is created and saved for future scenes with dontdestroyonload.   

    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behavior
{
    DeleteFromList1,
    AddToComparisonList,
}

public class LM_CompareLists : ExperimentTask
{

    [Header("Task-specific Properties")]

    public string list1ParentName = "";
    public GameObject list1Parent;
    public string comparisonListObjectName = "UsedTargets";
    private GameObject comparisonListParent;
    public Behavior behavior;

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        if (list1Parent == null && list1ParentName != "")
        {
            list1Parent = GameObject.Find(list1ParentName);
        }
    }


    public override bool updateTask()
    {
        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }
        else
        {
            CompareLists();
            return true;
        }
        
    }


    public override void endTask()
    {
        TASK_END();
    }


    public override void TASK_END()
    {
        base.endTask();

        
    }

    public void CompareLists()
    {
        // Create the comparison list object if it doesn't exist
        comparisonListParent = GameObject.Find(comparisonListObjectName);
        if (comparisonListParent == null)
        {
            comparisonListParent = new GameObject(comparisonListObjectName);
            DontDestroyOnLoad(comparisonListParent); // keep this object for subsequent levels
        }

        // Make sure any children of our comparisonlistobject are active
        foreach (Transform child in comparisonListParent.transform)
        {
            child.gameObject.SetActive(true);
        }


        // ---------------------------------------------------------------------
        // Handle Behavior after comparing
        // ---------------------------------------------------------------------

        // Create a list of the comparison items and populate it
        List<string> comparisonList = new List<string>();
        foreach (Transform child in comparisonListParent.transform)
        {
            comparisonList.Add(child.gameObject.name);
        }


        // Destroy any objects in List1 that appear in the comparison list
        if (behavior == Behavior.DeleteFromList1)
        {
            int destroyed = 0;
            foreach (Transform child in list1Parent.transform)
            {
                // Debug.Log(child.name);
                if (comparisonList.Contains(child.gameObject.name))
                {
                    Destroy(child.gameObject);
                    Debug.Log(child.name + "should now be destroyed from " + list1Parent.name);
                    destroyed++;
                }
            }
            // Debug.Log(destroyed + " objects destroyed during " + this.name);
        }
        // Add any new items from list1 to the comparisonlist
        else if (behavior == Behavior.AddToComparisonList)
        {
            foreach (Transform child in list1Parent.transform)
            {
                if (!comparisonList.Contains(child.gameObject.name))
                {
                    GameObject childcopy = Instantiate<GameObject>(child.gameObject);
                    childcopy.transform.SetParent(comparisonListParent.transform);
                    childcopy.name = child.gameObject.name;
                    childcopy.SetActive(false);
                }
            }
        }

        // Make sure any children of our comparisonlistobject are inactive so they don't show up in any subsequent levels
        foreach (Transform child in comparisonListParent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}