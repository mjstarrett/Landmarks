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
using System.Collections.Generic;
using System.Linq;

public enum TextListMode
{
	SingleLine,
	MultiLine,
	ObjectNames
}


public class TextList : ExperimentTask {
	
	public TextAsset textsText;
	private List<string> texts;
	public EndListMode EndListBehavior; 

	private int current=0;
	public string currentText = "";
	
	public bool shuffle=true;
	//public int size;
	private TextListMode 	listMode = TextListMode.SingleLine;
		
	public override void startTask () {
		TASK_START();

		string[] parts = textsText.text.Trim().Split(new char[] { '\r','\n' });
		int i = 1;
		string multi = "";
		foreach( string line in parts) {
			
			if (line.Trim().Length > 0) multi = multi + line + "\n";
		    if (line.Trim().Length < 1 && listMode == TextListMode.MultiLine) {
		    	i++;
		    	texts.Add(multi.Trim());
		    	multi = "";
		    } else if (multi.Trim().Length > 0 &&  listMode == TextListMode.SingleLine) {
		    	i++;
		    	texts.Add(multi.Trim());
		    	multi = "";
		    }
		}
		
		i++;
		if (multi.Trim().Length > 0) texts.Add(multi);
		
		string[] tmp = texts.ToArray();
			
		if ( shuffle ) {
			Experiment.Shuffle(tmp);
		}
		
		texts = tmp.ToList();
		//texts = texts.GetRange(0,size);
		foreach( string txt in texts) {
			Debug.Log(txt);
			log.log("TASK_ADD	" + name  + "\t" + this.GetType().Name + "\t" + name  + "\t" + txt,1 );

		}
		
		currentText = currentString();

		
	}	
	
	public override void TASK_ADD(GameObject go, string txt) {
		Debug.Log("ADD  " + txt);
		texts.Add(txt);
	}
	
	public override void TASK_START()
	//public  void Awake()
	{
		base.startTask();
		
		if (!manager) Start();
		texts = new List<string>();

		
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
	
	public override string currentString() {
		if (current >= texts.Count) {
			return null;
		}
		
		return texts[current];
	}
	
	public override void incrementCurrent() {
		current++;
		if (current >= texts.Count && EndListBehavior == EndListMode.Loop) {
			current = 0;
		}
		currentText = currentString();
	}
}
