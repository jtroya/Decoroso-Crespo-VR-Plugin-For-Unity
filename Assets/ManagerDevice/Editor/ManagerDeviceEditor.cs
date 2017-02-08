/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Este script es el editor personalizado para la clase
* ManagerDevice.cs
\*******************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using XboxCtrlrInput;


[CustomEditor (typeof (ManagerDevice))]
public class ManagerDeviceEditor : Editor {

	private ManagerDevice  ControllerDevice;
	private SpaceNavigatorWindow Mouse3Dwindow;
	private bool allowSceneObjects;
	//private bool fixCamRotationZ;
	//private bool resetAvatarRotation;
	//bool foldout1 = true;
	//bool foldout2 = true;

	// Min and Max Rotation speed
	private float minRotationSpeed = 1f;
	private float maxRotationSpeed = 20f;
	// Min and Max Traslation speed
	private float minTranslationSpeed = 6f;
	private float maxTranslationSpeed = 26f;
	// Min angle for camera rotation
	private float minRotAngleCam_X = 15f;
	private float minRotAngleCam_Y = 15f;
	private float minRotAngleCam_Z = 15f;
	// Max angle for camera rotation
	private float maxRotAngleCam_X = 45f;
	private float maxRotAngleCam_Y = 75f;
	private float maxRotAngleCam_Z = 90f;
	// Zoom factor
	private int minZoomFactor = 1;
	private int maxZoomFactor = 10;
	// Min Run and Slow factor
	private float minRunFactor = 2f;
	private float minSlowFactor = -0.75f;
	// Max Run and Slow factor
	private float maxRunFactor = 20f;
	private float maxSlowFactor = 1f;
	// Gameobjects array to move
	private SerializedObject m_Object;
	private SerializedProperty m_Property;


	void OnEnable() { 
		m_Object = new SerializedObject(target); 
	}

	public override void OnInspectorGUI() {
		ControllerDevice = (ManagerDevice) target;
		//Debug.Log("Current detected event: " + Event.current);

		/*************************
		 * Visualization
		 **************************/
		ControllerDevice.VarVisualDevices = (ManagerDevice.VisualizationDevices) EditorGUILayout.EnumPopup("Visualization Device", ControllerDevice.VarVisualDevices);

		// Options for oculus rift
		if (ControllerDevice.VarVisualDevices == ManagerDevice.VisualizationDevices.OculusRift){
			EditorGUILayout.BeginVertical("box");	
			// Help text about Oculus Position
			EditorGUILayout.HelpBox("The OVR position will use the position X,Y,Z of Manager Device in this scene.\n" +
				"Change the Manager Device Position to set OVR position.", MessageType.Info);
			ControllerDevice.Ovr_PlayerController = EditorGUILayout.Toggle("Player Controller", ControllerDevice.Ovr_PlayerController);
			ControllerDevice.Ovr_CameraRig = EditorGUILayout.Toggle("Camera Rig", ControllerDevice.Ovr_CameraRig);
			// the player controller and camera rig are mutually exclusive
			if (ControllerDevice.Ovr_PlayerController){
				ControllerDevice.Ovr_CameraRig = false;
			}
			EditorGUILayout.EndVertical();
		}

		// Options for stereo wall
		if (ControllerDevice.VarVisualDevices == ManagerDevice.VisualizationDevices.StereoWall){
			// Because the StereoWall Script controller is in JavaScript (s3dCameraEditor), it is not possible
			// to setup on runtime. It is necessary setup on design time.
			
			// Button for apply the changes in design
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button ("Apply Changes", GUILayout.Height(25), GUILayout.ExpandWidth(true))) {
				// Verificar si existe otro objeto con el mismo nombre
				if (GameObject.Find("StereoWall_Camera")){
					Debug.LogWarning("Manager Device · StereoWall · Warning\nThe GameObject <<StereoWall_Camera>> is on the scene.");

					if (EditorUtility.DisplayDialog("Manager Device · StereoWall · Warning","The GameObject 'StereoWall_Camera' is on the scene", "Ok")) {
						return;
					}

					//EditorGUILayout.HelpBox("Manager Device · StereoWall · Warning\nThe GameObject <<StereoWall_Camera>> is on the scene.", MessageType.Warning );
					//EditorGUILayout.Space ();
				} else {
					// Si existe indicar con un mensaje, de lo contrario instanciar el prefab
					Object nameObject = AssetDatabase.LoadAssetAtPath("Assets/ManagerDevice/Prefabs/StereoWallCamera.prefab", typeof(GameObject));
					GameObject stereoWallObj = Instantiate(nameObject, new Vector3(0,0,0), Quaternion.identity) as GameObject;
					stereoWallObj.name = "StereoWall_Camera";
				}
			}
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.HelpBox("Manager Device · Information\nPress <<Apply Changes>> button to add the component to the scene.", MessageType.Info );
			//EditorGUILayout.HelpBox("You must to go to menu Edit - Project Settings - Player \nIn panel PC, Mac & Linux set the values for resolution\nDefault Screen Widh: 3840 \nDefault Screen Height:1080", MessageType.Info );
		}

