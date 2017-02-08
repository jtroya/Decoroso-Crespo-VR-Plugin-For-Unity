/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Script para mover el player/avatar con el gamepad en
* primera y tercera persona.
\*******************************************************************/

using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class PlayerControllerGamePad : MonoBehaviour {

//	private Vector3 newPosition;
	private static bool didQueryNumOfCtrlrs = false;
//	private Rigidbody	playerRigidbody;
//	private float mPitch = 0f;
//	private float rotateX = 0f;
//	private float translateZ = 0f;
//	private float mHdg = 0f;

	private ManagerDevice md;
	//private bool run;
	//private bool walk;
	private float normalFactor = 1.0f;
	private float yRot = 0f;
	private float xRot = 0f;
	private float zRot = 0f;
	//private float rotationY = 0f;
	// Constante para movimiento
	private float constanteMov= 0.8f;

	private float pitchValue = 0f;
	private float yawValue = 0f;
	private float rollValue = 0f;

	public bool firstPerson;
	public bool thirdPerson;
	public float speed = 5f;
	public float turnSpeed = 55f; // Velocidad de rotacion
	public float sticksSensitivity = 0.65f;
	public float triggersSensitivity = 1f;
	public float fastMoveFactor = 3.0f;
	public float slowMoveFactor = -0.75f;


	void Awake() {
		md = ManagerDevice.instance;

	}

	void Start () {
		// Verifica si esta conectado uno o más gamepads
		if(!didQueryNumOfCtrlrs) {
			didQueryNumOfCtrlrs = true;
			
			int queriedNumberOfCtrlrs = XCI.GetNumPluggedCtrlrs();
			if(queriedNumberOfCtrlrs == 1) {
				Debug.Log("Only " + queriedNumberOfCtrlrs + " Xbox controller plugged in.");
			}
			else if (queriedNumberOfCtrlrs == 0) {
				Debug.LogWarning("No Xbox controllers plugged in!");
			}
			else {
				Debug.Log(queriedNumberOfCtrlrs + " Xbox controllers plugged in.");
			}
			
			XCI.DEBUG_LogControllerNames();
		}

		// Para 3ra persona
//		if (thirdPerson) {
//			playerRigidbody = GetComponent <Rigidbody> ();
//			newPosition = transform.position;
//		}

		// Almacena la posicion inicial del player
		xRot = transform.eulerAngles.x;
		yRot = transform.eulerAngles.y;
		zRot= transform.eulerAngles.z;
	}


	void Update () {

		// Aplicar las rotaciones sobre el eje x,y,z
		pitchValue 	 = XCI.GetAxis(md.gp_pitch) * sticksSensitivity;
		yawValue 	 = XCI.GetAxis(md.gp_yaw)   * sticksSensitivity;
		rollValue	 = XCI.GetAxis(md.gp_roll)  * sticksSensitivity;

		//pitchValue  = ((!md.gp_mixAvaRotAxisX) || (md.gp_mixAvaRotAxisX && XCI.GetButton(md.gp_mixButtonAvaRotAxisX))) ? XCI.GetAxis(md.gp_pitch) * sticksSensitivity : 0f;
		//yawValue    = ((!md.gp_mixAvaRotAxisY) || (md.gp_mixAvaRotAxisY && XCI.GetButton(md.gp_mixButtonAvaRotAxisY))) ? XCI.GetAxis(md.gp_yaw)   * sticksSensitivity : 0f;
		//rollValue	= ((!md.gp_mixAvaRotAxisZ) || (md.gp_mixAvaRotAxisZ && XCI.GetButton(md.gp_mixButtonAvaRotAxisZ))) ? XCI.GetAxis(md.gp_roll)  * sticksSensitivity : 0f;

		// Validacion de las teclas de rotacion pith con yaw, pitch con roll
		// para que asi se independicen las rotaciones y no se realicen al tiempo

		if (md.gp_pitch == md.gp_yaw) {
			if (!md.gp_mixAvaRotAxisX || (md.gp_mixAvaRotAxisX && XCI.GetButton(md.gp_mixButtonAvaRotAxisX)) ) {
				pitchValue = XCI.GetAxis(md.gp_pitch) * sticksSensitivity ;
				yawValue = 0f;
			}

			if (!md.gp_mixAvaRotAxisY || (md.gp_mixAvaRotAxisY && XCI.GetButton(md.gp_mixButtonAvaRotAxisY)) ) {
				yawValue = XCI.GetAxis(md.gp_yaw)   * sticksSensitivity;
				pitchValue = 0f;
			}

		}

		if (md.gp_pitch == md.gp_roll) {
			if (!md.gp_mixAvaRotAxisX || (md.gp_mixAvaRotAxisX && XCI.GetButton(md.gp_mixButtonAvaRotAxisX)) ) {
				pitchValue = XCI.GetAxis(md.gp_pitch) * sticksSensitivity ;
				rollValue = 0f;
			}

			if (!md.gp_mixAvaRotAxisZ || (md.gp_mixAvaRotAxisZ && XCI.GetButton(md.gp_mixButtonAvaRotAxisZ)) ) {
				rollValue = XCI.GetAxis(md.gp_roll)  * sticksSensitivity;
				pitchValue = 0f;
			}

		}

		if (md.gp_yaw == md.gp_roll) {
			//Debug.Log("las teclas Y Z son iguales");
			if (!md.gp_mixAvaRotAxisY || (md.gp_mixAvaRotAxisY && XCI.GetButton(md.gp_mixButtonAvaRotAxisY)) ) {
				yawValue = XCI.GetAxis(md.gp_yaw)   * sticksSensitivity;
				rollValue = 0f;
			} 

			if (!md.gp_mixAvaRotAxisZ || (md.gp_mixAvaRotAxisZ && XCI.GetButton(md.gp_mixButtonAvaRotAxisZ)) ) {
				rollValue = XCI.GetAxis(md.gp_roll)  * sticksSensitivity;
				yawValue = 0f;
			}
		}

		// Nota: En el gamepad usado como pruebas, los triggers derecho e izq. hacen que su valor sea por defecto 0,325
		// No se debe usar con el mando hasta que se reemplace por otro

		// Ajustes para 1ra persona
		if (firstPerson) {

			if (md.resetAvatarRotation && XCI.GetButton(md.gp_ResetAvatarRotaton)){
				// Rota al player a su posicion inicial
				transform.localEulerAngles = new Vector3(xRot, yRot, zRot);
			} else {
				transform.Rotate(pitchValue, 0 , 0);
				transform.Rotate(0, yawValue, 0);
				transform.Rotate(0, 0, -rollValue);	
				//transform.localEulerAngles = new Vector3(pitchValue, yawValue, -rollValue);

			}

			// Run o Slow
			if (XCI.GetButton(md.gp_Run) && XCI.GetDPad(md.gp_pad_Up)) {
				// mas rapido hacia adelante
				float vertical = constanteMov * (speed * fastMoveFactor) * Time.deltaTime;
				transform.Translate(0, 0, vertical);

			} else if (XCI.GetButton(md.gp_Run) && XCI.GetDPad(md.gp_pad_Down)) {
				// mas rapido hacia atras
				float vertical = constanteMov * (speed * fastMoveFactor) * Time.deltaTime * -1;
				transform.Translate(0, 0, vertical);

			} else if (XCI.GetButton(md.gp_Slow) && XCI.GetDPad(md.gp_pad_Up)) {
				// mas lento hacia adelante
				float vertical = constanteMov * (speed * -slowMoveFactor) * Time.deltaTime;
				transform.Translate(0, 0, vertical); 

			} else if (XCI.GetButton(md.gp_Slow) && XCI.GetDPad(md.gp_pad_Down)) {
				// mas lento hacia atras
				float vertical = constanteMov * (speed * -slowMoveFactor) * Time.deltaTime * -1;
				transform.Translate(0, 0, vertical);

			} else {
				MovePlayerPad(normalFactor);
			}

			// Go up - Go Down
			if (XCI.GetButton(md.gp_Y_Up)) {
				transform.position += transform.up * speed * Time.deltaTime;
			} else if (XCI.GetButton(md.gp_Y_Down)) {
				transform.position -= transform.up * speed * Time.deltaTime;
			}

		}

		// Ajustes para 3ra persona
		if (thirdPerson) {

			if (md.resetAvatarRotation && XCI.GetButton(md.gp_ResetAvatarRotaton)){
				// Rota al player a su posicion inicial
				transform.localEulerAngles = new Vector3(xRot, yRot, zRot);
			} else {
				
				transform.Rotate(pitchValue, 0 , 0);
				transform.Rotate(0, yawValue, 0);
				//transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime); 
				transform.Rotate(0, 0, -rollValue);
			}

			// Ir mas rapido o mas lento
			if (XCI.GetButton(md.gp_Run) && XCI.GetDPad (md.gp_pad_Up)){
				float vertical = constanteMov * (speed * fastMoveFactor) * Time.deltaTime;
				transform.Translate(0, 0, vertical);

			} else if(XCI.GetButton(md.gp_Run) && XCI.GetDPad (md.gp_pad_Down)) {
				float vertical = constanteMov * (speed * fastMoveFactor) * Time.deltaTime * -1;
				transform.Translate(0, 0, vertical);

			} else if (XCI.GetButton(md.gp_Slow) && XCI.GetDPad (md.gp_pad_Up)){
				float vertical = constanteMov * (speed * slowMoveFactor) * Time.deltaTime;
				transform.Translate(0, 0, vertical);

			} else if (XCI.GetButton(md.gp_Slow) && XCI.GetDPad (md.gp_pad_Down)){
				float vertical = constanteMov * (speed * slowMoveFactor) * Time.deltaTime * -1;
				transform.Translate(0, 0, vertical);
			}

			// Go up - Go Down
			if (XCI.GetButton(md.gp_Y_Up)) {
				transform.position += transform.up * speed * Time.deltaTime;
			} else if (XCI.GetButton(md.gp_Y_Down)) {
				transform.position -= transform.up * speed * Time.deltaTime;
			}


			// Mover el gameObject con el DPad
			if (XCI.GetDPad (md.gp_pad_Up)) {
				transform.Translate(Vector3.forward * speed * Time.deltaTime);
			}
			
			if (XCI.GetDPad (md.gp_pad_Down)) {
				transform.Translate(-Vector3.forward * speed * Time.deltaTime);
			}

			if (XCI.GetDPad (md.gp_pad_Right)) {
				//transform.Translate(speed * Time.deltaTime, 0f,0f);
				//float horizontal = constanteMov * turnSpeed * Time.deltaTime;
				//transform.Rotate(0, horizontal, 0);
				transform.Translate(Vector3.right * speed * Time.deltaTime);
			}
			
			if (XCI.GetDPad (md.gp_pad_Left)) {
				//transform.Translate(-(speed * Time.deltaTime), 0f,0f);
				//float horizontal = constanteMov * turnSpeed * Time.deltaTime * -1;
				//transform.Rotate(0, horizontal, 0);
				transform.Translate(Vector3.left * speed * Time.deltaTime);
			}

		}
	}


	void MovePlayerPad(float speedFactor) {
		// Mover el gameObject con el DPad
		if (XCI.GetDPad(md.gp_pad_Up)) {
			transform.Translate(Vector3.forward * (speed * speedFactor) * Time.deltaTime);
		}
		
		if (XCI.GetDPad (md.gp_pad_Down)) {
			transform.Translate(Vector3.back * (speed * speedFactor) * Time.deltaTime);
		}
		
		if (XCI.GetDPad (md.gp_pad_Right)) {
			//transform.Rotate(Vector3.up, (turnSpeed * speedFactor) * Time.deltaTime); 
			transform.Translate(Vector3.right * speed * Time.deltaTime);
		}
		
		if (XCI.GetDPad (md.gp_pad_Left)) {
			//transform.Rotate(Vector3.up, (-turnSpeed * speedFactor) * Time.deltaTime);
			transform.Translate(Vector3.left * speed * Time.deltaTime);
		}
	}
}
