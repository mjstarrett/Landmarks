using UnityEngine;
using System.Collections;
using CybSDK;

public class ElevatorHaptics : MonoBehaviour
{
    public bool playingHaptics = false;
    //
    public Elevator elevator = null;
    public CVirtHapticEmitter hapticEmitter = null;

	// Use this for initialization
	void Awake ()
    {
        this.hapticEmitter.Stop();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (this.playingHaptics != null && this.playingHaptics != (this.elevator.waitTime <= 0f))
        {
            this.playingHaptics = (this.elevator.waitTime <= 0f);
            //
            if (this.playingHaptics == true)
            {
                this.hapticEmitter.Play();
            }
            else
            {
                this.hapticEmitter.Stop();
            }
        }
	}
}
