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
using System.Diagnostics;

public class Startup_windows : MonoBehaviour {
	
    public GUISkin thisMetalGUISkin;
    public Texture logo;
    public Texture pulldown;
    public string[] levels;

	private Config config;

	private  Resolution[] resolutions;
	private GUIContent currentSelection;
	private int toolbarInt = 0;
	private string[] toolbarStrings = new string[] {"New Experiment", "Current Experiments"};
	
	private bool showList = false;
	private int listEntry = 0;
	private GUIContent[] reslist;
	private GUIContent[] levelList;
	private int currentLevelSelection = 0;
	private int newCurrentLevelSelection = 0;
	private int currentSubjectSelection = 0;
	private int newCurrentSubjectSelection = 0;
	
	
	private string[] subjects;
	private int subjectEntry = 0;
	private GUIContent[] subjectslist;
	
	private GUIStyle errorStyle;
	private GUIStyle popupStyle;
	private GUIStyle listStyle;
	private GUIStyle listSelectedStyle;
	private bool picked = true;
	
	
	
	
	
	private string selectSession = "Select Session";		        				
	private bool sesspicked = true;
	private int sessEntry = 0;
	private bool showSessList = false;
	private GUIContent[] sesslist;		        				
	
	public Vector2 scrollLevelListPosition;
	public Vector2 scrollSubjectListPosition;
	private string selectRes = "Override Resolution";
	private bool fsToggleState = false; 
	private bool noEEGToggleState = true; 
	private bool showFPSToggleState = false;
	private bool enableSkipStateToggleState = false;
	
	private bool blnToggleState = false; 
	
	private float fltSliderValue = 0.5f;
	private float fltScrollerValue = 0.5f;
	private Vector2 scrollPosition = Vector2.zero;
	
	private string subject = "";
	private string configfile = "";
	private string appDir = "";
	private string home = "";
	private string home2 = "";
	private string workingDir = "";
	
	private string errorText = "";
	
	//private Texture blackTex;
     
    void readyConfig()
    {
		config.width = resolutions[listEntry].width;
		config.height = resolutions[listEntry].height;
		config.nofullscreen = fsToggleState;
		config.bootstrapped = true;
		config.expPath = appDir + "/data/" + levelList[currentLevelSelection].text;
		config.subjectPath = appDir + "/data/" + levelList[currentLevelSelection].text + "/" + subject;

		config.appPath = appDir;
		//config.level = levelList[currentLevelSelection].text;
		config.subject = subject;
	 	config.filename = configfile;
    }
    
    void initSubjects () {
    	string lvl = levelList[currentLevelSelection].text;
    	//Debug.Log(appDir + "/data/" + lvl);
    	if (Directory.Exists(appDir + "/data/" + lvl)) {
    		
	    	subjects = Directory.GetDirectories(appDir + "/data/" + lvl);
			subjectslist = new GUIContent[subjects.Length ];
			int k = 0;
			foreach (string subjectPath in subjects) {
				subjectslist[k] = new GUIContent(Path.GetFileName(subjectPath));
				k=k+1;
			}
			currentSubjectSelection = 0;
			
    	} else {
    		subjectslist = new GUIContent[0 ];
    	}
    	currentSubjectSelection = 0;
    	initSessions();

    }
    
    void initSessions () {
    	string lvl = levelList[currentLevelSelection].text;
    	
    	if (subjectslist.Length < 1 ){
    		sesslist = new GUIContent[1];
    		sesslist[0] = new GUIContent("Select Session");   	
    	} else {	
    
	    	string sub = subjectslist[currentSubjectSelection].text;
	    	//Debug.Log(appDir + "/data/" + lvl + "/" + sub);
	    	if (Directory.Exists(appDir + "/data/" + lvl + "/" + sub )) {
	    		
		    	string[] sessions = Directory.GetDirectories(appDir + "/data/" + lvl + "/" + sub);
				sesslist = new GUIContent[sessions.Length + 1 ];
				sesslist[0] = new GUIContent("Select Session");
				int k = 1;
				foreach (string sessPath in sessions) {
					sesslist[k] = new GUIContent(Path.GetFileName(sessPath));
					k=k+1;
				}
				
				
	    	} else {
	    		sesslist = new GUIContent[1];
	    		sesslist[0] = new GUIContent("Select Session");
	    	}
    	}
    	sessEntry = 0;
    }
    
