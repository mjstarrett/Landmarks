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
    public IEnumerable<IEnumerable<string>> output = new List<List<string>>();

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
        else totalPermuations = Factorize(listToPermute.objects.Count) / Factorize(listToPermute.objects.Count - subset);

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
            var objectNames = new List<string>();
            foreach (var item in listToPermute.objects)
            {
                objectNames.Add(item.name);
            }
            output = Permute(objectNames, subset);

            var count = 1;
            foreach (var thing in output)
            {
                Debug.Log("Permuation " + count);
                foreach (var thing2 in thing)
                {
                    Debug.Log(thing2);
                }
                Debug.Log("==============================");
                count++;
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


    //// Permutation Recursion
    //public List<List<string>> Permute(List<GameObject> list, int sampleSize)
    //{
    //    var result = new List<List<string>>();

    //    var totalPermuations = Factorize(list.Count) / Factorize(list.Count - sampleSize);



    //    // get all iterations for each entry
    //    foreach (var item in list)
    //    {
    //        // there should be this many iterations per list item for permutations
    //        for (int i = 0; i < totalPermuations / list.Count; i++)
    //        {
    //            var entry = new List<string>(); // create an empty list for this entry

    //            entry.Add(item.name); // add the first item to the entry

    //            // get the remaining number of unique entries needed to complete the subset
    //            for (int j = 0; j < subset-1; j++)
    //            {
    //                foreach (var nextItem in list)
    //                {

    //                }
    //            }

    //        }
    //    }


    //    return result;
    //}











    public static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> list, int count)
    {
        if (count == 0)
        {
            yield return new T[0];
        }
        else
        {
            int startingElementIndex = 0;
            foreach (T startingElement in list)
            {
                IEnumerable<T> remainingItems = AllExcept(list, startingElementIndex);

                foreach (IEnumerable<T> permutationOfRemainder in Permute(remainingItems, count - 1))
                {
                    yield return Concat<T>(
                        new T[] { startingElement },
                        permutationOfRemainder);
                }
                startingElementIndex += 1;
            }
        }
    }

    // Enumerates over contents of both lists.
    public static IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
    {
        foreach (T item in a) { yield return item; }
        foreach (T item in b) { yield return item; }
    }

    // Enumerates over all items in the input, skipping over the item
    // with the specified offset.
    public static IEnumerable<T> AllExcept<T>(IEnumerable<T> input, int indexToSkip)
    {
        int index = 0;
        foreach (T item in input)
        {
            if (index != indexToSkip) yield return item;
            index += 1;
        }
    }



}

