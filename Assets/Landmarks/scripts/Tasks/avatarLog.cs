using UnityEngine;
using System.Collections;

public class avatarLog : MonoBehaviour {

	[HideInInspector] public bool navLog = false;
	private Transform avatar;
	private Transform cameraCon;
	private Transform cameraRig;

	private GameObject experiment;
	private dbLog log;
	private Experiment manager;
	
	public GameObject player;
	public GameObject camerarig;

	void Start () {

		cameraCon =player.transform as Transform;
		cameraRig =camerarig.transform as Transform;

		experiment = GameObject.FindWithTag ("Experiment");
		manager = experiment.GetComponent("Experiment") as Experiment;
		log = manager.dblog;
		avatar = transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (navLog){
			//print("AVATAR_POS	" + "\t" +  avatar.position.ToString("f3") + "\t" + "AVATAR_Body " + "\t" +  cameraCon.localEulerAngles.ToString("f3") +"\t"+ "AVATAR_Head " + cameraRig.localEulerAngles.ToString("f3"));
			log.log("AVATAR_POS	" + "\t" +  avatar.position.ToString("f3") + "\t" + "AVATAR_Body " + "\t" +  cameraCon.localEulerAngles.ToString("f3") +"\t"+ "AVATAR_Head " + cameraRig.localEulerAngles.ToString("f3"),2);
			// ^^^^ Logs the position (X Y Z), Body Rotations (only the middle value should change), and HMD rotation (Pitch, Yaw, Roll)
		}

	}
}
