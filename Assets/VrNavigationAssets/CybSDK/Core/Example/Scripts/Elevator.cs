using UnityEngine;
using System.Collections;
using CybSDK;

public class Elevator : MonoBehaviour 
{
    public bool goDown = false;
    public float yOffset = 5f;
    public float moveSpeed = 3f;
    //
    public float waitTime = 0f;
    //
    public Vector3 startPos = Vector3.zero;

	// Use this for initialization
	void Awake () 
    {
        this.startPos = this.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate ()     // fixed update to avoid character physics errors
    {
	    if(this.waitTime <= 0f)
        {
            this.transform.position = new Vector3(this.startPos.x, Mathf.Clamp(this.transform.position.y + ((goDown == true ? -1f : 1f) * moveSpeed * Time.deltaTime), this.startPos.y, this.startPos.y + yOffset), this.startPos.z);

            if(this.transform.position.y == this.startPos.y || this.transform.position.y == (this.startPos.y + yOffset))
            {
                this.goDown = !this.goDown;
                this.waitTime = 5f;
            }
        }
        else
        {
            this.waitTime -= Time.deltaTime;

            // Trigger Haptics when we switch move dir
            if (this.waitTime <= 0f)
            {
                // If an haptic emitter is attached, we trigger the haptic of the elevator
                CVirtHapticEmitter hapticEmitter = this.GetComponent<CVirtHapticEmitter>();
                if (hapticEmitter != null)
                    hapticEmitter.Play();
            }
        }
	}
}
