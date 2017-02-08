/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Descripcion: Script que se usa con Mouse 3D. Es igual al script
* SmoothFollow2 de SpaceNavigator driver for Unity3D, 
* con la diferencia de que se usa el evento FixedUpdate.
\*******************************************************************/

using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {

	public Transform target;			// Item sobre el cual hara el seguimiento
	public float 	 distance = 3.0f;	// Distancia sobre eje Z
	public float 	 height = 3.0f;		// Altura sobre eje Y
	public float 	 damping = 5.0f;	// Amortiguacion del movimiento
	public bool 	 smoothRotation = true;	// Si la rotacion sera suave
	public bool 	 followBehind = true;	
	public float 	 rotationDamping = 10.0f;	// Suavizar o amortiguar la rotacion


	void FixedUpdate () {
		Vector3 wantedPosition;
		if (followBehind)
			wantedPosition = target.TransformPoint(0, height, -distance);
		else
			wantedPosition = target.TransformPoint(0, height, distance);
		
		transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
		
		if (smoothRotation) {
			Quaternion wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
		} else transform.LookAt(target, target.up);
	
	}
}
