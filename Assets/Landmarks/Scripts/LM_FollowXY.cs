using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_FollowXY : MonoBehaviour
{
    [Tooltip("What should this object's transform to follow?")]
    public Transform leader;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //-------------------------------------------------
    void FixedUpdate()
    {
        float distanceFromFloor = Vector3.Dot(leader.localPosition, Vector3.up);
        //capsuleCollider.height = Mathf.Max(capsuleCollider.radius, distanceFromFloor);
        transform.localPosition = leader.localPosition - 0.5f * distanceFromFloor * Vector3.up;

        transform.localEulerAngles = new Vector3(0f, leader.localEulerAngles.y, 0f);    
    }
}