		// Opciones de punto de vista 1ra o 3ra persona
		// Object to follow
		// Rotation speed
		EditorGUILayout.BeginVertical("box");
		if ((ControllerDevice.VarVisualDevices == ManagerDevice.VisualizationDevices.OculusRift && ControllerDevice.Ovr_CameraRig) ||
		    (ControllerDevice.VarVisualDevices != ManagerDevice.VisualizationDevices.OculusRift)) {
			// Device different Oculus
			if (ControllerDevice.VarVisualDevices != ManagerDevice.VisualizationDevices.OculusRift)
				ControllerDevice.VarKeyboardNavPerson = (ManagerDevice.keyboardNavigationPerson) EditorGUILayout.EnumPopup("Point of View", ControllerDevice.VarKeyboardNavPerson);
			// Game object that will be tracked by the main camera
			allowSceneObjects = !EditorUtility.IsPersistent (ControllerDevice);
			ControllerDevice.keyboard_ObjectFollow = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Object to Follow", "Assing a GameObject to Follow"), ControllerDevice.keyboard_ObjectFollow, typeof(GameObject), allowSceneObjects);
		}

		// Rotation Speed
		ControllerDevice.cam_maxRotationSpeed = EditorGUILayout.Slider("Rotation Speed", ControllerDevice.cam_maxRotationSpeed, minRotationSpeed, maxRotationSpeed);
		EditorGUILayout.EndVertical();


