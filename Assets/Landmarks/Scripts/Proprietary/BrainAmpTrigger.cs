using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainAmpTrigger : BrainAmpManager
{
    public string label; // name prefix for unique triggers
    public bool onStart = true; // mark a unique trigger at TASK_START
    public bool onEnd = true; // mark a unique trigger at TASK_END

    private ExperimentTask task; // object attached must have a component that inherits from ExpTask

    // Start is called before the first frame update
    void Start()
    {
        task = GetComponent<ExperimentTask>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
