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


public class dbLog {

    protected long microseconds = 1;
    protected string workingFile = "";
    private StreamWriter logfile;

	public dbLog(string filename) {
		workingFile = filename;
		logfile = new StreamWriter ( workingFile );
	}
	
	public dbLog() {
		//openNew(filename);
	}
	
	public virtual void close()
	{
		logfile.Close();	
	}
	
	public virtual string[] NextAction() {
		return null;
	}
	public virtual long PlaybackTime() {
		return 0;
	}
	
	public virtual void log(string msg, int level) {
		
	    long tick = DateTime.Now.Ticks;
        //long seconds = tick / TimeSpan.TicksPerSecond;
        long milliseconds = tick / TimeSpan.TicksPerMillisecond;
        microseconds = tick / 10;
        //Debug.Log(milliseconds);
        //Debug.Log(Time.frameCount + ": " + Event.current);
        
		logfile.WriteLine( milliseconds + "\t" + msg );
	}

    // MJS function to cleanly log info with no prefixes
    public virtual void Write(string msg)
    {
        logfile.WriteLine(msg);
    }
}