		// Options for Camera rotations with keyboard
		if ((ControllerDevice.VarVisualDevices == ManagerDevice.VisualizationDevices.Display || 
		     ControllerDevice.VarVisualDevices == ManagerDevice.VisualizationDevices.StereoWall) &&
		    ControllerDevice.VarNavigationDevices == ManagerDevice.NavigationDevices.Keyboard ) {

			// Opciones para controlar la camara con teclado
			GUILayout.Label("Camera Settings - Rotation");

			// For 1st person
			//if (ControllerDevice.VarKeyboardNavPerson == ManagerDevice.keyboardNavigationPerson.FirstPerson) {

			// X axis
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("X Axis");
			ControllerDevice.kb_Cam_Up = (KeyCode) EditorGUILayout.EnumPopup("Camera Turn Up", ControllerDevice.kb_Cam_Up);
			ControllerDevice.kb_Cam_Down = (KeyCode) EditorGUILayout.EnumPopup("Turn Down", ControllerDevice.kb_Cam_Down);
			// Max angle rotation
			ControllerDevice.cam_maxAngleRot_X = EditorGUILayout.Slider("Max Angle", ControllerDevice.cam_maxAngleRot_X, minRotAngleCam_X, maxRotAngleCam_X);
			EditorGUILayout.EndVertical();

			//}

			// Y axis
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Y Axis");
			// For 1st and 3rd person
			ControllerDevice.kb_Cam_Left 	= (KeyCode) EditorGUILayout.EnumPopup("Turn Left", ControllerDevice.kb_Cam_Left);
			ControllerDevice.kb_Cam_Right 	= (KeyCode) EditorGUILayout.EnumPopup("Turn Right", ControllerDevice.kb_Cam_Right);
			// Max angle rotation
			ControllerDevice.cam_maxAngleRot_Y = EditorGUILayout.Slider("Max Angle", ControllerDevice.cam_maxAngleRot_Y, minRotAngleCam_Y, maxRotAngleCam_Y);
			EditorGUILayout.EndVertical();

			// Z axis
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Z Axis");
			ControllerDevice.kb_Cam_Rot_Z_Left = (KeyCode) EditorGUILayout.EnumPopup("Twist Left", ControllerDevice.kb_Cam_Rot_Z_Left);
			ControllerDevice.kb_Cam_Rot_Z_Right = (KeyCode) EditorGUILayout.EnumPopup("Twist Right", ControllerDevice.kb_Cam_Rot_Z_Right);
			// Max angle rotation
			ControllerDevice.cam_maxAngleRot_Z = EditorGUILayout.Slider("Max Angle", ControllerDevice.cam_maxAngleRot_Z, minRotAngleCam_Z, maxRotAngleCam_Z);
			EditorGUILayout.EndVertical();

			// Zoom keys
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Zoom");
			ControllerDevice.kb_Cam_ZoomIn 	= (KeyCode) EditorGUILayout.EnumPopup("Zoom In", ControllerDevice.kb_Cam_ZoomIn);
			ControllerDevice.kb_Cam_ZoomOut = (KeyCode) EditorGUILayout.EnumPopup("Zoom Out", ControllerDevice.kb_Cam_ZoomOut);
			ControllerDevice.cam_zoomFactor = EditorGUILayout.IntSlider("Zoom Factor", ControllerDevice.cam_zoomFactor, minZoomFactor, maxZoomFactor);
			EditorGUILayout.EndVertical();

			// Reset Camera Rotation
			EditorGUILayout.BeginVertical("box");
			//GUILayout.Label("Reset Camera Rotation");
			ControllerDevice.resetCameraRotation = EditorGUILayout.ToggleLeft("Reset Camera Rotation", ControllerDevice.resetCameraRotation);
			if (ControllerDevice.resetCameraRotation) {
				ControllerDevice.kb_ResetCameraRotation = (KeyCode) EditorGUILayout.EnumPopup("Select the Key ", ControllerDevice.kb_ResetCameraRotation);
				EditorGUILayout.HelpBox("With this option, the Camera will set to the start rotation in Design Mode." , MessageType.Info );
			} else {
				ControllerDevice.kb_ResetCameraRotation = KeyCode.None;
			}
			EditorGUILayout.EndVertical();
		}

