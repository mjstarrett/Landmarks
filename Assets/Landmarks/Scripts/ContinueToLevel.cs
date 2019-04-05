using UnityEngine;
using System.Collections;

public class ContinueToLevel : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return))
		{
			Application.LoadLevel (PlayerPrefs.GetString("levelName"));
		}
	}
}
