/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Script para mover la camara principal en 1ra persona.
* Por defecto se debe establecer el tag MainCamera al game objet que
* corresponda para que pueda referenciarlo y mover la camara.
* 
* Se debe asociar un game object como referencia y que representa
* al actor principal. Este game object debe tener las propiedades
* rigidbody, collider y/o character controller.
\*******************************************************************/

using UnityEngine;
using System.Collections;

public class CameraControllerFirstPerson : MonoBehaviour {
	// La camara principal de la escena
	public 	GameObject 	cameraMain;	
	// Gameobject que representa al dispositivo configurado
	public  Transform	objectFollow;
	// Variables que almacenaran la distancia y la altura de la camara, respectivamente
	public float distance;
	public float height;
	// Variables que almacenaran el suavizado de la altura y de la rotacion
//	private float heightDamping = 1f;
	private float rotationDamping = 1f;	
	// Calculos de angulos de rotacion
	private float wantedRotationAngle = 0f;
	private float wantedHeight = 0f;
	private float currentRotationAngle = 0f;
	private float currentHeight = 0f;
	
	
	// Use this for initialization
	void Awake () {
		if (cameraMain == null){
			cameraMain = GameObject.FindWithTag("MainCamera");
			if (cameraMain == null) {
				Debug.LogWarning("Manager Device · Warning.\nYou should set the tag MainCamera the corresponding Game Object");
				return;			
			}
			
			objectFollow = (Transform) this.transform;
			if (objectFollow == null) {
				Debug.LogWarning("Manager Device · Warning.\nThere is no GameObject in the 'Object to Follow' field");
				return;
			} 
			// Determinar la posición de la camara segun la posicion del target en el eje Z + el valor en Z del target
			// Otra opcion es ponerlo encima el target en el eje Y
		}
	}
	
	void Move() {
		wantedRotationAngle = objectFollow.eulerAngles.y;
		wantedHeight = objectFollow.position.y + height;
		currentRotationAngle = cameraMain.transform.eulerAngles.y;
		currentHeight = cameraMain.transform.position.y;
		
		
		// Suavizamos la rotacion alrededor del eje y
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		// Suavizamos la altura
		//currentHeight = Mathf.Lerp(currentHeight,wantedHeight,heightDamping*Time.deltaTime);
		currentHeight = wantedHeight;
		// Convertimos el angulo a una rotation
		Quaternion currentRotation = Quaternion.Euler(0,currentRotationAngle,0);
		// Modificamos la posicion de la camara hasta situarse
		// tras el objetivo a la distancia deseada
		cameraMain.transform.position = objectFollow.position;
		cameraMain.transform.position -= currentRotation * Vector3.forward * distance;
		// Modificamos la altura de la camara
		cameraMain.transform.position = new Vector3(cameraMain.transform.position.x,currentHeight, cameraMain.transform.position.z);
		// Obligamos a la camara a apuntar siempre al objetivo
		cameraMain.transform.LookAt(objectFollow);
	}
	
	
	// Update is called once per frame
	void LateUpdate () {
		Move();
	}
}