		// Options for Camera rotations with gamepad
		if ((ControllerDevice.VarVisualDevices == ManagerDevice.VisualizationDevices.Display || 
		     ControllerDevice.VarVisualDevices == ManagerDevice.VisualizationDevices.StereoWall) &&
		    ControllerDevice.VarNavigationDevices == ManagerDevice.NavigationDevices.Gamepad ) {

			GUILayout.Label("Camera Settings - Rotation");					

			// X axis
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("X Axis");
			ControllerDevice.gp_Cam_UpDown	  = (XboxCtrlrInput.XboxAxis) EditorGUILayout.EnumPopup("Turn Up and Down", ControllerDevice.gp_Cam_UpDown);
			// Max angle rotation
			ControllerDevice.cam_maxAngleRot_X = EditorGUILayout.Slider("Max Angle", ControllerDevice.cam_maxAngleRot_X, minRotAngleCam_X, maxRotAngleCam_X);

			// Combine with other button
			ControllerDevice.gp_mixCamRotAxisX = EditorGUILayout.ToggleLeft("Combine with button", ControllerDevice.gp_mixCamRotAxisX);
			if (ControllerDevice.gp_mixCamRotAxisX){
				ControllerDevice.gp_mixButtonCamRotAxisX = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Select the button", ControllerDevice.gp_mixButtonCamRotAxisX);
			}
			EditorGUILayout.EndVertical();

			// Y axis
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Y Axis");
			// For 1st and 3rd person
			ControllerDevice.gp_Cam_LeftRight = (XboxCtrlrInput.XboxAxis) EditorGUILayout.EnumPopup("Turn Left and Right", ControllerDevice.gp_Cam_LeftRight);
			// Max angle rotation
			ControllerDevice.cam_maxAngleRot_Y = EditorGUILayout.Slider("Max Angle", ControllerDevice.cam_maxAngleRot_Y, minRotAngleCam_Y, maxRotAngleCam_Y);
			// Combine with other button
			ControllerDevice.gp_mixCamRotAxisY = EditorGUILayout.ToggleLeft("Combine with button", ControllerDevice.gp_mixCamRotAxisY);
			if (ControllerDevice.gp_mixCamRotAxisY) {
				ControllerDevice.gp_mixButtonCamRotAxisY = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Select the button", ControllerDevice.gp_mixButtonCamRotAxisY);
			}

			EditorGUILayout.EndVertical();

			// Z axis
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Z Axis");
			// For 1st and 3rd person
			ControllerDevice.gp_Cam_zRotation = (XboxCtrlrInput.XboxAxis) EditorGUILayout.EnumPopup("Twist Left and Right", ControllerDevice.gp_Cam_zRotation);
			// Max angle rotation
			ControllerDevice.cam_maxAngleRot_Z = EditorGUILayout.Slider("Max Angle", ControllerDevice.cam_maxAngleRot_Z, minRotAngleCam_Z, maxRotAngleCam_Z);
			// Combine with other button
			ControllerDevice.gp_mixCamRotAxisZ = EditorGUILayout.ToggleLeft("Combine with button", ControllerDevice.gp_mixCamRotAxisZ);
			if (ControllerDevice.gp_mixCamRotAxisZ){
				ControllerDevice.gp_mixButtonCamRotAxisZ = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Select the button", ControllerDevice.gp_mixButtonCamRotAxisZ);
			}
			EditorGUILayout.EndVertical();

			// Zoom keys
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Zoom");
			ControllerDevice.gp_zoomIn 	= (XboxCtrlrInput.XboxAxis) EditorGUILayout.EnumPopup("Zoom In", ControllerDevice.gp_zoomIn);
			ControllerDevice.gp_zoomOut = (XboxCtrlrInput.XboxAxis) EditorGUILayout.EnumPopup("Zoom Out", ControllerDevice.gp_zoomOut);
			ControllerDevice.cam_zoomFactor = EditorGUILayout.IntSlider("Zoom Factor", ControllerDevice.cam_zoomFactor, minZoomFactor, maxZoomFactor);
			EditorGUILayout.EndVertical();

			// Reset Camera Rotation
			EditorGUILayout.BeginVertical("box");
			ControllerDevice.resetCameraRotation = EditorGUILayout.ToggleLeft("Reset Camera Rotation", ControllerDevice.resetCameraRotation);
			if (ControllerDevice.resetCameraRotation) {
				ControllerDevice.gp_ResetCameraRotation = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Select the Button ", ControllerDevice.gp_ResetCameraRotation);	
				EditorGUILayout.HelpBox("With this option, the Camera will set to the start rotation in Design Mode." , MessageType.Info );
			}
			EditorGUILayout.EndVertical();
		}


		EditorGUILayout.Space ();

		// Line Separator
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

		/*************************
		 * Manipulation
		 **************************/

