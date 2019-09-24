using UnityEngine;
using System.Collections;
using System;

public class alignJRD : ExperimentTask {

	public GameObject forwardDirection; // 04/12/2016 MJS
	public GameObject jrdRig; // 04/12/2016 MJS

	public override void startTask () {
		TASK_START();
	}	

	public override void TASK_START()
	{
		base.startTask();
		
		if (!manager) Start();
				
		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}

		jrdRig.transform.localEulerAngles = forwardDirection.transform.localEulerAngles; // 04/12/2016 MJS
	}

	public override bool updateTask () {
		return true;
	}
	public override void endTask() {
		TASK_END();
	}
	
	public override void TASK_END() {
		base.endTask();
	}
}
