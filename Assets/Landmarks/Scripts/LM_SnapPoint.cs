/*
    Landmarks   
    Copyright (C) 2019 Jason Laczko & Michael J. Starrett

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_SnapPoint : MonoBehaviour
{

    /* Currently, this script is a dummy that makes it possible to quickly select 
     * subcomponents of a gameObject in order to snap other objects to an offset
     * locations. For example, if you want to move your player to the current destination
     * (but not inside the destination) you could attach an empty gameobject to your
     * destination, where you want the player snapped, and then add this component to it.
     * 
     * Use in conjunction with movePlayerToDestination.cs to use snapPoint to move the avatar.
     */    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