		ControllerDevice.VarManipulationDevices = (ManagerDevice.ManipulationDevices) EditorGUILayout.EnumPopup("Manipulation Device", ControllerDevice.VarManipulationDevices);
		if(ControllerDevice.VarManipulationDevices == ManagerDevice.ManipulationDevices.LeapMotion){
			EditorGUILayout.BeginVertical("box");
			ControllerDevice.Leap_IsHeadMounted 		= EditorGUILayout.Toggle("Is Head Mounted", ControllerDevice.Leap_IsHeadMounted);
			ControllerDevice.Leap_LeftGraphicsModel 	= (HandModel)EditorGUILayout.ObjectField("Left Hand Graphics Model", ControllerDevice.Leap_LeftGraphicsModel, typeof(HandModel), true);
			ControllerDevice.Leap_RightGraphicsModel 	= (HandModel)EditorGUILayout.ObjectField("Right Hand Graphics Model", ControllerDevice.Leap_RightGraphicsModel, typeof(HandModel), true);
			ControllerDevice.Leap_ToolModel				= (ToolModel)EditorGUILayout.ObjectField("Tool Model", ControllerDevice.Leap_ToolModel, typeof(ToolModel), true);
			// Check if is head mounted the oculus rift is selected
			if (ControllerDevice.Leap_IsHeadMounted && ControllerDevice.VarVisualDevices != ManagerDevice.VisualizationDevices.OculusRift){
				EditorGUILayout.HelpBox("If you set Is Head Mounted, you must select Oculus Rift as visualization device", MessageType.Warning );
				
			} else {
				//EditorGUILayout.HelpBox("", MessageType.None);
				EditorGUILayout.Space ();
			}

			// Check if the device will debug the position of the hands
			ControllerDevice.Leap_DebugInfo = EditorGUILayout.Toggle("Debug tracking info", ControllerDevice.Leap_DebugInfo);
			EditorGUILayout.EndHorizontal();
		}

		// Options for mouse
		if (ControllerDevice.VarManipulationDevices == ManagerDevice.ManipulationDevices.Mouse) {
			EditorGUILayout.BeginVertical("box");
	
			m_Property = m_Object.FindProperty("mouse_selectionGameObjects"); 
			EditorGUILayout.PropertyField(m_Property, new GUIContent("Add GameObjects to move"), true, GUILayout.ExpandWidth(true));
			do {
				if (m_Property.propertyPath != "mouse_selectionGameObjects" && !m_Property.propertyPath.StartsWith("mouse_selectionGameObjects" + ".") ) {
					break;
				}
				//EditorGUILayout.PropertyField(m_Property, new GUIContent("MyLabel"), true);
			} while (m_Property.NextVisible(true));

			// Apply the property, handle undo
			m_Object.ApplyModifiedProperties();

			EditorGUILayout.HelpBox("You can drag and drop the gameobjects to be moved into this array, or use the method 'AddGameObjectToMoveMouse(GameObject)' in runtime", MessageType.Info );

			EditorGUILayout.EndHorizontal();
		}

		// Options for optitrack
//		if(ControllerDevice.VarManipulationDevices == ManagerDevice.ManipulationDevices.OptiTrack){
//			EditorGUILayout.BeginVertical("box");
//			ControllerDevice.Opti_IpServer		= EditorGUILayout.TextField("IP Server", ControllerDevice.Opti_IpServer);
//			EditorGUILayout.Space ();
//			ControllerDevice.Opti_vrpnBroadcast	= EditorGUILayout.Toggle("VRPN Broadcast", ControllerDevice.Opti_vrpnBroadcast);
//			ControllerDevice.Opti_vrpnPort		= EditorGUILayout.IntField("VRPN Port", ControllerDevice.Opti_vrpnPort);
//			EditorGUILayout.Space ();
//			ControllerDevice.Opti_MultiCastingPort  = EditorGUILayout.IntField("Multicasting Port", ControllerDevice.Opti_MultiCastingPort);
//			ControllerDevice.Opti_MultiCastingGroup = EditorGUILayout.TextField("Multicasting Group", ControllerDevice.Opti_MultiCastingGroup);
//			EditorGUILayout.EndHorizontal();
//		}

		EditorGUILayout.Space ();

