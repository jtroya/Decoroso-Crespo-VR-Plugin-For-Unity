/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Descripcion: Script que permite mover un gameobject con el raton.
* Se adjunta en tiempo de ejecucion sobre los gameobjects que estan
* en el array de gameobjects a mover con el raton y cuando se usa
* el metodo AddGameObjectToMoveMouse(gameobject go)
* 
\*******************************************************************/

using UnityEngine;
using System.Collections;

public class DragObject : MonoBehaviour {

	private Vector3 dist;
	private float posX;
	private float posY;
//	private GameObject targetRigidBody = null;

	public float range = 5f;
	public LayerMask layerMask = -1;
	

	void OnMouseDown(){

		dist = Camera.main.WorldToScreenPoint(transform.position);
		posX = Input.mousePosition.x - dist.x;
		posY = Input.mousePosition.y - dist.y;
		
	}

	void OnMouseDrag(){
		Vector3 curPos = 
			new Vector3(Input.mousePosition.x - posX, 
			            Input.mousePosition.y - posY, dist.z);  
		
		//Debug.Log("Drag Something");
		//if (targetRigidBody != null) {
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);
			transform.position = worldPos;
		//}	
	}


}
