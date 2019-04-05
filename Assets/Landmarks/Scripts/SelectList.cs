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


public class SelectList {


        public static int List( ICollection list, int selected, GUIStyle defaultStyle, GUIStyle selectedStyle )
        {         
        	int i  = 0;
            foreach( GUIContent item in list )
            {
                if( GUILayout.Button( item.text, ( selected == i ) ? selectedStyle : defaultStyle ) )
                {
                    if( selected == i )
                    // Clicked an already selected item. Deselect.
                    {
                        selected = 0;
                    }
                    else
                    {
                        selected = i;
                    }
                }
                i = i + 1;
            }
        
            return selected;
        }
      
}        
        
   