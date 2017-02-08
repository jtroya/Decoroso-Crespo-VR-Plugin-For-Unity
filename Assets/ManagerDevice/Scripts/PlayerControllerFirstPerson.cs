/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Script para moverse en la escena creada en 1ra persona
* usando el teclado (flechas de direccion y las teclas w, a, s, d).
* 
* Desde en Manager Device se debe asociar un game object con el cual
* podra interactuar en la escena.
* 
* 1ra persona: El game object asociado debera tener cualquiera de
* las siguientes propiedades: Character Controller o Rigid Body.
* 
* 3ra persona: El game object asociado debera tener la propiedad:
* Rigid Body y Collider (Sphere, Capsule, Box, etc.).
*  
* Stereo Wall: Para usar este dispositivo se recomienda quitar la
* etiqueta (tag) MainCamera de cualquier camara creada en la escena.
\*******************************************************************/

using UnityEngine;
using System.Collections;

public class PlayerControllerFirstPerson : MonoBehaviour {
	// Velocidad de desplazamiento
	public float	speed; //6f;
	// Velocidad de rotacion
	public float	rotationSpeed = 55f;
	// Sensibilidad
	public float 	sensitivity = 1f;
	// Valor para hacer mas lento el movimiento
	public float slowMoveFactor = -0.75f;
	// Valor para hacer mas rapido el movimiento
	public float fastMoveFactor = 2f;
	
	// Referencia a la propiedad rigidbody del game object de referencia
	//private Rigidbody	playerRigidbody;	
	private Transform	_camera;
	private ManagerDevice md;
	//private float pitchValue = 0f;
	//private float yawValue = 0f; 
	private float rollValue = 0f;
	// Constante para rotaciones en los ejes X,Y,Z
	private float constanteGiro = 0.3f;
	private float yStartRotation = 0f;
	private float xStartRotation = 0f;
	private float zStartRotation = 0f;

	
	
	void Awake () {
		md = ManagerDevice.instance;


	}

	void Start () {
		_camera = Camera.main.transform;
		// Verificar si esta asignada la etiqueta correspondiente
		if (_camera == null){
			Debug.LogWarning("Manager Device · Warning.\nYou should set the tag MainCamera the corresponding Game Object");
			return;
		} 

		// Almacena la rotacion inicial del player
		xStartRotation = transform.eulerAngles.x;
		yStartRotation = transform.eulerAngles.y;
		zStartRotation = transform.eulerAngles.z;
	}
	
	
	void Update () {
		// Rotacion eje X
		if (Input.GetKey(md.kb_pitch) && Input.GetKey(md.kb_Up)) { 
			//transform.Rotate(pitchValue, 0 , 0);
			transform.Rotate(-Vector3.right, rotationSpeed * Time.deltaTime);
		} else if (Input.GetKey(md.kb_pitch) &&  Input.GetKey(md.kb_Down)) {
			//transform.Rotate(pitchValue, 0 , 0);
			transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);

		} else if (Input.GetKey(md.kb_yaw) && Input.GetKey(md.kb_Right)) {
			// Rotacion eje Y
			transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
		} else if (Input.GetKey(md.kb_yaw) && Input.GetKey(md.kb_Left)) {
			transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);

		} else if (Input.GetKey(md.kb_roll) && Input.GetKey(md.kb_Right)) {
			// Rotation eje Z
			rollValue = constanteGiro * sensitivity;
			transform.Rotate(0, 0, -rollValue);
		} else if (Input.GetKey(md.kb_roll) && Input.GetKey(md.kb_Left)) {
			rollValue = constanteGiro * sensitivity * -1;
			transform.Rotate(0, 0, -rollValue);

		} else if (md.resetAvatarRotation && Input.GetKey(md.kb_ResetAvatarRotation)) {
			// Volver a la rotacion inicial
			transform.localEulerAngles = new Vector3(xStartRotation, yStartRotation, zStartRotation);

		} else {
			// Para mover mas rapido o mas lento el player
			if (Input.GetKey(md.kb_Run) && Input.GetKey(md.kb_Up)){
				// mas rapido hacia adelante
				transform.Translate(Vector3.forward * (speed * fastMoveFactor)* Time.deltaTime);

			} else if (Input.GetKey(md.kb_Run) && Input.GetKey(md.kb_Down)) {
				// mas rapido hacia atras
				transform.Translate(-Vector3.forward * (speed * fastMoveFactor)* Time.deltaTime);

			} else if (Input.GetKey(md.kb_Slow) && Input.GetKey(md.kb_Up)) {
				// mas lento hacia adelante
				transform.Translate(Vector3.forward * (speed * slowMoveFactor)* Time.deltaTime);

			} else if (Input.GetKey(md.kb_Slow) && Input.GetKey(md.kb_Down)){
				// mas lento hacia atras
				transform.Translate(-Vector3.forward * (speed * slowMoveFactor)* Time.deltaTime);
			}

			// Go up - Go Down
			if (Input.GetKey(md.kb_Y_Up)) {
				transform.position += transform.up * speed * Time.deltaTime;
			}else if (Input.GetKey(md.kb_Y_Down)) {
				transform.position -= transform.up * speed * Time.deltaTime;
			}

			// Movimientos en el plano
			if (Input.GetKey(md.kb_Up)){
				transform.Translate(Vector3.forward * speed * Time.deltaTime);
			}
			
			if (Input.GetKey(md.kb_Down)){		
				transform.Translate(-Vector3.forward * speed * Time.deltaTime);
			}
			
			if (Input.GetKey(md.kb_Right)){
				//transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
				transform.Translate(Vector3.right * speed * Time.deltaTime);
			}
			
			if (Input.GetKey(md.kb_Left)){
				//transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
				transform.Translate(Vector3.left * speed * Time.deltaTime);
			}

		}
	}
}

