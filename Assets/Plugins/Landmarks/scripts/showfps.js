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


@script ExecuteInEditMode

private var gui : GUIText;

private var updateInterval = 1.0;
private var lastInterval : double; // Last interval end time
private var frames = 0; // Frames over current interval

function Start()
{
    lastInterval = Time.realtimeSinceStartup;
    frames = 0;
   
}

function OnDisable ()
{
	if (gui)
		DestroyImmediate (gui.gameObject);
}

function Update()
{
    ++frames;
    var timeNow = Time.realtimeSinceStartup;
    if (timeNow > lastInterval + updateInterval)
    {
		if (!gui)
		{
			var go : GameObject = new GameObject("FPS Display", GUIText);
			go.hideFlags = HideFlags.HideAndDontSave;
			go.transform.position = Vector3(0,0,0);
			gui = go.GetComponent.<GUIText>();
			gui.pixelOffset = Vector2(5,15);
		}
        var fps : float = frames / (timeNow - lastInterval);
		var ms : float = 1000.0f / Mathf.Max (fps, 0.00001);
		gui.text = ms.ToString("f1") + "ms " + fps.ToString("f2") + "FPS";
        frames = 0;
        lastInterval = timeNow;
    }
}
