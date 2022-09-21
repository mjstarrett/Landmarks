using UnityEngine;
using System.Collections;

public class avatarLog : MonoBehaviour {

	[HideInInspector] public bool navLog = false;
    //private Transform avatar;
    private Transform body;
    private Transform head;

    private dbLog log;
    private Experiment manager;
    private LM_PlayerController controller;

    void Start () 
    {
        manager = FindObjectOfType<Experiment>().GetComponent<Experiment>();
		log = manager.dblog;
		//avatar = transform;

        controller = manager.player.GetComponent<LM_PlayerController>();
        body = controller.collisionObject.transform;
        head = controller.cam.transform;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        // Log the name of the tracked object, it's body position, body rotation, and camera (head) rotation
		if (navLog){
            //print("AVATAR_POS	" + "\t" +  avatar.position.ToString("f3") + "\t" + "AVATAR_Body " + "\t" +  cameraCon.localEulerAngles.ToString("f3") +"\t"+ "AVATAR_Head " + cameraRig.localEulerAngles.ToString("f3"));
            log.log("Avatar: \t" + controller.name + "\t" +
                    "Body Position (xyz): \t" + body.position.x + "\t" + body.position.y + "\t" + body.position.z + "\t" +
                    "Body Rotation (xyz): \t" + body.eulerAngles.x + "\t" + body.eulerAngles.y + "\t" + body.eulerAngles.z + "\t" +
                    "Camera Position (xyz): \t" + head.position.x + "\t" + head.position.y + "\t" + head.position.z + "\t" +
                    "Camera Rotation   (xyz): \t" + head.eulerAngles.x + "\t" + head.eulerAngles.y + "\t" + head.eulerAngles.z + "\t"
                    , 1);
        }
	}
}
