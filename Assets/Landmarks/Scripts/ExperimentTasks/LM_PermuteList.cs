/*
    LM Dummy
       
    Attached object holds task components that need to be effectively ignored 
    by Tasklist but are required for the script. Thus the object this is 
    attached to can be detected by Tasklist (won't throw error), but does nothing 
    except start and end.   

    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_PermuteList : ExperimentTask
{
    [Header("Task-specific Properties")]
    public ObjectList listToPermute;
    public int subset = 2;
    [Tooltip("Return combinations instead of permutations (ignore order)")]
    public bool returnCombinations;

    private int totalPermuations;

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        try
        {
            // Combinations or Permutations (! denotes a factorial -- eg, 3! = 1*2*3 = 6)
            if (returnCombinations)
            {
                // combinations = listLength! / subset! * (listLength - subset)!
                totalPermuations = factorialRecursion(listToPermute.objects.Count) /
                                    factorialRecursion(subset) * factorialRecursion(listToPermute.objects.Count - subset);
            }
            // or permutations (perms = listLength! / (listLength - subset)!
            totalPermuations = factorialRecursion(listToPermute.objects.Count) /
                               factorialRecursion(listToPermute.objects.Count - subset);
        }
        catch (System.Exception)
        {
            Debug.LogError("Subset cannot be equal to or greater in length than the list to permute");
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



// calculate the factorial of an integer (3! = 1*2*3 = 6)
public int factorialRecursion(int number)
    {
        int currentTotal = number;

        while (number > 1)
        {
            currentTotal *= number-1;
            number--;
        }
        return currentTotal;
    }

}