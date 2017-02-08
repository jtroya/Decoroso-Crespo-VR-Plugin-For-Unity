/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Script para mover el player con el gamepad
* 
* Desde en Manager Device se debe asociar un game object con el cual
* podra interactuar en la escena.
\*******************************************************************/

using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class CameraControllerGamepad3rd : MonoBehaviour {

	public Transform target;            // The position that that camera will be following.
	[HideInInspector]
	public float damping = 20f;        // The speed with which the camera will be following.
	private Vector3 offset;            // The initial offset from the target.


	public float sticksSensitivity = 2f;
	public float triggersSensitivity = 1f;
	public float smoothing = 7f; 

	private float mHdg = 0f;
	private float mPitch = 0f;
	private float rotateX = 0f;
	private float rotateY = 0f;
	private Quaternion startRotation;
	private Vector3 startPosition;
	//private Vector3 offset;

//	private const float MAX_WAIT_TME = 0.3f;
//	private float waitTimer = MAX_WAIT_TME;


	void Start () {
		// Almacena la primera posicion antes de mover la camara
		//startRotation = transform.rotation;
		//startPosition = transform.position;
		// Calculate the initial offset.
		//offset = transform.position - target.position;

		if (target == null) {
			Debug.LogWarning("Manager Device · Warning.\nYou should attach a GameObject in the 'Object to Follow' field");
			return;
		} else {
			// Calculate the initial offset.
			offset = target.transform.position - transform.position;
		}
	}
	

	void LateUpdate () {
		float currentAngle = transform.eulerAngles.y;
		float desiredAngle = target.transform.eulerAngles.y;
		float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);
		Quaternion rotation = Quaternion.Euler(0, angle, 0);

		transform.position = target.transform.position - (rotation * offset);
		//transform.LookAt(target.transform);

//		float currentPitchValue = transform.eulerAngles.x;
//		float currentRollValue  = transform.eulerAngles.z;
		float pitchValue = XCI.GetAxis(XboxAxis.RightStickY) * sticksSensitivity;
		float rollValue  = XCI.GetAxis(XboxAxis.RightStickX) * sticksSensitivity;


		if ((pitchValue <= 2 || pitchValue >= -2) && (rollValue <= 2 || rollValue >= -2)) {
			transform.Rotate(pitchValue, 0 , 0);
			transform.Rotate(0, 0, -rollValue);
		} else {
			transform.LookAt(target.transform);
		}

		//Debug.Log("Current Pitch: " + currentPitchValue.ToString() + "\tNew Pitch: " + pitchValue.ToString() + "\n");
		//Debug.Log("Current Roll: " + currentRollValue.ToString() + "\tNew Roll: " + rollValue.ToString());




//		rotateX = XCI.GetAxis(XboxAxis.RightStickX) * sticksSensitivity;
//		rotateY = XCI.GetAxis(XboxAxis.RightStickY) * sticksSensitivity;

		// Create a postion the camera is aiming for based on the offset from the target.
//		Vector3 targetCamPos = target.position + offset;
		// Smoothly interpolate between the camera's current position and it's target position.
//		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);


		// Verificar si ha cambiado la posicion
//		if (transform.position.x != rotateX) {
//			ChangeHeading(rotateX);
//		}
//
//		if (transform.rotation.x != rotateY && transform.rotation.x != startRotation.x){
//			ChangePitch(rotateY);
//		}

		// Verificar si ha de volver a la posicion inicial
//		if (waitTimer > 0.0f)
//			waitTimer -= Time.deltaTime;
//
//		if (waitTimer <= 0.0f) {
//			if (XCI.GetButton(XboxButton.RightStick)) {
//				ResetCameraPosition();
//				waitTimer = MAX_WAIT_TME;
//			}
//		}
	}
	

	void ResetCameraPosition() {
		// Establece la posición de la camara a la inicial
		Debug.Log ("Reset camera position");
		// Debe rotar la diferencia entre la posicion actual y la inicial en los 3 ejes
		//transform.rotation = startRotation;
		rotateX -= startRotation.x;
		rotateY -= startRotation.y;
		ChangeHeading(rotateX);
		ChangePitch(rotateY);

	}

	void ChangeHeading(float aVal) {
		mHdg += aVal;
		WrapAngle(ref mHdg);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}
	
	void ChangePitch(float aVal) {
		mPitch += aVal;
		WrapAngle(ref mPitch);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}
	
	public static void WrapAngle(ref float angle) {
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
	}
}
