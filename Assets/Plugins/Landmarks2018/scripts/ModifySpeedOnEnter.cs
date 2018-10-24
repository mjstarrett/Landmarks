using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ModifySpeedOnEnter : MonoBehaviour {

	[Range(0.1f,10.0f)]
	public float speedMultiplier = 1;

	FirstPersonController desktopPlayer;

	private float baseSpeed;

	// Use this for initialization
	void Awake () {
		desktopPlayer = GameObject.FindObjectOfType<FirstPersonController> ();
		baseSpeed = desktopPlayer.m_WalkSpeed;
	}


	void OnTriggerEnter(Collider other) 
	{
		if (other.CompareTag ("Player")) 
		{
			desktopPlayer.m_WalkSpeed = desktopPlayer.m_WalkSpeed * speedMultiplier;
		}
	}

	void OnTriggerExit(Collider other) 
	{
		if (other.CompareTag ("Player")) 
		{
			desktopPlayer.m_WalkSpeed = baseSpeed;
		}
	}
}
