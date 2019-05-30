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
            foreach (Transform child in list1Parent.transform)
            {
                if (comparisonList.Contains(child.gameObject.name))
                {
                    Destroy(child.gameObject);
                }
            }
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
    }


    public override bool updateTask()
    {
        return true;
    }


    public override void endTask()
    {
        TASK_END();
    }


    public override void TASK_END()
    {
        base.endTask();
    }

}