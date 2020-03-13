/*
    Copyright (C) 2010  Jason Laczko

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowItemTask : ExperimentTask {

	public TextAsset itemText;
	public Color text_color;
	public Font instructionFont;
	public int instructionSize = 12;
	
	public Texture itemImage;
	
	public TextList items;
	private string current;
	
	
	public string itemName;
		
	private Image gui;
	
	void OnDisable ()
	{
		if (gui)
			DestroyImmediate (gui.gameObject);
	}
	
	public override void startTask () {
		TASK_START();
	}	

	public override void TASK_START()
	{
		
		base.startTask();
		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}
		
		if (!manager) Start();
		if (!gui)
	    {	       			
    		GameObject sgo = new GameObject("Item Display");
    		sgo.AddComponent<Image>();
			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0,0,0);
			gui = sgo.GetComponent<Image>();
			//gui.pixelInset = new Rect( 20, Screen.height + 100, 500/4, 600/4);
			//gui.pixelInset = new Rect( (Screen.width-500)/2, 0, 500, 600);	  
			gui.transform.position = Vector3.zero;
        	gui.transform.localScale = Vector3.zero;
	    }
	    
	    current = items.currentString().Trim();
	    gui = (Image)Resources.Load("Items/"+current, typeof(Texture2D));	
		hud.setMessage("You delivered " + current);
	
		
	}
	// Update is called once per frame
	public override bool updateTask () {
		
		if (skip) return true;
		if ( Experiment.Now() - task_start >= interval)  {
	//	if (Input.GetButtonDown("Return")) {
	//		log.log("INPUT_EVENT	dismiss	1",1 );
	        return true;
	    }
	    return false;
	}
	
	public override void endTask() {
		TASK_END();
	}
	
	public override void TASK_END() {
		if (skip) return;
		base.endTask();
		items.incrementCurrent();
		current = items.currentString();
		if (current == null) skip = true;
		hud.setMessage("");		
		if (gui)
			DestroyImmediate (gui.gameObject);

	}
}