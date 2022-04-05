using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_DecisionPoint : MonoBehaviour
{
    public string initialChoice;
    public string currentChoice;
    public int totalChoices = 0;

    public void HandleDecision(Collider choice)
    {
        if (totalChoices == 0) initialChoice = choice.name;

        // Don't count passing back through to change their mind (backtracking)
        if (choice.name != currentChoice)
        {
            currentChoice = choice.name;
            totalChoices++;
        }
        
    }

    public void ResetDecisionPoint()
    {
        initialChoice = "";
        currentChoice = "";
        totalChoices = 0;
    }
}

