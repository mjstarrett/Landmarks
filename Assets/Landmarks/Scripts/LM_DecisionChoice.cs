using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// NOTE must be a child of an LM_DecisionChoice component

public class LM_DecisionChoice : MonoBehaviour
{
    private LM_DecisionPoint nexus;

    // Start is called before the first frame update
    void Start()
    {
        nexus = transform.parent.GetComponent<LM_DecisionPoint>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name ==
            GameObject.FindWithTag("Player").GetComponentInChildren<LM_PlayerController>().collisionObject.gameObject.name)
        {
            nexus.HandleDecision(GetComponent<Collider>()); // feed this collider object up to the handler in LM_DecisionPoint.cs
        }
    }
}
