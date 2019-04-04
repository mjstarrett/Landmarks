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
using System;
using System.IO;
using System.Collections.Generic;


public class dbPlaybackLog : dbLog {

    private StreamReader logfile;
    //private List<string> actions;
        private List<string[]> actions;
    //private List<List<string>> actions = new List<List<string>>();
    public long start_time = 0;
    private long playback_time = 0;
    private string[] next_action;
    private int index = 0;
    
    public override long PlaybackTime() {
		return Int64.Parse(next_action[0]) - start_time;
	}


	public dbPlaybackLog(string filename) {
		workingFile = filename;
		logfile = new StreamReader ( workingFile );

		
		//actions = getActions();
		
		//actions = new List<List<string>>();
		actions = new List<string[]>();
	    string[] actiondefs;
	    //List<string> actiondefs = new List<string>();
	             
		string line = logfile.ReadLine();
        while (line != null) {
        	actiondefs = line.Split(new char[] {'\t'});
        	actions.Add(actiondefs);
			line = logfile.ReadLine();
        }
        
        
		//next_action = actions[index];
		//NextAction();
		//string[] actiondefs = next_action.Split(new char[] {'\t'});
		//Debug.Log(actiondefs[0]);
		start_time = Int64.Parse(actions[0][0]);
	}

	public override string[] NextAction() {
		next_action = actions[index];
		index++;
		if (index >= actions.Count) {
			return null;
		} else {
			return next_action;
		}
	}
/*	
	public List<string> getActions() {
	    //ArrayList actions = new ArrayList();
	    
	    List<string> all = new List<string>(); 
	    
	    actions = new List<List<string>>();
	    string[] actiondefs;
	    //List<string> actiondefs = new List<string>();         
		string line = logfile.ReadLine();
        while (line != null) {
        	actiondefs = line.Split(new char[] {'\t'});
        	actions.Add(actiondefs);
			line = logfile.ReadLine();
        }
        
        return all;
        
        
        //foreach (string action in actions)
	}
*/	
	public override void close()
	{
		logfile.Close();	
	}
	
	public override void log(string msg, int level) {
		//ignore logs		
	}
}
