/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Descripcion: Script para mover la camara principal en 3ra persona.
* Por defecto se debe establecer el tag MainCamera al game objet que
* corresponda para que pueda referenciarlo y mover la camara.
* 
* Se debe asociar un game object como referencia y que representa
* al actor principal. Este game object debe tener las propiedades
* rigidbody, collider y/o character controller.
\*******************************************************************/

using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class CameraControllerThirdPerson : MonoBehaviour {

	public Transform target;           // Gameobject al cual la camara seguira
	[HideInInspector]
	public float damping = 25;        // Velocidad a la cual seguira la camara al gameobject.
	public float rotationSpeed = 5f;          // Velocidad con la que la camara hara el seguimiento
	public float sticksSensitivity = 2f;
	public float maximumX; //35f;
	public float maximumY; //75f;
	public float maximumZ;
	public float zoomSpeed = 60f;
	public float zoomFactor = 6f;
	public float camFieldOfView = 60f;

//	private Vector3 offset;            // Distancia entre la camara y el target.
//	private bool inicio = false;
//	private float turnSpeed = 90f;
	private ManagerDevice md;
	private Quaternion initialCameraRotation;
	private float yRot = 0f;
	private float xRot = 0f;
	private float zRot = 0f;
	private float rotationY = 0f;
	private float rotationX = 0f;
	private float rotationZ = 0f;
	private Camera camPlayer;	
	private float camInitFieldOfView = 0f;
	private float camFieldOfViewMin = 15f;
	private float camFieldOfViewMax = 0f;


	// Use this for initialization

	void Awake() {
		md = ManagerDevice.instance;
	}

	void Start () {
		if (target == null) {
			Debug.LogWarning("Manager Device · Warning.\nYou should attach a GameObject in the 'Object to Follow' field");
			return;
		} else {
			// Se debe posicionar la camara por defecto
			transform.position = new Vector3(target.position.x, target.position.y + 2.5f, target.position.z - 4f);
			transform.localEulerAngles = new Vector3(30, 0, 0);
			// Calculate the initial offset.
			//offset = target.position - transform.position;
			// Almacena la rotacion inicial de la camara
			//m_CameraTargetRot = transform.localRotation;
			initialCameraRotation = transform.localRotation;
			xRot = initialCameraRotation.eulerAngles.x;
			yRot = initialCameraRotation.eulerAngles.y;
			zRot = initialCameraRotation.eulerAngles.z;
			rotationY = yRot;
			rotationX = xRot;
			rotationZ = zRot;
			// Establece los valores maximos de rotacion
			maximumX = md.cam_maxAngleRot_X;
			maximumY = md.cam_maxAngleRot_Y;
			maximumZ = md.cam_maxAngleRot_Z;

			// Selecciona la camara para zoom
			camPlayer = this.GetComponent<Camera>();
			camInitFieldOfView = camPlayer.fieldOfView;
			camFieldOfViewMax = camInitFieldOfView;
			// Calculo del zoom max
			camFieldOfViewMin = Mathf.RoundToInt(camInitFieldOfView / zoomFactor);
		}
	}


	/// <summary>
	/// Restarts the camera rotation.
	/// </summary>
	void RestartCameraRotation() {
		xRot = initialCameraRotation.eulerAngles.x;
		yRot = initialCameraRotation.eulerAngles.y;
		zRot = initialCameraRotation.eulerAngles.z;
		rotationY = yRot;
		rotationX = xRot;
		rotationZ = zRot;
		camPlayer.fieldOfView = camInitFieldOfView;
		transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
	}
	
	// Update is called once per frame
	void LateUpdate  () {

		// Con vector3.up mueve el mundo
		// Con transform.up mueve la camara
		// para poner la camara como hijo del target - transform.parent = yourParentObject.transform
		//transform.RotateAround (transform.position, Vector3.up, damping * Time.deltaTime * 3f);
		
		
		// Angulos de rotacion de la camara en 3 direcciones
		Vector3 rightRotation = new Vector3(0, maximumY, 0);
		Vector3 leftRotation = new Vector3(0, -maximumY, 0);
		Vector3 upRotation = new Vector3(-maximumX , 0, 0);
		Vector3 downRotation = new Vector3(maximumX , 0, 0);
		Vector3 zAxisRotationRight = new Vector3(0, 0, maximumZ);
		Vector3 zAxisRotationLeft = new Vector3(0, 0, -maximumZ);

		// Si se navega con el teclado
		if (md.VarNavigationDevices == ManagerDevice.NavigationDevices.Keyboard) {
			// Girar la camara a la izq, der o arriba
			if (Input.GetKey(md.kb_Cam_Left)) { 
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(leftRotation), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Right)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rightRotation), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Rot_Z_Left)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(zAxisRotationLeft), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Rot_Z_Right)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(zAxisRotationRight), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Up)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(upRotation), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Down)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(downRotation), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Rot_Z_Left)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(zAxisRotationLeft), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Rot_Z_Right)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(zAxisRotationRight), rotationSpeed * Time.deltaTime);
			}
			//else {
			//	transform.LookAt(target.transform);
			//}

			// Si pulsa el boton establecido retorna a la rotacion inicial
			if (md.resetCameraRotation && Input.GetKey(md.kb_ResetCameraRotation)) {
				RestartCameraRotation();
			}

			// Zoom
			if(camFieldOfView >= camFieldOfViewMax)
				camFieldOfView = camFieldOfViewMax;
			// Zoom 
			if(camFieldOfView <= camFieldOfViewMin)
				camFieldOfView = camFieldOfViewMin;

			if (Input.GetKey(md.kb_Cam_ZoomIn)){
				// Zoom in
				camFieldOfView -= zoomSpeed * Time.smoothDeltaTime;
			} else if (Input.GetKey(md.kb_Cam_ZoomOut)) {
				// Zoom out
				camFieldOfView += zoomSpeed * Time.smoothDeltaTime;
			}
			// Ajustar el Zoom
			camPlayer.fieldOfView = camFieldOfView;

		}

		// Si se navega por gamepad
		if (md.VarNavigationDevices == ManagerDevice.NavigationDevices.Gamepad) {

			//xRot = ((!md.gp_mixCamRotAxisX) || (md.gp_mixCamRotAxisX && XCI.GetButton(md.gp_mixButtonCamRotAxisX))) ? XCI.GetAxis(md.gp_Cam_UpDown) * sticksSensitivity : 0f;
			//yRot = ((!md.gp_mixCamRotAxisY) || (md.gp_mixCamRotAxisY && XCI.GetButton(md.gp_mixButtonCamRotAxisY))) ? XCI.GetAxis(md.gp_Cam_LeftRight) * sticksSensitivity :0f;
			//zRot = ((!md.gp_mixCamRotAxisZ) || (md.gp_mixCamRotAxisZ && XCI.GetButton(md.gp_mixButtonCamRotAxisZ))) ? XCI.GetAxis(md.gp_Cam_zRotation) * sticksSensitivity :0f;

			xRot = XCI.GetAxis(md.gp_Cam_UpDown) * sticksSensitivity;
			yRot = XCI.GetAxis(md.gp_Cam_LeftRight) * sticksSensitivity;
			zRot = XCI.GetAxis(md.gp_Cam_zRotation) * sticksSensitivity;
					
			float Xon = Mathf.Abs (XCI.GetAxis(md.gp_Cam_UpDown));
			float Yon = Mathf.Abs (XCI.GetAxis(md.gp_Cam_LeftRight));
			float Zon = Mathf.Abs (XCI.GetAxis(md.gp_Cam_zRotation));

			// En el gamepad el joystick analogo del eje Y hace la rotacion sobre el eje X en el plano
			// y en el joystick analogo del eje X hace la rotacion sobre el eje Y en el plano.

			if (md.resetCameraRotation && XCI.GetButton(md.gp_ResetCameraRotation)){
				// Vuelve a la rotacion inicial
				RestartCameraRotation();
			} else {
				// Nota:
				// Al usar el mismo boton para que haga dos o mas rotaciones
				// esas rotaciones se realizan al tiempo. La unica solucion por ahora
				// es combinarla con otra tecla, de modo que no coincidan.

				if (md.gp_Cam_UpDown == md.gp_Cam_LeftRight) {
					if (!md.gp_mixCamRotAxisX || (md.gp_mixCamRotAxisX && XCI.GetButton(md.gp_mixButtonCamRotAxisX))) {
						xRot = XCI.GetAxis(md.gp_Cam_UpDown) * sticksSensitivity;
						Xon = Mathf.Abs (XCI.GetAxis(md.gp_Cam_UpDown));
						yRot = 0f;
						Yon = 0f;
					}
					
					if (!md.gp_mixCamRotAxisY || (md.gp_mixCamRotAxisY && XCI.GetButton(md.gp_mixButtonCamRotAxisY))) {
						yRot = XCI.GetAxis(md.gp_Cam_LeftRight) * sticksSensitivity;
						Yon  = Mathf.Abs (XCI.GetAxis(md.gp_Cam_LeftRight));
						xRot = 0f;
						Xon  = 0f;
					}
				}
				
				if (md.gp_Cam_UpDown == md.gp_Cam_zRotation) {
					if (!md.gp_mixCamRotAxisX || (md.gp_mixCamRotAxisX && XCI.GetButton(md.gp_mixButtonCamRotAxisX))) {
						xRot = XCI.GetAxis(md.gp_Cam_UpDown) * sticksSensitivity;
						Xon = Mathf.Abs (XCI.GetAxis(md.gp_Cam_UpDown));
						zRot = 0f;
						Zon = 0f;
					}
					
					if (!md.gp_mixCamRotAxisZ || (md.gp_mixCamRotAxisZ && XCI.GetButton(md.gp_mixButtonCamRotAxisZ))) {
						zRot = XCI.GetAxis(md.gp_Cam_zRotation) * sticksSensitivity;
						Zon  = Mathf.Abs (XCI.GetAxis(md.gp_Cam_zRotation));
						xRot = 0f;
						Xon  = 0f;
					}
				}
				
				if (md.gp_Cam_LeftRight == md.gp_Cam_zRotation){
					if (!md.gp_mixCamRotAxisY || (md.gp_mixCamRotAxisY && XCI.GetButton(md.gp_mixButtonCamRotAxisY))) {
						yRot = XCI.GetAxis(md.gp_Cam_LeftRight) * sticksSensitivity;
						Yon  = Mathf.Abs (XCI.GetAxis(md.gp_Cam_LeftRight));
						zRot = 0f;
						Zon  = 0f;
					}
					
					if (!md.gp_mixCamRotAxisZ || (md.gp_mixCamRotAxisZ && XCI.GetButton(md.gp_mixButtonCamRotAxisZ))) {
						zRot = XCI.GetAxis(md.gp_Cam_zRotation) * sticksSensitivity;
						Zon  = Mathf.Abs (XCI.GetAxis(md.gp_Cam_zRotation));
						yRot = 0f;
						Yon  = 0f;
					}
				}

				if (Xon > .05){
					rotationX -= xRot; // poner += si se quiere invertir el uso del joystick
					rotationX = Mathf.Clamp(rotationX, -maximumX, maximumX);
					transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
				} 
				
				if (Yon > .05){
					rotationY += yRot;
					rotationY = Mathf.Clamp (rotationY, -maximumY, maximumY);
					transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, transform.localEulerAngles.z);
				} 
				
				if (Zon > .05) {
					rotationZ -= zRot; // poner += si se quiere invertir el uso del joystick
					rotationZ = Mathf.Clamp (rotationZ, -maximumZ, maximumZ);
					transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, rotationZ);
				}
			}

			// Zoom
			if(camFieldOfView >= camFieldOfViewMax)
				camFieldOfView = camFieldOfViewMax;
			// Zoom 
			if(camFieldOfView <= camFieldOfViewMin)
				camFieldOfView = camFieldOfViewMin;

			float zoomInGamePad  =  Mathf.Abs( XCI.GetAxis(md.gp_zoomIn)) * sticksSensitivity;
			float zoomOutGamePad =  Mathf.Abs (XCI.GetAxis(md.gp_zoomOut))* sticksSensitivity;

			if (zoomInGamePad > 0.05 && zoomInGamePad < 1) {
				// Zoom in
				camFieldOfView -= zoomSpeed * Time.smoothDeltaTime;
			} else if (zoomOutGamePad > 0.05 && zoomOutGamePad < 1) {
				// Zoom out
				camFieldOfView += zoomSpeed * Time.smoothDeltaTime;
			} 
			// Ajustar el Zoom
			camPlayer.fieldOfView = camFieldOfView;
		}
		
	}

}
