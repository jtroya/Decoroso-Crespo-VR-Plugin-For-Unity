/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Script que se usa con Leap Motion para obtener los 
* datos de posicion de cada dedo. Los datos que se obtienen para 
* hacer seguimiento son: Pitch, Yaw, Roll y Basis.
\*******************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

public class LeapTrackingInfo : MonoBehaviour {

	private Controller 	leapController;
	private FingerList 	fingersInFrame;	
	private FingerList	listFinger;
	private string		debugText;
	private string		separatorText = "\n";
	private GameObject	go_textDebugLeft;
	private GameObject	go_textDebugRight;
	private float   	debugTextRightPosition = 0.85f;
	private Vector2		offSetText = new Vector2 (15f, 15f);

	public 	bool 		isHeadMounted = false;
	
	void Start () {
		leapController = new Controller();
		/***
		go_debugText = GameObject.Find("GUI-Debug");
		if (go_debugText != null) {
			go_debugText.GetComponent<GUIText>().text = "Starting debug";
		}
		***/
		// Debug mano izquierda
		go_textDebugLeft = new GameObject();
		go_textDebugLeft.name = "LeapDebugInfoLeft";
		go_textDebugLeft.AddComponent<GUIText>();
		go_textDebugLeft.GetComponent<GUIText>().anchor = TextAnchor.LowerLeft;
		go_textDebugLeft.GetComponent<GUIText>().alignment = TextAlignment.Left;
		go_textDebugLeft.GetComponent<GUIText>().fontSize = 14;
		go_textDebugLeft.GetComponent<GUIText>().fontStyle = FontStyle.Normal;
		go_textDebugLeft.GetComponent<GUIText>().richText = true;
		go_textDebugLeft.GetComponent<GUIText>().pixelOffset = offSetText;

		// Debug mano derecha
		go_textDebugRight = new GameObject();
		go_textDebugRight.name = "LeapDebugInfoRight";
		go_textDebugRight.transform.position = new Vector3 (debugTextRightPosition, 0f, 0f);
		go_textDebugRight.AddComponent<GUIText>();
		go_textDebugRight.GetComponent<GUIText>().anchor = TextAnchor.LowerRight;
		go_textDebugRight.GetComponent<GUIText>().alignment = TextAlignment.Right;
		go_textDebugRight.GetComponent<GUIText>().fontSize = 14;
		go_textDebugRight.GetComponent<GUIText>().fontStyle = FontStyle.Normal;
		go_textDebugRight.GetComponent<GUIText>().richText = true;
		go_textDebugRight.GetComponent<GUIText>().pixelOffset = offSetText;
	}
	
	// Return the hand tracking info
	Hashtable GetHandInfo (Hand hand, Frame frame) {
//		HandList 	hands = frame.Hands;
		Hashtable	handInfo = new Hashtable();
		
		handInfo.Add("pitch", hand.Direction.Pitch);
		handInfo.Add("yaw", hand.Direction.Yaw);
		handInfo.Add("roll", hand.PalmNormal.Roll);
		handInfo.Add("basis", hand.Basis);
		
		return handInfo;
	}
	
	// Return the fingers tracking info
	Hashtable GetFingersInfo (Hand hand, Frame frame) {
//		HandList 	hands = frame.Hands;
		Hashtable	fingerInfo = new Hashtable();
		FingerList 	listaDedos = new FingerList();
		Finger 		dedo = new Finger();
		
		listaDedos = hand.Fingers.FingerType(Finger.FingerType.TYPE_THUMB);
		dedo = listaDedos[0];
		fingerInfo.Add("thumb", dedo.TipPosition);
		
		listaDedos = hand.Fingers.FingerType(Finger.FingerType.TYPE_INDEX);
		dedo = listaDedos[0];
		fingerInfo.Add("index", dedo.TipPosition);
		
		listaDedos = hand.Fingers.FingerType(Finger.FingerType.TYPE_MIDDLE);
		dedo = listaDedos[0];
		fingerInfo.Add("middle", dedo.TipPosition);
		
		listaDedos = hand.Fingers.FingerType(Finger.FingerType.TYPE_RING);
		dedo = listaDedos[0];
		fingerInfo.Add("ring", dedo.TipPosition);
		
		listaDedos = hand.Fingers.FingerType(Finger.FingerType.TYPE_PINKY);
		dedo = listaDedos[0];
		fingerInfo.Add("pinky", dedo.TipPosition);
		
		return fingerInfo;
	}
	
	
	
	void Update () {
		Frame frame = leapController.Frame();
		HandList hands = frame.Hands;
		Hashtable rHandInfo = new Hashtable();
		Hashtable lHandInfo = new Hashtable();
		Hashtable rFingerInfo = new Hashtable();
		Hashtable lFingerInfo = new Hashtable();
		
		
		// tracking data in the frame
		if (frame.IsValid){ 
			foreach (Hand hand in hands) {
				if (hand.IsRight) {
					rFingerInfo = GetFingersInfo(hand, frame);
					rHandInfo = GetHandInfo(hand, frame);
					
				} else {
					lFingerInfo = GetFingersInfo(hand, frame);
					lHandInfo = GetHandInfo(hand, frame);
				}
			}

			// debug the tracking info
			if (lHandInfo.Count > 0){
				debugText = "Left Hand" + separatorText;
				debugText += "Pitch Left Hand: " + string.Format("0.00", lHandInfo["pitch"]) + separatorText;
				debugText += "Yaw Left Hand: " + string.Format("0.00",lHandInfo["yaw"]) + separatorText;
				debugText += "Roll Left Hand: " + string.Format("0.00",lHandInfo["roll"]) + separatorText;
				debugText += "Basis Left Hand: " + string.Format("0.00",lHandInfo["basis"]) + separatorText;
				/***
				Debug.Log("Pitch Left Hand= " + lHandInfo["pitch"].ToString());
				Debug.Log("Yaw Left Hand= " + lHandInfo["yaw"].ToString());
				Debug.Log ("Roll Left Hand= " + lHandInfo["roll"].ToString());
				Debug.Log ("Basis Left Hand= " + lHandInfo["basis"].ToString());
				***/
				go_textDebugLeft.GetComponent<GUIText>().text = debugText;
			}


			if (rHandInfo.Count > 0) {
				debugText = "Right Hand" + separatorText;
				debugText += "Pitch Right Hand= " + rHandInfo["pitch"].ToString() + separatorText;
				debugText += "Yaw Right Hand= " + rHandInfo["yaw"].ToString() + separatorText;
				debugText += "Roll Right Hand= " + rHandInfo["roll"].ToString() + separatorText;
				debugText += "Basis Right Hand: " + rHandInfo["basis"].ToString() + separatorText;
				/***
				Debug.Log("Pitch Right Hand= " + rHandInfo["pitch"].ToString());
				Debug.Log("Yaw Right Hand= " + rHandInfo["yaw"].ToString());
				Debug.Log ("Roll Right Hand= " + rHandInfo["roll"].ToString());
				Debug.Log ("Basis Right Hand= " + rHandInfo["basis"].ToString());
				***/
				go_textDebugRight.GetComponent<GUIText>().text = debugText;
			}

		}
	}
}