    void setupDirectories() {
    	
    	home = Application.dataPath;
		appDir = Directory.GetCurrentDirectory();
		home = "";		
		workingDir = home;
		home2 = home;
		bool topCreated = false;
		
		try {
				
	    	if (  !Directory.Exists(appDir + "/data/") ) {
	    		Directory.CreateDirectory(appDir + "/data/");
	    		topCreated = true;
	    		errorText = "Created data directory and experiment subdirectories";
	    	}
	    	
	    	foreach( string item in levels ) {
	    		if (  !Directory.Exists(appDir + "/data/" + item) ) {
	    			Directory.CreateDirectory(appDir + "/data/" + item);
	    			if (!topCreated) {
	    				errorText = "Created experiment subdirectories";
	    			}
	    		}
	    	}
		}
		catch (Exception e) 
        {
            errorText = "The process failed: " +  e.ToString();
        } 
    }
    
	// Use this for initialization
	void Start () {
		config = Config.Instance;

		
		//setup directories
		setupDirectories();
		
		//setup subjects
		subjectslist = new GUIContent[0];			

		
		
		resolutions = Screen.resolutions;
		
		reslist = new GUIContent[resolutions.Length ];
		int j = 0;
		//reslist[0] = new GUIContent("No Override");
		foreach (Resolution res in resolutions) {
			reslist[j] = new GUIContent(res.width + " x " + res.height);
			if (res.width == 1024 & res.height == 768) listEntry = j;
			j = j + 1;
		}
		selectRes = reslist[listEntry].text;		
        
	
	    int i = 0;
	    levelList = new GUIContent[levels.Length];  

        foreach( string item in levels )
        {
        	levelList[i] = new GUIContent(item);
        	i = i + 1;
        }
	           
	           
	    
	          
	    currentSelection  = new GUIContent("Select a Resolution");
	    // Make a GUIStyle that has a solid white hover/onHover background to indicate highlighted items
	    listStyle = new GUIStyle();
	    listStyle.normal.textColor = Color.white;
	    
	    popupStyle = new GUIStyle();
	    popupStyle.normal.textColor = Color.white;
	   
	    Texture2D tex = new Texture2D(1, 1);
	    
	    Color[] colors;
	    colors = new Color[1];
	    colors[0] = Color.white; 
	    tex.SetPixels(colors);
	    tex.Apply();
	    
	    
	    Texture2D blackTex = new Texture2D(1, 1);
    	colors[0] = Color.gray;
   	    blackTex.SetPixels(colors);
	    blackTex.Apply();


	    popupStyle.normal.background = blackTex; 
	    popupStyle.hover.background = tex;
	    popupStyle.onHover.background = tex;
	    popupStyle.padding.left = popupStyle.padding.right = popupStyle.padding.top = popupStyle.padding.bottom = 4;   
	    listStyle.hover.background = tex;
	    listStyle.onHover.background = tex;
	    listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;
    
        listSelectedStyle = new GUIStyle();
		//listSelectedStyle.normal.textColor = Color.white;
	    //Texture2D tex = new Texture2D(1, 1);
	    
	    //Color[] colors;
	    //colors = new Color[1];
	    colors[0] = Color.white; 
	    
	    tex.SetPixels(colors);
	    tex.Apply();
	    listSelectedStyle.normal.background = tex;
	    listSelectedStyle.onNormal.background = tex;
	    listSelectedStyle.padding.left = listSelectedStyle.padding.right = listSelectedStyle.padding.top = listSelectedStyle.padding.bottom = 4;
 
       
        errorStyle = new GUIStyle();
        errorStyle.normal.textColor = new Color(0.7F,0.1F,0.1F);
    	initSubjects();
    	//initSessions();
  
	}
	
