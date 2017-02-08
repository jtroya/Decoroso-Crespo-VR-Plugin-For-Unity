using UnityEngine;
using System.Collections;

public class MouseDragDrop : MonoBehaviour {

	private GameObject grabbedObject;
	private float grabbedObjectSize;
	private GameObject cameraMain;

	public float range = 5f;

	void Start () {
		cameraMain = GameObject.FindWithTag("MainCamera");
		if (cameraMain = null) {
			Debug.LogWarning("Manager Device · Mouse Manipulation · Warning\nYou should set the tag MainCamera the corresponding Game Object");
		}
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log(GetMouseHoverObject(5));

		if (Input.GetMouseButtonDown(0)) {
			if (grabbedObject == null)
				TryGrabObject(GetMouseHoverObject(range));
			else
				DropObject();
		}

		if (grabbedObject != null) {
			Vector3 newPosition = gameObject.transform.position + Camera.main.transform.forward * grabbedObjectSize;
			grabbedObject.transform.position = newPosition;
		}


	}

	GameObject GetMouseHoverObject (float range) {
		Vector3 position = gameObject.transform.position;
		RaycastHit rayCastHit;
		Vector3 target = position + Camera.main.transform.forward * range;
		
		if (Physics.Linecast(position, target, out rayCastHit))
			return rayCastHit.collider.gameObject;
		
		return null;
	}
	
	void TryGrabObject (GameObject grabObject) {
		if (grabObject == null || !CanGrab(grabObject))
			return;
		
		grabbedObject = grabObject;
		grabbedObjectSize = grabbedObject.GetComponent<Renderer>().bounds.size.magnitude;
	}
	
	bool CanGrab(GameObject candidate) {
		return candidate.GetComponent<Rigidbody>() != null;
		
	}
	
	void DropObject () {
		if (grabbedObject == null)
			return;
		
		if (grabbedObject.GetComponent<Rigidbody>() != null)
			grabbedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		
		grabbedObject = null;
	}
}
