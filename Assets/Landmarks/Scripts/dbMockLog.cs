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

public class dbMockLog : dbLog  {


    // log object for running in editor and debugging
    
	public dbMockLog(string filename) {
		Debug.Log("Open file:" + filename);
	}
	
	public override void close()
	{
		Debug.Log("Close file");
	}
	
	public override void log(string msg, int level) {
		if ( level < 2 ) {
			Debug.Log(msg);
		}
	}

}