	void startNewExperiment() {
		if (subject.Length > 0) {
			bool isNew = true;
		    foreach (GUIContent asubject in subjectslist) {
		    	if (asubject.text == subject ) {
		    		isNew = false;
		    		errorText = "Can not start experiment. Subject '" + subject + "' already exists.";
		    	} 
		    }
		    if (isNew) {
		    	Directory.CreateDirectory(appDir + "/data/" + levelList[currentLevelSelection].text + "/" + subject );
		    	config.runMode = ConfigRunMode.NEW;
				run();					
		    }
		} else  {
					errorText = "Please enter a new subject";
		}  
		    
						
	}
	
	void run() {
			readyConfig();
			//Debug.Log(config.width);
			Screen.SetResolution(resolutions[listEntry].width, resolutions[listEntry].height, !fsToggleState);
			Process.Start (levelList[currentLevelSelection].text + "_DirectToRift.exe");
			//Application.LoadLevel(currentLevelSelection + 1);	//used for Mac. Loads next scene
	}
    
    //// GUI GUI
    
    	void OnGUI()
    {
    	
    	//boxStyle = ;
        GUI.skin.label.normal.textColor = Color.black;
        GUI.skin.toggle.normal.textColor = Color.black;
        
    	// LOGO
        GUI.BeginGroup (new Rect (0, 0, 1024, 200));
     	GUI.Label(new Rect (0, 0, 1024, 163), logo);   
   		GUI.EndGroup();
   
   
        GUI.BeginGroup (new Rect (Screen.width / 2 - 400, 100, 800, 600));		  
			//GUILayout.BeginVertical();
			GUI.Box (new Rect (0,0,800,600),"");
 
		        	
		
		  
		  /*      	
		        GUILayout.BeginVertical();

		        GUILayout.TextField("Im a textfield");
		          
		        blnToggleState = GUILayout.Toggle(blnToggleState, "Im a Toggle button");
		        GUILayout.EndVertical();
		        GUILayout.BeginVertical();
		   
        
  
   //   fltScrollerValue = GUILayout.VerticalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Height(290));
     
        GUILayout.EndVertical();
          GUILayout.EndHorizontal();
          */
         
                    
   		
			GUI.BeginGroup (new Rect (20, 80, 800, 600));
				GUI.skin = thisMetalGUISkin;  	   
			//	GUILayout.Label("3D Graphics Settings");
				 
				//toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Experiments:");
				GUILayout.Space(90);
				GUILayout.Label("Subjects:");

				GUILayout.EndHorizontal();
				 
				GUILayout.BeginHorizontal();
				
					GUILayout.BeginVertical();
					    GUI.Box (new Rect (0,25,300,300),"");
					    scrollLevelListPosition = GUILayout.BeginScrollView(scrollLevelListPosition, GUILayout.Width(300), GUILayout.Height(300));
				        newCurrentLevelSelection = SelectList.List( levelList, currentLevelSelection, listStyle, listSelectedStyle );
				        if ( newCurrentLevelSelection != currentLevelSelection) { 
				        	currentLevelSelection = newCurrentLevelSelection;
				        	initSubjects();
				        	errorText = "";
				        }
				        currentLevelSelection = newCurrentLevelSelection;
				        GUILayout.EndScrollView();
				        
				        GUILayout.BeginHorizontal();
						GUILayout.Label("New Subject:");
				   		subject = GUILayout.TextField(subject);
						subject = subject.Trim();
						
						GUILayout.EndHorizontal();
						
						
						GUILayout.BeginHorizontal();
						GUILayout.Label("Config File:");
				   		configfile = GUILayout.TextField(configfile);
						configfile = configfile.Trim();
						
						GUILayout.EndHorizontal();
						
						
				     
				        if ( GUI.Button(new Rect (0,450,200,30),"Start New Experiment") ) {
		    				startNewExperiment();
			    		}
			    		
			            if ( GUI.Button(new Rect (405,450,200,30),"Resume Experiment") ) {
				        	
				        		if (subjectslist.Length > 0) {
					        		config.runMode = ConfigRunMode.RESUME;	
					        		//run();        	
									
				        		} else  {
				        			errorText = "Please select a subject";
				        		}  			    
			    		}

		
		
			        GUILayout.EndVertical();
			        
			        
			        //SUBJECTS
					GUILayout.Space(100);        
					GUILayout.BeginVertical();
					GUI.Box (new Rect (400,25,300,300),"");
					scrollSubjectListPosition = GUILayout.BeginScrollView(scrollSubjectListPosition, GUILayout.Width(300), GUILayout.Height(300));
					newCurrentSubjectSelection = SelectList.List( subjectslist, currentSubjectSelection, listStyle, listSelectedStyle );
					if ( newCurrentSubjectSelection != currentSubjectSelection) { 
				        	currentSubjectSelection = newCurrentSubjectSelection;
				        	initSessions();
				        	errorText = "";
				    }
				    currentSubjectSelection = newCurrentSubjectSelection;
					GUILayout.EndScrollView();
					
						//draw last so it's on top
						
						    if (PopupList.List (
					           	new Rect(405, 330, 120, 20), 
					           	ref showSessList, 
					           	ref sessEntry, 
					           	new GUIContent(selectSession), 
					           	sesslist, 
					           	popupStyle)) {
			        				sesspicked = true;
			    			}
						    
						    GUI.Label(new Rect (512, 333,16, 16), pulldown);
						    
						    //Debug.Log(sessEntry);
						    if (sesspicked) {
						        selectSession = sesslist[sessEntry].text;
	   						}
	   						
	   						
	   						if ( GUI.Button(new Rect (530,330,150,20),"Playback Session") ) {
				        	
				        		if (subjectslist.Length > 0) {
					        			        	
									config.runMode = ConfigRunMode.PLAYBACK;	
									
									subject = subjectslist[currentSubjectSelection].text;
									///Debug.Log(sub);
					        		run();
				        		} else  {
				        			errorText = "Please select a subject";
				        		}  			    
			    		}
					
					GUILayout.EndVertical();
					

		     
		        
		       GUILayout.EndHorizontal();
               GUI.Label(new Rect (0, 495, 325, 30), errorText, errorStyle); 
	       GUI.EndGroup();
	       
	       
            ///////////////////	       
            /// top section
	    	///////////////////    
	    	
			GUI.BeginGroup (new Rect (20, 20, 900, 600));
				GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
			    		fsToggleState = GUI.Toggle(new Rect (260, 0, 100, 20),fsToggleState, "No Full Screen");    			
    					noEEGToggleState = GUI.Toggle(new Rect (380, 0, 100, 20),noEEGToggleState, "No EEG");		 
						showFPSToggleState = GUI.Toggle(new Rect (490, 0, 100, 20),showFPSToggleState, "Show FPS");						
						//enableSkipStateToggleState = GUI.Toggle(new Rect (580, 0, 100, 20),showFPSToggleState, "Enable Skip Task Key");		        	
               		GUILayout.EndHorizontal();
               		
               		
       				//GUI.Label(new Rect (0, 30, 125, 20), "Working Directory:");
							
					//home2 = GUI.TextField(new Rect(120,30, 550, 20),home);
					if (home2 != home) { 
						initSubjects();
					}
					home = home2;
					
					//draw last so it's on top
					GUI.Label(new Rect (0, 0, 145, 20), "Screen Resolution:");
					    if (PopupList.List (
				           	new Rect(120, 0, 120, 20), 
				           	ref showList, 
				           	ref listEntry, 
				           	new GUIContent(selectRes), 
				           	reslist, 
				           	popupStyle)) {
		        				picked = true;
		    			}
					    
					    GUI.Label(new Rect (226, 3,16, 16), pulldown);
					    
					    if (picked) {
					        selectRes = reslist[listEntry].text;
   						}
  
		       GUILayout.EndVertical();	
		     GUI.EndGroup();   
		     
		    
	    GUI.EndGroup();
	    
	    	GUI.Label(new Rect (Screen.width / 2 + 330, 705, 300, 30), "Version: " + config.version);   

   
    }

}
