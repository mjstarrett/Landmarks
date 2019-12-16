using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_CreatePlaceHolders : ExperimentTask
{

    public ObjectList targetObjectList;
    public float placeholderSpacing = 10.0f;

    private GameObject placeholder;

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        for (int i = 0; i < targetObjectList.objects.Count; i++)
        {
            placeholder = new GameObject("PlaceHolder");
            var instance = Instantiate(placeholder, transform.position, Quaternion.identity);
            instance.transform.parent = transform;
            instance.transform.localPosition = new Vector3(0.0f, 0.0f, -1 * i * placeholderSpacing);
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
