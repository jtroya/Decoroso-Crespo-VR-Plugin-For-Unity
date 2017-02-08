/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Script para mover la camara principal en 1ra persona
* de una manera muy simple. Ya que se ajusta el movimiento segun la
* posicion del gameobject a seguir.
* 
* Se debe asociar un game object como referencia y que representa
* al actor principal. Este game object debe tener las propiedades
* rigidbody, collider y/o character controller.
* 
* La camara se ubica en la parte superior del gameobject asociado.
\*******************************************************************/

using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class CameraController : MonoBehaviour {

	public Transform target;            	// Gameobject al que se seguira
	public float rotationSpeed = 5f;        	// Velocidad con la que la camara hara la rotacion
	//public float damping = 3.0f;  			// Velocidad para la posicion
	//public float rotationDamping = 40.0f; 	// Velocidad para la rotacion
	public float sticksSensitivity = 2f;
	public float camFieldOfView = 60f;
	public float zoomSpeed = 60f;
	public float zoomFactor = 6f;
	public float maximumX; //45f
	public float maximumY; //75f
	public float maximumZ;

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



	void Awake() {
		md = ManagerDevice.instance;
	}

	// Use this for initialization
	void Start () {
		// Verificar si se ha asociado un gameobject
		if (target == null) {
			Debug.LogWarning("Manager Device · Warning.\nYou should attach a GameObject in the 'Object to Follow' field");
			return;
		} else {
			// Calcular su altura o dimension equivalente (local scale en Y)
			//Debug.Log("Escala en Y: " + target.lossyScale.y);
			// Ubicar la camara en la parte superior del avatar
			transform.position = new Vector3 (target.position.x, target.position.y + target.lossyScale.y, target.position.z);
			// Ajustar la rotación  de la camara igual a la rotacion del target
			transform.rotation = target.rotation;

			// Almacena la rotacion inicial de la camara
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
			//Debug.Log("Maximo zoom in " + camFieldOfViewMin.ToString());
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

	void LateUpdate  () {
		// Establece la posición del target
		//Vector3 targetCamPos = new Vector3 (target.position.x, target.position.y + target.lossyScale.y, target.position.z); 

		// Angulos de rotacion de la camara sobre los ejes X,Y
		Vector3 rightRotation = new Vector3(0, maximumY, 0);
		Vector3 leftRotation = new Vector3(0, -maximumY, 0);
		Vector3 upRotation = new Vector3(-maximumX , 0, 0);
		Vector3 downRotation = new Vector3(maximumX , 0, 0);
		Vector3 zAxisRotationRight = new Vector3(0, 0, maximumZ);
		Vector3 zAxisRotationLeft = new Vector3(0, 0, -maximumZ);

		// Si se navega con el teclado
		if (md.VarNavigationDevices == ManagerDevice.NavigationDevices.Keyboard) {
			// Girar la camara sobre los ejes X,Y,Z
			if (Input.GetKey(md.kb_Cam_Left)) {
				//transform.localEulerAngles = leftRotation; 
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(leftRotation), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Right)) {
				//transform.localEulerAngles = rightRotation;
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rightRotation), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Up)) {
				//transform.localEulerAngles = upRotation;
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(upRotation), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Down)) {
				//transform.localEulerAngles = downRotation;
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(downRotation), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Rot_Z_Left)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(zAxisRotationLeft), rotationSpeed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Cam_Rot_Z_Right)) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(zAxisRotationRight), rotationSpeed * Time.deltaTime);
			} //else {
				// Ajusta la rotacion de la camara a la del target
				//transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotationSpeed * Time.deltaTime);
				
			//}

			// Ajusta la posicion de la camara a la del target
			//transform.position = Vector3.Lerp(transform.position, targetCamPos, speed * damping * Time.deltaTime);

			// Si pulsa la tecla asignada vuelve a la rotacion inicial
			if (md.resetCameraRotation && Input.GetKey(md.kb_ResetCameraRotation)) {	
				//transform.position = targetCamPos;
				RestartCameraRotation();
			}
			//transform.position = targetCamPos;

			// Zoom inicial
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
			//yRot = ((!md.gp_mixCamRotAxisY) || (md.gp_mixCamRotAxisY && XCI.GetButton(md.gp_mixButtonCamRotAxisY))) ? XCI.GetAxis(md.gp_Cam_LeftRight) * sticksSensitivity : 0f;
			//zRot = ((!md.gp_mixCamRotAxisZ) || (md.gp_mixCamRotAxisZ && XCI.GetButton(md.gp_mixButtonCamRotAxisZ))) ? XCI.GetAxis(md.gp_Cam_zRotation) * sticksSensitivity : 0f;

			xRot = XCI.GetAxis(md.gp_Cam_UpDown) * sticksSensitivity;
			yRot = XCI.GetAxis(md.gp_Cam_LeftRight) * sticksSensitivity;
			zRot = XCI.GetAxis(md.gp_Cam_zRotation) * sticksSensitivity;

			float Xon = Mathf.Abs (XCI.GetAxis(md.gp_Cam_UpDown));
			float Yon = Mathf.Abs (XCI.GetAxis(md.gp_Cam_LeftRight));
			float Zon = Mathf.Abs (XCI.GetAxis(md.gp_Cam_zRotation));


			if (md.resetCameraRotation && XCI.GetButton(md.gp_ResetCameraRotation)){
				// Vuelve a la rotacion inicial
				RestartCameraRotation();
			} else {
				// Aplicar las rotaciones

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
			if(camFieldOfView > camFieldOfViewMax)
				camFieldOfView = camFieldOfViewMax;
			// Zoom 
			if(camFieldOfView < camFieldOfViewMin)
				camFieldOfView = camFieldOfViewMin;

			float zoomInGamePad  =  Mathf.Abs( XCI.GetAxis(md.gp_zoomIn)) * sticksSensitivity;
			float zoomOutGamePad =  Mathf.Abs (XCI.GetAxis(md.gp_zoomOut))* sticksSensitivity;

			if (zoomInGamePad > 0.05 && zoomInGamePad < 1) {
				// Zoom in
				camFieldOfView -= zoomSpeed * Time.smoothDeltaTime;
			} 
			/****/
			else if (zoomOutGamePad > 0.05 && zoomOutGamePad < 1) {
				// Zoom out
				camFieldOfView += zoomSpeed * Time.smoothDeltaTime;
			} 
			/***
			else {
				camFieldOfView = camPlayer.fieldOfView;
			}

			***/
			// Ajustar el Zoom
			camPlayer.fieldOfView = camFieldOfView;


		}
	}
}
