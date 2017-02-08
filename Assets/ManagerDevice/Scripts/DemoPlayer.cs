using UnityEngine;
using System.Collections;

public class DemoPlayer : MonoBehaviour {

	public float speed = 10;
	public float turnSpeed = 60;
	
	void Update() {
		float horizontal = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
		transform.Rotate(0, horizontal, 0);
		
		float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
		transform.Translate(0, 0, vertical);
	}
}
