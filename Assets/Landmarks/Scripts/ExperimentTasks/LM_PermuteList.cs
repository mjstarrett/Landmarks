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

    [HideInInspector]
    public List<List<string>> output = new List<List<string>>();

    private int totalPermuations;

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // Throw an error if subset entered is equal to or greater than the permuted list length
        if (subset >= listToPermute.objects.Count)
        {
            Debug.LogError("Subset cannot be equal to or greater in length than the list to permute");
        }


        // Determine how long the output list should be ------------------------

        // Combinations or Permutations (! denotes a factorial -- eg, 3! = 1*2*3 = 6)
        if (returnCombinations)
        {
            // combinations = listLength! / subset! * (listLength - subset)!
            totalPermuations = Factorize(listToPermute.objects.Count) /
                                Factorize(subset) * Factorize(listToPermute.objects.Count - subset);
        }
        // or permutations (permutations = listLength! / (listLength - subset)!)
        else totalPermuations = Factorize(listToPermute.objects.Count) /
                            Factorize(listToPermute.objects.Count - subset);

        Debug.Log("Permutations/Combinations: " + totalPermuations.ToString());


        // Generate permutations/combinations ----------------------------------



        // populate each possible combination/permuation
        
        // Did the user ask for combinations?
        if (returnCombinations)
        {
                
        }

        // If not, return permuations
        else
        {
            List<string> entry = new List<string>(); // create a new list for this permutation/combination

            for (int i = 0; i < subset; i++)
            {
                var listCopy = listToPermute.objects; // copy the list


                foreach (var item in listCopy)
                {
                    
                }
            }
            

            


            //while (entry.Count < subset)
            //{

            //}

            // add an item (1)
            foreach (var item in listToPermute.objects)
            {
                entry.Add(item.name); // add the first item of this entry

                while (entry.Count < subset)
                {
                    foreach (var nextItem in listToPermute.objects)
                    {
                        if (!entry.Contains(nextItem.name))
                        {
                            entry.Add(nextItem.name);
                        }
                    }
                }


                output.Add(entry); // add our entry list as an entry to the output list
                entry.Clear(); // clear the entry so we can start the next one
                Debug.Log("next permuation/combination");
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



    // calculate the factorial of an integer (3! = 1*2*3 = 6)
    public int Factorize(int number)
    {
        int currentTotal = number;

        while (number > 1)
        {
            currentTotal *= number-1;
            number--;
        }
        return currentTotal;
    }


    // Permutation Recursion
    public List<List<string>> Permute(List<GameObject> list, int subset)
    {
        var listCopy = list;
        var result = new List<List<string>>();

        var entry = new List<string>();

        
        

        return result;
    }
}

