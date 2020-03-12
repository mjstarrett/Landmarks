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
using System.Linq;


public class LM_PermutedList : ExperimentTask
{
    [Header("Task-specific Properties")]
    public ObjectList listToPermute;
    public int subset = 3;
    public bool shuffle = true;

    //[HideInInspector]
    public List<List<GameObject>> permutedList = new List<List<GameObject>>();


    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();


        // Generate permutations
        permutedList = Permute(listToPermute.objects, subset).ToList();


        // Shuffle if necessary
        if (shuffle)
        {
            FisherYatesShuffle(permutedList);
        }


        for (int i = 0; i < subset; i++)
        {
            var ol = new GameObject();
            
            ol.AddComponent<ObjectList>();
            var thing = Instantiate(ol, transform);
            thing.name = this.name + "_subset" + i;

            foreach (var entry in permutedList)
            {
                thing.GetComponent<ObjectList>().objects.Add(entry[i]);
            }
            Destroy(ol);
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



//------------------------------------------------------------------------------


    // -------------------------------------------------------------------------
    // Factorization Helper ----------------------------------------------------
    // -------------------------------------------------------------------------


    // calculate the factorial of an integer (3! = 1*2*3 = 6)
    public int Factorize(int number)
    {
        int currentTotal = number;

        while (number > 1)
        {
            currentTotal *= number - 1;
            number--;
        }
        return currentTotal;
    }



    // -------------------------------------------------------------------------
    // Permutation Helpers -----------------------------------------------------
    // -------------------------------------------------------------------------

    // iterators to return permutations - www.interact-sw.co.uk/iangblog/2004/09/16/permuterate
    public static List<List<T>> Permute<T>(List<T> list, int count)
    {
        var output = new List<List<T>>();
        if (count == 0)
        {
            var entry = new T[0].ToList();
            output.Add(entry);
        }
        else
        {
            int startingElementIndex = 0;
            var entry = new List<T>();
            foreach (T startingElement in list)
            {
                List<T> remainingItems = AllExcept(list, startingElementIndex).ToList();

                foreach (List<T> permutationOfRemainder in Permute(remainingItems, count - 1))
                {
                    var catIn1 = new T[] { startingElement }.ToList();
                    var catIn2 = permutationOfRemainder.ToList();

                    entry = Concat<T>(catIn1, catIn2);
                    output.Add(entry);
                }
                startingElementIndex += 1;

            }

        }

        return output;
    }

    // Enumerates over contents of both lists.
    public static List<T> Concat<T>(List<T> a, List<T> b)
    {
        var output = new List<T>();
        foreach (T item in a)
        {

            output.Add(item);
        }
        foreach (T item in b)
        {
            output.Add(item);
        }
        return output;
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
    // www.interact-sw.co.uk/iangblog/2004/09/16/permuterate



    // -------------------------------------------------------------------------
    // Shuffle Helper ----------------------------------------------------------
    // -------------------------------------------------------------------------

    public static void FisherYatesShuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}


