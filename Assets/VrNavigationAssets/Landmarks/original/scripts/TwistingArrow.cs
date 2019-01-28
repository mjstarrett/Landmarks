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

public class TwistingArrow : MonoBehaviour {


	public Transform arrow;
	public Transform avatar;
		

	void Start () {
	
	}
	

	void Update () {
		
		//float trueRot = arrow.localEulerAngles.y + 90.0;
		

		
		
		Quaternion lookAt = Quaternion.LookRotation(avatar.position - arrow.position);
		
		float sign = -1;
		float flipped = lookAt.eulerAngles.y;
		if (flipped > 180) {
			flipped = 360 - flipped;
			sign = 1;
		}
		float quad = flipped;
		if (quad > 90) quad = 180 - quad;
		
		flipped = flipped/2;
		
		
		//arrow.Rotate( 0,225 + flipped, 0);
		
		
		Quaternion a = Quaternion.Euler ( 90,0,0 );
		Quaternion b = a * Quaternion.Euler ( 0,0,0 );
		arrow.rotation = b * Quaternion.Euler ( 0,flipped - 45,0 ) * Quaternion.Euler ( quad * sign,0,0 );
		//arrow.eulerAngles = new Vector3 ( 90,0,90 );
		//Debug.Log( arrow.rotation.ToString("f3") );
		//Debug.Log( avatar.localEulerAngles.ToString("f3") );
		
		
	}
}
