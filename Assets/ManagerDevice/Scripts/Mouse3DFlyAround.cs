/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Patrick Hogenboom									
* Descripcion: Script que se usa con Mouse 3D. Se adjunta sobre el
* game object sobre el cual se hara uso este dispositivo. Este script
* es igual al fichero FlyAround.cs de SpaceNavigator driver for Unity3D
\*******************************************************************/

using UnityEngine;
using System.Collections;

public class Mouse3DFlyAround : MonoBehaviour {

	public bool HorizonLock = true;
	
	void Update () {
		transform.Translate(SpaceNavigator.Translation, Space.Self);
		
		if (HorizonLock) {
			// This method keeps the horizon horizontal at all times.
			// Perform azimuth in world coordinates.
			transform.Rotate(Vector3.up, SpaceNavigator.Rotation.Yaw() * Mathf.Rad2Deg, Space.World);
			// Perform pitch in local coordinates.
			transform.Rotate(Vector3.right, SpaceNavigator.Rotation.Pitch() * Mathf.Rad2Deg, Space.Self);
		}
		else {
			transform.Rotate(SpaceNavigator.Rotation.eulerAngles, Space.Self);
		}
	}
}
