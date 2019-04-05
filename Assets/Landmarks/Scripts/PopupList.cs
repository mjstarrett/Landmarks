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

public class PopupList {
    static int popupListHash = "PopupList".GetHashCode();
    
    public static bool List (Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, GUIContent[] listContent,
                             GUIStyle listStyle) {
        return List(position, ref showList, ref listEntry, buttonContent, listContent, "button", "box", listStyle);
    }
    
    public static bool List (Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, GUIContent[] listContent,
                             GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle) {
        int controlID = GUIUtility.GetControlID(popupListHash, FocusType.Passive);
        bool done = false;
        switch (Event.current.GetTypeForControl(controlID)) {
            case EventType.MouseDown:
                if (position.Contains(Event.current.mousePosition)) {
                    GUIUtility.hotControl = controlID;
                    showList = true;
                }
                break;
            case EventType.MouseUp:
                if (showList) {
                    done = true;
                }
                break;
        }
        
        GUI.Label(position, buttonContent, buttonStyle);
        if (showList) {
            Rect listRect = new Rect(position.x, position.y, position.width, listStyle.CalcHeight(listContent[0], 1.0f)*listContent.Length);
            GUI.Box(listRect, "", boxStyle);
            listEntry = GUI.SelectionGrid(listRect, listEntry, listContent, 1, listStyle);
        }
        if (done) {
            showList = false;
        }
        return done;
    }
}