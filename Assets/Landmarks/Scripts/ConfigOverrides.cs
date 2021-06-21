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
using System;
using System.IO;
using System.Reflection;

public class ConfigFormatException: System.Exception {}
public class ConfigKeyFormatException: System.Exception {}
public class GameObjectNullException: System.Exception {}
public class ScriptNullException: System.Exception {}
public class MissingKeyException: System.Exception {}

public class ConfigOverrides {

	public static void parse(string filepath, dbLog log) {
		
		// Debug.Log("Filepath:\t" + filepath);
		if (File.Exists(filepath)) {
			
			StreamReader file = new StreamReader ( filepath );
			string line = file.ReadLine();
			
			int lineNum = 1;
			
	        while (line != null) {
	        	line = line.Trim();
	        	if (line.IndexOf("#") != 0 && line.Length > 1) { 

						ConfigOverrides.set_keyvalue(line,"Line: " + lineNum, log);							
		        	}		 
	        	line = file.ReadLine();
	        	lineNum++;
	        }
		} 
		
	}
	
	public static void set_keyvalue(string line, string lineNum, dbLog log ) {
		
			string go = "";
			string code = "";
			string key = "";
			string val = "";
			string typestring = "";
			string[] keyvalue = new string[1];
			string[] parts = new string[1];
		
		        try { 
	        		keyvalue = line.Split(new char[] {'='},StringSplitOptions.RemoveEmptyEntries);	
	        		if (keyvalue.Length != 2) throw new ConfigFormatException();
		
			        	val = keyvalue[1].Trim();	
		        		
		        		parts = keyvalue[0].Trim().Split(new char[] {'.'},StringSplitOptions.RemoveEmptyEntries);
		        		if (parts.Length != 3) throw new ConfigKeyFormatException();

						
							
							
		        		go = parts[0];
		        		code = parts[1];
		        		key = parts[2];
	        		 		       			
	        			GameObject taskObject = GameObject.Find(go);
	        			if (!taskObject) throw new GameObjectNullException();
	        			
						Component script = taskObject.GetComponent(code) as Component;
	        			if (!script) throw new ScriptNullException();	

						Type valType;
						BindingFlags flags;
					 	FieldInfo fi = script.GetType().GetField(key,  BindingFlags.Instance | BindingFlags.Public);
					 	
					 	// don't care if key is a field or property
					 	if (fi ==  null )  {
					 		PropertyInfo pi = script.GetType().GetProperty(key,  BindingFlags.Instance | BindingFlags.Public);
					 		if (pi ==  null ) {
					 			throw new MissingKeyException();
					 		} else {
						 		typestring = pi.PropertyType.ToString();
						 		valType = pi.PropertyType;	
						 		flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty;
					 		}
					 	} else {
					 		valType = fi.FieldType;
						 	typestring = fi.FieldType.ToString();	
						 	flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField;	
					 	}			          
          
          				// try to convert value to correct type
          				object converted = null;
          				if (valType.IsEnum) {
          					converted = Enum.Parse(valType, val, true);
          				} else {
          					converted = Convert.ChangeType(val, valType);
          				}
          				if (converted == null) throw new InvalidCastException();
          				
          				//set via reflection
						script.GetType().InvokeMember(key, flags, null, script, new object[] { converted });
						log.log("CONFIG_SET	" + keyvalue[0].Trim() + "\t" + keyvalue[1].Trim(),1 );
						
											
				} catch (ConfigFormatException) {
					Debug.Log(lineNum + "  Configuration '" + line +  "' has invalid format.\nValid config format is 'Session.Game Object name.Script Type.Variable = value'");
	        	} catch (ConfigKeyFormatException) {
	        		Debug.Log(lineNum + "  Configuration key '" + key.Trim() +  "' has invalid format.\nValid config format is 'Session.Game Object name.Script Type.Variable'");
				} catch (GameObjectNullException) {
        			Debug.Log(lineNum + "  Unable to find game object: " + go);
				} catch (ScriptNullException) {
        			Debug.Log(lineNum + "  Unable to find script: " + code);
				} catch (MissingKeyException) {
					Debug.Log(lineNum + "  Key: " + key  + " Not found in " + go + "." + code );		
				} catch (FormatException) {
					Debug.Log(lineNum + "  Unable to set " + go + "." + code + "." + key + " to " + val + ". Was expecting to be formated for " + typestring);
				} catch (InvalidCastException) {
					Debug.Log(lineNum + "  Unable to convert " + val + " to " + typestring);
				} catch (ArgumentException e) {
					Debug.Log("Line: " + lineNum + "  " + e.ToString());
					
				}

		}
}
