/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Script para moverse en la escena creada en 3ra persona.
* Toma como referencia el game object donde se ha agregado en tiempo de ejecucion.
* Este game object debe tener las propiedades: rigidbody y collider. 
* El script toma la posicion de la camara principal y hace el seguimiento
* con los valores de las entradas Horizontal y Vertical, segun las teclas
* que se hayan establecido para el desplazamiento.
\*******************************************************************/

using UnityEngine;
using System.Collections;

public class PlayerControllerThirdPerson : MonoBehaviour {

	public float speed;
	public float turnSpeed = 60;
	// Valor para hacer mas lento el movimiento
	public float slowMoveFactor = -0.75f;
	// Valor para hacer mas rapido el movimiento
	public float fastMoveFactor = 2f;


	private ManagerDevice md;
	// Constante para movimiento
	private float constanteMov= 0.8f;
	private float yStartRotation = 0f;
	private float xStartRotation = 0f;
	private float zStartRotation = 0f;

	void Awake() {
		md = ManagerDevice.instance;
	}


	void Start () {
		// Almacena la rotacion inicial del player
		xStartRotation = transform.eulerAngles.x;
		yStartRotation = transform.eulerAngles.y;
		zStartRotation = transform.eulerAngles.z;
	}

	void Update() {

		// Rotacion eje X
		if (Input.GetKey(md.kb_pitch) && Input.GetKey(md.kb_Up)) { 
			transform.Rotate(-Vector3.right, turnSpeed * Time.deltaTime);
		} else if (Input.GetKey(md.kb_pitch) &&  Input.GetKey(md.kb_Down)) {
			transform.Rotate(Vector3.right, turnSpeed * Time.deltaTime);
		
		} else if (Input.GetKey(md.kb_yaw) && Input.GetKey(md.kb_Right)) {
			// Rotacion eje Y
			transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
		} else if (Input.GetKey(md.kb_yaw) && Input.GetKey(md.kb_Left)) {
			transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
		
		} else if (Input.GetKey(md.kb_roll) && Input.GetKey(md.kb_Right)) {
			// Rotation eje Z
			//rollValue = constanteGiro * sensitivity;
			//transform.Rotate(0, 0, -rollValue);
			transform.Rotate(-Vector3.forward, turnSpeed * Time.deltaTime);
		} else if (Input.GetKey(md.kb_roll) && Input.GetKey(md.kb_Left)) {
			//rollValue = constanteGiro * sensitivity * -1;
			//transform.Rotate(0, 0, -rollValue);
			transform.Rotate(Vector3.forward, turnSpeed * Time.deltaTime);
		
		} else if (md.resetAvatarRotation && Input.GetKey(md.kb_ResetAvatarRotation)) {
			// Volver a la rotacion inicial
			transform.localEulerAngles = new Vector3(xStartRotation, yStartRotation, zStartRotation);

		} else {
			// Mover mas rapido o mas lento el player
			if (Input.GetKey(md.kb_Run) && Input.GetKey(md.kb_Up)){
				float vertical = constanteMov * (speed * fastMoveFactor) * Time.deltaTime;
				transform.Translate(0, 0, vertical);

			} else if (Input.GetKey(md.kb_Run) && Input.GetKey(md.kb_Down)){
				float vertical = constanteMov * (speed * fastMoveFactor) * Time.deltaTime * -1;
				transform.Translate(0, 0, vertical);

			} else if (Input.GetKey(md.kb_Slow) && Input.GetKey(md.kb_Up)) {
				float vertical = constanteMov * (speed * slowMoveFactor) * Time.deltaTime;
				transform.Translate(0, 0, vertical);

			} else if (Input.GetKey(md.kb_Slow) && Input.GetKey(md.kb_Down)){
				float vertical = constanteMov * (speed * slowMoveFactor) * Time.deltaTime * -1;
				transform.Translate(0, 0, vertical);
			}

			// Go up - Go Down
			if (Input.GetKey(md.kb_Y_Up)) {
				//transform.position += transform.up * climbSpeed * Time.deltaTime;
				transform.position += transform.up * speed * Time.deltaTime;
			}else if (Input.GetKey(md.kb_Y_Down)) {
				transform.position -= transform.up * speed * Time.deltaTime;
			}


			// Movimiento en el plano
			if (Input.GetKey(md.kb_Up)){
				float vertical = constanteMov * speed * Time.deltaTime;
				transform.Translate(0, 0, vertical);
			} else if (Input.GetKey(md.kb_Down)){
				float vertical = constanteMov * speed * Time.deltaTime * -1;
				transform.Translate(0, 0, vertical);
			}

			if (Input.GetKey(md.kb_Right)){
				//float horizontal = constanteMov * turnSpeed * Time.deltaTime;
				//transform.Rotate(0, horizontal, 0);
				transform.Translate(Vector3.right * speed * Time.deltaTime);
			} else if (Input.GetKey(md.kb_Left)){
				//float horizontal = constanteMov * turnSpeed * Time.deltaTime * -1;
				//transform.Rotate(0, horizontal, 0);
				transform.Translate(Vector3.left * speed * Time.deltaTime);
			}
		}
	}
}
