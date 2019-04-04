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

public class CollisionDetection : MonoBehaviour {

	private Experiment manager;
	private dbLog log;


	void OnControllerColliderHit(ControllerColliderHit hit)  {
		if(hit.gameObject.tag == "Target") {
			manager.OnControllerColliderHit(hit.gameObject);
		}   
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Target")
        {
            manager.OnControllerColliderHit(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        manager.OnControllerColliderHit(other.gameObject);
    }
    
    
    void Start ()
	{		
		GameObject experiment = GameObject.FindWithTag ("Experiment");
	    manager = experiment.GetComponent("Experiment") as Experiment;
	    log = manager.dblog;
	}
	
	
	


}