		// Line Separator
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});


		/*************************
		 * Navigation
		 **************************/
		ControllerDevice.VarNavigationDevices = (ManagerDevice.NavigationDevices) EditorGUILayout.EnumPopup("Navigation Device", ControllerDevice.VarNavigationDevices);

		if (ControllerDevice.VarNavigationDevices == ManagerDevice.NavigationDevices.Keyboard || 
		    ControllerDevice.VarNavigationDevices == ManagerDevice.NavigationDevices.Gamepad){
			allowSceneObjects = !EditorUtility.IsPersistent (ControllerDevice);
			EditorGUILayout.BeginVertical("box");

			// For Oculus Rift the navigation is 1st person

			// Speed of navigation
			ControllerDevice.Keyboard_PlayerSpeed = EditorGUILayout.Slider("Translation Speed", ControllerDevice.Keyboard_PlayerSpeed, minTranslationSpeed, maxTranslationSpeed);

			EditorGUILayout.EndVertical();

			// Keys Assignation for keyboard
			if (ControllerDevice.VarNavigationDevices == ManagerDevice.NavigationDevices.Keyboard) {
				GUILayout.Label("Translation");
				EditorGUILayout.BeginVertical("box");
				ControllerDevice.kb_Up   = (KeyCode) EditorGUILayout.EnumPopup("Move Forwards", ControllerDevice.kb_Up);
				ControllerDevice.kb_Down = (KeyCode) EditorGUILayout.EnumPopup("Move Backwards", ControllerDevice.kb_Down);
				ControllerDevice.kb_Left = (KeyCode) EditorGUILayout.EnumPopup("Move Left", ControllerDevice.kb_Left);
				ControllerDevice.kb_Right = (KeyCode) EditorGUILayout.EnumPopup("Move Right", ControllerDevice.kb_Right);
				ControllerDevice.kb_Y_Up = (KeyCode) EditorGUILayout.EnumPopup("Go Up", ControllerDevice.kb_Y_Up);
				ControllerDevice.kb_Y_Down = (KeyCode) EditorGUILayout.EnumPopup("Go Down", ControllerDevice.kb_Y_Down);
				EditorGUILayout.EndVertical();

				// Run and Slow
				EditorGUILayout.BeginVertical("box");
				ControllerDevice.kb_Run  = (KeyCode) EditorGUILayout.EnumPopup("Run", ControllerDevice.kb_Run);
				ControllerDevice.navRunFactor = EditorGUILayout.Slider("Run Factor", ControllerDevice.navRunFactor, minRunFactor, maxRunFactor);
				// run factor
				ControllerDevice.kb_Slow = (KeyCode) EditorGUILayout.EnumPopup("Slow", ControllerDevice.kb_Slow);
				ControllerDevice.navSlowFactor = EditorGUILayout.Slider("Slow Factor", ControllerDevice.navSlowFactor, minSlowFactor, maxSlowFactor);
				// slow factor
				EditorGUILayout.EndVertical();

				// Rotation section
				if (ControllerDevice.VarVisualDevices != ManagerDevice.VisualizationDevices.OculusRift){
					GUILayout.Label("Rotations");
					EditorGUILayout.BeginVertical("box");
					ControllerDevice.kb_pitch = (KeyCode) EditorGUILayout.EnumPopup("Pitch - X Axis", ControllerDevice.kb_pitch);
					ControllerDevice.kb_yaw = (KeyCode) EditorGUILayout.EnumPopup("Yaw - Y Axis", ControllerDevice.kb_yaw);
					ControllerDevice.kb_roll = (KeyCode) EditorGUILayout.EnumPopup("Roll - Z Axis", ControllerDevice.kb_roll);
					EditorGUILayout.HelpBox("Use the keys combinations:\n(Move Forwards / Backwards) -> Pitch.\n(Move Left / Right) -> Yaw.\n(Move Left / Right) -> Roll.", MessageType.Info);
					EditorGUILayout.EndVertical();								
				}

				// Reset Avatar Rotation
				EditorGUILayout.BeginVertical("box");
				ControllerDevice.resetAvatarRotation = EditorGUILayout.ToggleLeft("Reset Avatar Rotation", ControllerDevice.resetAvatarRotation);
				if (ControllerDevice.resetAvatarRotation) {
					ControllerDevice.kb_ResetAvatarRotation = (KeyCode) EditorGUILayout.EnumPopup("Select the Key ", ControllerDevice.kb_ResetAvatarRotation);
					EditorGUILayout.HelpBox("With this option, the Avatar/Player will set to the start rotation in Design Mode." , MessageType.Info );
				} else {
					ControllerDevice.kb_ResetAvatarRotation = KeyCode.None;
				}
				EditorGUILayout.EndVertical();

			}

			// Keys Assignation for gamepad
			if (ControllerDevice.VarNavigationDevices == ManagerDevice.NavigationDevices.Gamepad) {
				GUILayout.Label("Translation");			
				EditorGUILayout.BeginVertical("box");
				ControllerDevice.gp_pad_Up = (XboxCtrlrInput.XboxDPad) EditorGUILayout.EnumPopup("Move Forwards", ControllerDevice.gp_pad_Up);
				ControllerDevice.gp_pad_Down = (XboxCtrlrInput.XboxDPad) EditorGUILayout.EnumPopup("Move Backwards", ControllerDevice.gp_pad_Down);
				ControllerDevice.gp_pad_Left = (XboxCtrlrInput.XboxDPad) EditorGUILayout.EnumPopup("Move Left", ControllerDevice.gp_pad_Left);
				ControllerDevice.gp_pad_Right = (XboxCtrlrInput.XboxDPad) EditorGUILayout.EnumPopup("Move Right", ControllerDevice.gp_pad_Right);
				ControllerDevice.gp_Y_Up 	= (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Go Up", ControllerDevice.gp_Y_Up);
				ControllerDevice.gp_Y_Down 	= (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Go Down", ControllerDevice.gp_Y_Down);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical("box");
				ControllerDevice.gp_Run  = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Run", ControllerDevice.gp_Run);
				ControllerDevice.navRunFactor = EditorGUILayout.Slider("Run Factor", ControllerDevice.navRunFactor, minRunFactor, maxRunFactor);
				ControllerDevice.gp_Slow= (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Slow", ControllerDevice.gp_Slow);
				ControllerDevice.navSlowFactor = EditorGUILayout.Slider("Slow Factor", ControllerDevice.navSlowFactor, minSlowFactor, maxSlowFactor);
				//ControllerDevice.gp_Jump = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Jump", ControllerDevice.gp_Jump);
				EditorGUILayout.EndVertical();

				// Rotations
				if (ControllerDevice.VarVisualDevices != ManagerDevice.VisualizationDevices.OculusRift){
					GUILayout.Label("Rotations - Avatar");
					// X rotation
					EditorGUILayout.BeginVertical("box");
					ControllerDevice.gp_pitch = (XboxCtrlrInput.XboxAxis) EditorGUILayout.EnumPopup("Pitch - X Axis", ControllerDevice.gp_pitch);
					// Combine with other button
					ControllerDevice.gp_mixAvaRotAxisX = EditorGUILayout.ToggleLeft("Combine with button", ControllerDevice.gp_mixAvaRotAxisX);
					if (ControllerDevice.gp_mixAvaRotAxisX){
						ControllerDevice.gp_mixButtonAvaRotAxisX = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Select the button", ControllerDevice.gp_mixButtonAvaRotAxisX);
					}
					EditorGUILayout.EndVertical();

					// Y Rotation
					EditorGUILayout.BeginVertical("box");
					ControllerDevice.gp_yaw = (XboxCtrlrInput.XboxAxis) EditorGUILayout.EnumPopup("Yaw - Y Axis", ControllerDevice.gp_yaw);
					// Combine with other button
					ControllerDevice.gp_mixAvaRotAxisY = EditorGUILayout.ToggleLeft("Combine with button", ControllerDevice.gp_mixAvaRotAxisY);
					if (ControllerDevice.gp_mixAvaRotAxisY){
						ControllerDevice.gp_mixButtonAvaRotAxisY = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Select the button", ControllerDevice.gp_mixButtonAvaRotAxisY);
					}
					EditorGUILayout.EndVertical();

					// Z Rotation
					EditorGUILayout.BeginVertical("box");
					ControllerDevice.gp_roll = (XboxCtrlrInput.XboxAxis) EditorGUILayout.EnumPopup("Roll - Z Axis", ControllerDevice.gp_roll);			
					// Combine with other button
					ControllerDevice.gp_mixAvaRotAxisZ = EditorGUILayout.ToggleLeft("Combine with button", ControllerDevice.gp_mixAvaRotAxisZ);
					if (ControllerDevice.gp_mixAvaRotAxisZ){
						ControllerDevice.gp_mixButtonAvaRotAxisZ = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Select the button", ControllerDevice.gp_mixButtonAvaRotAxisZ);
					}
					EditorGUILayout.EndVertical();
				}

				// Reset Avatar Rotation
				EditorGUILayout.BeginVertical("box");
				ControllerDevice.resetAvatarRotation = EditorGUILayout.ToggleLeft("Reset Avatar Rotation", ControllerDevice.resetAvatarRotation);
				if (ControllerDevice.resetAvatarRotation) {
					ControllerDevice.gp_ResetAvatarRotaton = (XboxCtrlrInput.XboxButton) EditorGUILayout.EnumPopup("Select the Button ", ControllerDevice.gp_ResetAvatarRotaton);
					EditorGUILayout.HelpBox("With this option, the Avatar/Player will set to the start rotation in Design Mode." , MessageType.Info );				
				}
				EditorGUILayout.EndVertical();
			}

		}

		// Options for 3D Mouse
		if (ControllerDevice.VarNavigationDevices == ManagerDevice.NavigationDevices.Mouse3D) {
			bool allowSceneObjects = !EditorUtility.IsPersistent (ControllerDevice);

			ControllerDevice.m3d_target = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Object to Follow", "Assing a GameObject to Follow"), ControllerDevice.m3d_target, typeof(GameObject), allowSceneObjects);
			ControllerDevice.m3d_distance = EditorGUILayout.FloatField (new GUIContent("Distance","Distance on Z - axis between camera and game object related."), ControllerDevice.m3d_distance);
			ControllerDevice.m3d_height	  = EditorGUILayout.FloatField (new GUIContent("Height","Distance on Y - axis between camera and game object related."), ControllerDevice.m3d_height);
			ControllerDevice.m3d_damping  = EditorGUILayout.FloatField (new GUIContent("Damping",""), ControllerDevice.m3d_damping);
			ControllerDevice.m3d_smootRotation = EditorGUILayout.Toggle("Smooth Rotation", ControllerDevice.m3d_smootRotation);
			ControllerDevice.m3d_followBehind  = EditorGUILayout.Toggle("Follow Behind", ControllerDevice.m3d_followBehind);
			ControllerDevice.m3d_rotationDamping = EditorGUILayout.FloatField (new GUIContent("Rotatin Damping",""), ControllerDevice.m3d_rotationDamping);

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox("Go to the menu Window / SpaceNavigator \nto open the window to setup the Mouse 3D.", MessageType.Info);

			// Open the SpaceNavigator window
			Mouse3Dwindow = SpaceNavigatorWindow.GetWindow(typeof(SpaceNavigatorWindow)) as SpaceNavigatorWindow;  
			if (Mouse3Dwindow) {
				Mouse3Dwindow.Show();
			}

		} else {
			// Close the SpaceNavigator Window
			if (Mouse3Dwindow) {
				Mouse3Dwindow.Close();
			}
		}

		// Tell to Unity that the values has been changed
		if (GUI.changed) {
			EditorUtility.SetDirty(ControllerDevice);		
		}

	}

}
