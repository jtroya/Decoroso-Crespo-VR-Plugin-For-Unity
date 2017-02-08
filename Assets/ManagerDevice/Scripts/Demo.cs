using UnityEngine;
using System.Collections;

[System.Serializable]
public class Demo : MonoBehaviour {


	public GameObject[] __mouse_moveGameObjects = new GameObject[1];

	public GameObject this [int i] {
		get {
			return __mouse_moveGameObjects [i];
		}

		set {
			__mouse_moveGameObjects [i] = value;
		}
	}

	public int Length {
		get {
			return __mouse_moveGameObjects.Length;
		}
	}
}
