/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Clase que gestiona los diferentes dispositivos
* a usar en el Laboratorio.
\*******************************************************************/


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using XboxCtrlrInput;

[System.Serializable]
public class ManagerDevice : MonoBehaviour {

	// Singleton
	static ManagerDevice _instance;

	public static bool isActive {
		get {
			return _instance != null;
		}
	}

	public static ManagerDevice instance {
		get {
			if (_instance == null) {
				_instance = Object.FindObjectOfType( typeof(ManagerDevice) ) as ManagerDevice;

				if (_instance == null) {
					GameObject go = new GameObject("_managerdevice");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<ManagerDevice>();
				}

			}
			return _instance;
		}
	}

	// Opciones para visualizacion
	public enum VisualizationDevices{
		Display,
		OculusRift,
		StereoWall
	}

	// Opciones para manipulacion
	public enum ManipulationDevices{
		None,
		Mouse,
		LeapMotion
		//OptiTrack
	}

	// Opciones para navegacion
	public enum NavigationDevices{
		Keyboard,
		Mouse3D,
		Gamepad,
		None
	}

	// Opciones para navegacion por teclado
	public enum keyboardNavigationPerson {
		FirstPerson,	// Navega en primera persona
		ThirdPerson		// Navega en tercera persona
	}


	// Disposicion de las camaras para StereoWall
	public enum cams3D {
		Left_Right, 
		Right_Left,
		Left_Only, 
		Right_Only
	}

	public VisualizationDevices VarVisualDevices;
	public ManipulationDevices	VarManipulationDevices;
	public NavigationDevices	VarNavigationDevices;

	/*** Options for Keyword Navigation  ***/
	public keyboardNavigationPerson VarKeyboardNavPerson;
	public float Keyboard_PlayerSpeed = 7f;
	public float keyboard_FirstPerson_CameraDistance = 0.1f;
	public float keyboard_FirstPerson_CameraHeight = 1f;
	public GameObject keyboard_ObjectFollow;

	/*** Options for Display Visualization  ***/
	public bool Display_Third_Person = false; 
	public bool Display_First_Person = false; 
		
	/*** Options for Oculus Rift	***/
	public bool	Ovr_PlayerController = false;
	public bool Ovr_CameraRig = false;

	/*** Options for leap motion	**/
	// If leap is mounted on oculus
	public bool Leap_IsHeadMounted = false;
	// left hand model
	public HandModel Leap_LeftGraphicsModel;
	// right hand model
	public HandModel Leap_RightGraphicsModel;
	// tool model
	public ToolModel Leap_ToolModel;
	// debug info
	public bool Leap_DebugInfo  = false;


	/*** Options for optitrack	***/
	public bool 	Opti_vrpnBroadcast = false;
	public int		Opti_vrpnPort = 3883;
	public string 	Opti_IpServer = "138.100.14.98";
	public string 	Opti_MultiCastingGroup = "239.255.42.99";
	public int 		Opti_MultiCastingPort = 1511;

	/*** Options for Stereo Wall  ***/
	public float stw_interaxial = 65; // Distance (in millimeters) between cameras
	public float stw_zeroPrlxDist = 3f; // Distance (in meters) at which left and right images overlap exactly
	public cams3D stw_cameraSelect = cams3D.Left_Right; // View order - swap cameras for cross-eyed free-viewing
	public float stw_H_I_T = 0f;
	public GameObject stw_depthPlane;


	/*** Options for Mouse3D ***/
	public float m3d_distance = 5f;
	public float m3d_height = 3f;
	public float m3d_damping = 5f;
	public float m3d_rotationDamping = 10f;
	public bool  m3d_smootRotation = true;
	public bool	 m3d_followBehind = true;
	public GameObject m3d_target;


	/*** Constants for prefabs Oculus and Leap Motion ***/
	private const string OVR_PREFAB_CAMERA_RIG = "Assets/ManagerDevice/Prefabs/OVRCameraRig.prefab";
	private const string OVR_PREFAB_PLAYER_CONTROLLER = "Assets/ManagerDevice/Prefabs/OVRPlayerController.prefab";
	private const string LEAP_PREFAB_CAMERA_RIG = "Assets/ManagerDevice/Prefabs/LeapOVRCameraRig.prefab";
	private const string LEAP_PREFAB_PLAYER_CONTROLLER = "Assets/ManagerDevice/Prefabs/LeapOVRPlayerController.prefab";
	private const string PREFAB_NAME_OCULUS_PLAYER = "OVRPlayerController";
	private const string PREFAB_NAME_OCULUS_CAMERA_RIG = "OVRCameraRig";
	private const string PREFAB_NAME_LEAPMOTION = "LeapMotionDevice";


	// Para guardar las teclas que puede usar
	private string[] m_keys;

	// Opciones para el teclado
	public KeyCode kb_Slow;
	public KeyCode kb_Run;
	//public KeyCode kb_Fly;
	//public KeyCode kb_Jump;
	public KeyCode kb_Up;
	public KeyCode kb_Down;
	public KeyCode kb_Left;
	public KeyCode kb_Right;
	public KeyCode kb_pitch;
	public KeyCode kb_yaw;
	public KeyCode kb_roll;
	public KeyCode kb_Y_Up;
	public KeyCode kb_Y_Down;
	// Opciones para el gamepdad
	public XboxButton gp_Slow;
	public XboxButton gp_Run;
	public XboxButton gp_Y_Up;
	public XboxButton gp_Y_Down;
	// Mover con los axis del gamepad
	public XboxAxis gp_pitch;
	public XboxAxis gp_yaw;
	public XboxAxis gp_roll;
	// Zoom con el gamepad
	public XboxAxis gp_zoomIn;
	public XboxAxis gp_zoomOut;
	// Mover con el pad del gamepad
	public XboxDPad gp_pad_Up;
	public XboxDPad gp_pad_Down;
	public XboxDPad gp_pad_Left;
	public XboxDPad gp_pad_Right;
	// Mover la camara con el teclado
	public KeyCode kb_Cam_Right;
	public KeyCode kb_Cam_Left;
	public KeyCode kb_Cam_Up;
	public KeyCode kb_Cam_Down;
	public KeyCode kb_Cam_Rot_Z_Left;
	public KeyCode kb_Cam_Rot_Z_Right;
	// Zoom la camara con el teclado
	public KeyCode kb_Cam_ZoomIn;
	public KeyCode kb_Cam_ZoomOut;
	// Rotar la camara con el gamepad
	public XboxAxis gp_Cam_UpDown;
	public XboxAxis gp_Cam_LeftRight;
	public XboxAxis gp_Cam_zRotation;
	// Max velocidad de rotacion de la camara
	public float cam_maxRotationSpeed;
	// Max angulo de rotacion de la camara
	public float cam_maxAngleRot_X;
	public float cam_maxAngleRot_Y;
	public float cam_maxAngleRot_Z;
	// Zoom camara
	public int cam_zoomFactor;
	// Run factor
	public float navRunFactor;
	// Slow factor
	public float navSlowFactor;
	// Reset Camera rotacion
	public KeyCode kb_ResetCameraRotation;
	public XboxButton gp_ResetCameraRotation;
	public bool resetCameraRotation;
	// Reset Avatar rotation
	public KeyCode kb_ResetAvatarRotation;
	public XboxButton gp_ResetAvatarRotaton;
	public bool resetAvatarRotation;
	// Combine gamepad buttons for camera rotation
	public bool gp_mixCamRotAxisX;
	public bool gp_mixCamRotAxisY;
	public bool gp_mixCamRotAxisZ;
	public XboxButton gp_mixButtonCamRotAxisX;
	public XboxButton gp_mixButtonCamRotAxisY;
	public XboxButton gp_mixButtonCamRotAxisZ;
	// Combine gamepad button for Avatar rotation
	public bool gp_mixAvaRotAxisX;
	public bool gp_mixAvaRotAxisY;
	public bool gp_mixAvaRotAxisZ;
	public XboxButton gp_mixButtonAvaRotAxisX;
	public XboxButton gp_mixButtonAvaRotAxisY;
	public XboxButton gp_mixButtonAvaRotAxisZ;
	// Gameobject list to move with mouse
	public GameObject[] mouse_moveGameObjects = new GameObject[1];
	public List<GameObject> mouse_selectionGameObjects = new List<GameObject>();

	/// <summary>
	/// Add the gameobjects will be moved in runtime. 
	/// </summary>
	public void AddGameObjectToMoveMouse (GameObject go) {
		mouse_selectionGameObjects.Add(go);
		// Add the Drag script
		go.AddComponent<DragObject>();

	}

	/// <summary>
	/// Check the gameobjects added in the array in design time.
	/// For each gameobject adds the script for drag in the scene in runtime.
	/// </summary>
	public void CheckGameObjectsMoveMouse() {
		// Check if the array is no empty
		if (mouse_selectionGameObjects.Count > 0){
			// Add the script for drag for each gameobject in the array
			foreach (GameObject goToMove in mouse_selectionGameObjects){
				if (goToMove != null)
					goToMove.AddComponent<DragObject>();

			}
		}
	}


	// Metodo para buscar camaras y segun el caso desactivarlas
	public void SearchCameras (bool _disableCamera ) {
		// Si _disableCamera es true, se desactivaran las camaras
		// Se espera que solo haya una sola camara. Si hay mas, este metodo no funcionara.
		GameObject cam;

		// Busca la camara por su nombre por defecto
		cam = GameObject.Find("Camera");

		if (cam != null) {
			cam.SetActive(!_disableCamera);
		} else {
			// Busca la camara por el tag
			GameObject.FindWithTag("MainCamera").SetActive(!_disableCamera);
		}

	}


	// Metodo para instanciar prefabs
	public GameObject CreatePrefabs (string str_prefab) {
		// Metodo 1 - usando carpeta resources
		//GameObject	thirdPerson = (GameObject)Instantiate(Resources.Load("DisplayKewordThirdPerson"), transform.position, transform.rotation);

		// Metodo 2 - instanciando un prefab establecido
		Object nameObject = AssetDatabase.LoadAssetAtPath(str_prefab, typeof(GameObject));
		GameObject objPrefab = Instantiate(nameObject, transform.position, transform.rotation) as GameObject;

		return objPrefab;
	}
	

	// Metodo para configurar Oculus + LeapMotion
	public void SetOculus_Leap_Prefab (string prefabName) {
		GameObject manipulationObj; // Para instanciar el prefab de leap
		GameObject handController;
		// Constante de path para ubicar el controllador y configurar el tipo de manos
		const string PAHT_HANDCONTROLLER_OCULUS_LEAP = "LeapOVRCameraRig/CenterEyeAnchor/HeadMountedHandController";

		manipulationObj = CreatePrefabs(prefabName);
		// Revisar opcion de debug para LeapMotion
		manipulationObj.GetComponent<LeapTrackingInfo>().enabled = Leap_DebugInfo;

		// Buscar el HandController dentro del prefab 
		manipulationObj.name = PREFAB_NAME_LEAPMOTION;
		handController = manipulationObj.transform.Find (PAHT_HANDCONTROLLER_OCULUS_LEAP).gameObject;

		// Asignar el tipo de manos elegido
		if (handController != null) {
			//Debug.Log("Encontro el componente: " + handController.name);
			handController.GetComponent<HandController>().leftGraphicsModel = Leap_LeftGraphicsModel;
			handController.GetComponent<HandController>().rightGraphicsModel = Leap_RightGraphicsModel;
			handController.GetComponent<HandController>().toolModel = Leap_ToolModel;
		} else 
			Debug.LogWarning("Manager Device · Oculus + LeapMotion · Warning.\nThere is no GameObject: HeadMountedHandController, to set up Hands Graphics Model.");

	}

	// Metodo para crear los prefabs de acuerdo a la seleccion
	public void SetUpVisualOptions () {
		GameObject oculusObj; // Para instanciar el prefab		

		// Opciones para Oculus Rift
		if (VarVisualDevices == VisualizationDevices.OculusRift) {
			// Guardar la posicion de la camara seleccionada
//			Vector3 camPosition = GameObject.FindWithTag("MainCamera").transform.position;

			// Deshabilitar las camaras
			SearchCameras(true);

			// Revisar si Oculus se combina con LeapMotion
			if (VarManipulationDevices == ManipulationDevices.LeapMotion && Ovr_PlayerController) {
				// Es Oculus + LeapMotion en modo PlayerController
				//Debug.Log ("Es Oculus + LeapMotion en modo PlayerController");
				SetOculus_Leap_Prefab (LEAP_PREFAB_PLAYER_CONTROLLER);
			} else {
				if (VarManipulationDevices != ManipulationDevices.LeapMotion && Ovr_PlayerController) {
					// Es Oculus en modo Player Controller
					//Debug.Log("Es Oculus en modo Player Controller");
					oculusObj = CreatePrefabs(OVR_PREFAB_PLAYER_CONTROLLER);
					oculusObj.name = PREFAB_NAME_OCULUS_PLAYER; //PREFAB_NAME_OCULUS_PLAYER
					// Establecer el prefab en la posicion del Manager Device
					oculusObj.transform.position = this.transform.position;
					// Establecer los parametros
					oculusObj.GetComponent<OVRPlayerController>().Acceleration = Keyboard_PlayerSpeed;
					oculusObj.GetComponent<OVRPlayerController>().GravityModifier = 1f;
					//cameraMain.GetComponent<OVRPlayerController>().GravityModifier = 0f;

				} else {
					if (VarManipulationDevices == ManipulationDevices.LeapMotion && !Ovr_PlayerController) {
						// Es Oculus + LeapMotion en modo CameraRig
						//Debug.Log("Es Oculus + LeapMotion en modo CameraRig");
						SetOculus_Leap_Prefab (LEAP_PREFAB_PLAYER_CONTROLLER);
					} else {
						// Es Oculus en modo Camera Rig
						//Debug.Log("Es Oculus en modo Camera Rig");
						oculusObj = CreatePrefabs(OVR_PREFAB_CAMERA_RIG);
						oculusObj.name = PREFAB_NAME_OCULUS_CAMERA_RIG;
					}
				}
			}

		}

	}

	// Configuracion para dispositivos de manipulacion
	public void SetUpManipulationOptions () {
		GameObject manipulationObj; // Para instanciar el prefab de leap
		GameObject leapNotice; // Para verificar si leap motion esta conectado
		const string PATH_LEAP_NOTICE = "Assets/ManagerDevice/Prefabs/PluginLeapNotice.prefab";
		const string PATH_LEAP_HANDCONTROLLER = "Assets/ManagerDevice/Prefabs/LeapHandController.prefab";

		// Configuracion LeapMotion
		if (VarManipulationDevices == ManipulationDevices.LeapMotion){
			//Debug.Log("Configurando LeapMotion");
			// Instancia el prefab PluginLeapNotice
			leapNotice = CreatePrefabs(PATH_LEAP_NOTICE);
			leapNotice.name = "PluginLeapNotice";
			leapNotice.transform.position = new Vector3 (0f, 0f, 1f);

			// Establecer si no esta asociado a Oculus
			if (!Leap_IsHeadMounted && VarVisualDevices != VisualizationDevices.OculusRift) {
				// No se usa con Oculus
				manipulationObj = CreatePrefabs(PATH_LEAP_HANDCONTROLLER);
				manipulationObj.name = PREFAB_NAME_LEAPMOTION;
				// Asignar el tipo de manos
				manipulationObj.GetComponent<HandController>().leftGraphicsModel = Leap_LeftGraphicsModel;
				manipulationObj.GetComponent<HandController>().rightGraphicsModel = Leap_RightGraphicsModel;
				manipulationObj.GetComponent<HandController>().toolModel = Leap_ToolModel;
				// Revisar opcion de debug para LeapMotion
				manipulationObj.GetComponent<LeapTrackingInfo>().enabled = Leap_DebugInfo;
			
				//manipulationObj.name = "LeapMotionDevice";
				//manipulationObj.transform.localScale = this.transform.localScale;
				
				//float  newPositionLeapController = 0.5f;
				// Adjuntar el prefab al gameobject asociado si navega en 1ra o 3ra persona
				if (VarNavigationDevices != NavigationDevices.None) {
					// 1ra o 3ra persona
					if (keyboard_ObjectFollow != null) {
						manipulationObj.transform.position = keyboard_ObjectFollow.transform.position;
						manipulationObj.transform.parent = keyboard_ObjectFollow.transform;			
						// Establecer la mejor posicion para el leap controller
						// esto es temporal, es la posicion para un cubo.
						manipulationObj.transform.position = manipulationObj.transform.transform.parent.TransformPoint(0f, 0.5f, 0.25f);
					} else {
						// Debe asociar un gameobject para su desplazamiento en el campo Object to Follow
						Debug.LogWarning("Manager Device · LeapMotion · Warning.\nYou should attach a Game Object to the field: Object to Follow");
						return;
					}
				} else {
					// No realiza navegacion y lo ubica en la posicion del Manager Device
					manipulationObj.transform.position = this.transform.position;
					// Establece la escala segun la del Manager Device
					manipulationObj.transform.localScale = this.transform.localScale;				
				}
			}
		}

	}


	// Configuracion para dispositivos de navegacion
	void SetUpNavigationOptions (){
		GameObject cameraMain;
		GameObject targetOculus;

		// Opciones para navegacion por teclado y gamepad
		if (VarNavigationDevices == NavigationDevices.Keyboard || VarNavigationDevices == NavigationDevices.Gamepad) {
			// Busca la camara principal por el tag
			cameraMain = GameObject.FindWithTag("MainCamera");

			// Configuracion para primera persona diferente a oculus
			if (VarVisualDevices != VisualizationDevices.OculusRift) {
				// Configuracion para primera persona
				if (VarKeyboardNavPerson == keyboardNavigationPerson.FirstPerson) {
					// Primera persona
					// Establecer donde esta la camara principal
					if (cameraMain == null) {
						Debug.LogWarning("Manager Device · Warning.\nYou should set the tag MainCamera the corresponding Game Object");
						return;			
					} else {
						// Camera controller 1ra persona
						cameraMain.AddComponent<CameraController>().target = keyboard_ObjectFollow.transform;
						cameraMain.GetComponent<CameraController>().rotationSpeed = cam_maxRotationSpeed;


					}
					// Agregar el script que corresponda ya sea keyword o gamepad
					if (VarNavigationDevices == NavigationDevices.Keyboard){
						// Agrega el script y envia el valor de la velocidad
						keyboard_ObjectFollow.AddComponent<PlayerControllerFirstPerson>();
						keyboard_ObjectFollow.GetComponent<PlayerControllerFirstPerson>().speed = Keyboard_PlayerSpeed;
						// Establecer la camara como hijo del target
						cameraMain.transform.parent = keyboard_ObjectFollow.transform;
					} else {
						// Corresponde a gamepad
						keyboard_ObjectFollow.AddComponent<PlayerControllerGamePad>();
						keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().firstPerson = true;
						keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().thirdPerson = false;
						keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().speed = Keyboard_PlayerSpeed;
						// Establecer la camara como hijo del target
						cameraMain.transform.parent = keyboard_ObjectFollow.transform;
					}

				}
				// Configuracion para tercera persona
				if (VarKeyboardNavPerson == keyboardNavigationPerson.ThirdPerson) {
					// Tercera persona
					// Verificar si se ha asignado a algun game object
					if (keyboard_ObjectFollow == null){
						Debug.LogWarning("Manager Device · Keyword 3rd Person · Warning.\nYou should set a game object to follow in third person mode");
						return;
					} else { 
						//Establecer donde esta la camara principal
						if (cameraMain == null) {
							Debug.LogWarning("Manager Device · Keyword 3rd Person · Warning.\nYou should set the tag MainCamera the corresponding Game Object");
							return;			
						} else {
							// Agregar los scripts que correspondan ya sea keyword o gamepad
							if (VarNavigationDevices == NavigationDevices.Keyboard){
								cameraMain.AddComponent<CameraControllerThirdPerson>().target = keyboard_ObjectFollow.transform;
								cameraMain.GetComponent<CameraControllerThirdPerson>().rotationSpeed = cam_maxRotationSpeed;
								// Agrega el script y envia el valor de la velocidad
								keyboard_ObjectFollow.AddComponent<PlayerControllerThirdPerson>();
								keyboard_ObjectFollow.GetComponent<PlayerControllerThirdPerson>().speed = Keyboard_PlayerSpeed;
								// Establecer la camara como hijo del target
								cameraMain.transform.parent = keyboard_ObjectFollow.transform;			
							} else {
								// Corresponde a gamepad
								//cameraMain.AddComponent<CameraControllerThirdPerson>().target = keyboard_ObjectFollow.transform;
								//cameraMain.AddComponent<CameraControllerGamepad3rd>().target = keyboard_ObjectFollow.transform;
								cameraMain.AddComponent<CameraControllerThirdPerson>().target = keyboard_ObjectFollow.transform;
								keyboard_ObjectFollow.AddComponent<PlayerControllerGamePad>();
								keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().thirdPerson = true;
								keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().firstPerson = false;
								keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().speed = Keyboard_PlayerSpeed;
								// Establecer la camara como hijo del target
								cameraMain.transform.parent = keyboard_ObjectFollow.transform;
							}
						}															
					}	
				}				
			} else {

				// Ajustes para Oculus con Gamepad
				if (VarNavigationDevices == NavigationDevices.Gamepad){
					// Configuracion para primera persona
					if (VarKeyboardNavPerson == keyboardNavigationPerson.FirstPerson) {
						// Evaluar si Oculus va como player controller o camera rig
						if (Ovr_PlayerController) {
							// Agrega script para el gameobject de referencia
							//Debug.Log("Oculus + Gamepad + 1st Person + player controller");
							cameraMain.AddComponent<PlayerControllerGamePad>();
							cameraMain.GetComponent<PlayerControllerGamePad>().speed = Keyboard_PlayerSpeed;
							cameraMain.GetComponent<PlayerControllerGamePad>().firstPerson = true;
							cameraMain.GetComponent<PlayerControllerGamePad>().thirdPerson = false;
							cameraMain.GetComponent<OVRPlayerController>().enabled = false;
						}

						if (Ovr_CameraRig) {
							//Debug.Log("Oculus + Gamepad + 1st Person + camera rig");
							cameraMain.AddComponent<CameraControllerFirstPerson>().objectFollow = keyboard_ObjectFollow.transform;
							cameraMain.GetComponent<CameraControllerFirstPerson>().distance = keyboard_FirstPerson_CameraDistance;
							cameraMain.GetComponent<CameraControllerFirstPerson>().height = keyboard_FirstPerson_CameraHeight;

							keyboard_ObjectFollow.AddComponent<PlayerControllerGamePad>();
							keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().speed = Keyboard_PlayerSpeed;
							keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().firstPerson = true;
							keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().thirdPerson = false;
						}

					}

					// Configuracion para tercera persona
					if (VarKeyboardNavPerson == keyboardNavigationPerson.ThirdPerson) {
						//Debug.Log("Oculus + Gamepad + 3rd Person");
						// Agrega el script para el gameobject de referencia
						keyboard_ObjectFollow.AddComponent<PlayerControllerGamePad>();
						keyboard_ObjectFollow.GetComponent<PlayerControllerGamePad>().speed = Keyboard_PlayerSpeed;
						cameraMain.GetComponent<PlayerControllerGamePad>().firstPerson = false;
						cameraMain.GetComponent<PlayerControllerGamePad>().thirdPerson = true;
						cameraMain.GetComponent<OVRPlayerController>().enabled = false;
					}

				}

			}
		}

		// Mouse 3D
		if (VarNavigationDevices == NavigationDevices.Mouse3D) {
			// Si usa StereoWall o display se debe ubicar el script en el prefab StereoWallCamera
			if (VarVisualDevices != VisualizationDevices.OculusRift) {
				// Busca la camara principal por el tag
				cameraMain = GameObject.FindWithTag("MainCamera");
				// Agregar el script para la camara y sus configuraciones 
				if (cameraMain != null){
					cameraMain.AddComponent<SmoothFollow>();
					cameraMain.GetComponent<SmoothFollow>().target = m3d_target.transform;
					cameraMain.GetComponent<SmoothFollow>().distance = m3d_distance;
					cameraMain.GetComponent<SmoothFollow>().height = m3d_height;
					cameraMain.GetComponent<SmoothFollow>().damping = m3d_damping;
					cameraMain.GetComponent<SmoothFollow>().smoothRotation = m3d_smootRotation;
					cameraMain.GetComponent<SmoothFollow>().followBehind = m3d_followBehind;
					cameraMain.GetComponent<SmoothFollow>().rotationDamping = m3d_rotationDamping;
					
				} else {
					Debug.LogWarning("Manager Device · Mouse 3D · Warning\nYou should set the tag MainCamera the corresponding Game Object");
				}

			} else {
				// Si usa Oculus se busca si esta en modo player controller o camera rig
				if (Ovr_PlayerController) 
					targetOculus = GameObject.Find(PREFAB_NAME_OCULUS_PLAYER); 
				else
					targetOculus = GameObject.Find(PREFAB_NAME_OCULUS_CAMERA_RIG);
				
				if (targetOculus != null){
					targetOculus.AddComponent<SmoothFollow>();
					targetOculus.GetComponent<SmoothFollow>().target = m3d_target.transform;
					targetOculus.GetComponent<SmoothFollow>().distance = m3d_distance;
					targetOculus.GetComponent<SmoothFollow>().height = m3d_height;
					targetOculus.GetComponent<SmoothFollow>().damping = m3d_damping;
					targetOculus.GetComponent<SmoothFollow>().smoothRotation = m3d_smootRotation;
					targetOculus.GetComponent<SmoothFollow>().followBehind = m3d_followBehind;
					targetOculus.GetComponent<SmoothFollow>().rotationDamping = m3d_rotationDamping;
				} else {
					Debug.LogWarning("Manager Device · Mouse 3D · Warning\nError on assign SmoothFollow scipt for Oculus Camera Rig");
				}
			}
			// Agregar los parametros para el Target
			m3d_target.AddComponent<FlyAround>();
			m3d_target.GetComponent<FlyAround>().HorizonLock = true;

		}

	}

	/// <summary>
	/// Revisa si una o mas teclas se usan para una o mas acciones
	/// ya sea en la camara o en el avatar.
	/// retorna un debug log warning en consola
	/// </summary>
	void CheckKeyboardActions () {
		const string headerMessage = "Manager Device · Keyboard Settings · Warning\n";
//		ArrayList keysActions = new ArrayList();
		Hashtable hashKeysActions = new Hashtable();
		Hashtable hashResult = new Hashtable();
		ArrayList nameActions = new ArrayList();
		string repeatedActionsValue = null;
		string repeatedActions = null;

		// Teclas rotacion camara
		hashKeysActions.Add("Camera Turn Up", kb_Cam_Up); // turn up
		hashKeysActions.Add("Camera Turn Down",kb_Cam_Down); // turn down		
		hashKeysActions.Add("Camera Turn Left",kb_Cam_Left); // turn left
		hashKeysActions.Add("Camera Turn Right",kb_Cam_Right); // turn right
		hashKeysActions.Add("Camera Twist Left",kb_Cam_Rot_Z_Left); // twist left
		hashKeysActions.Add("CameraTwist Right",kb_Cam_Rot_Z_Right); // twist right
		hashKeysActions.Add("Zoom In",kb_Cam_ZoomIn); // zoom in
		hashKeysActions.Add("Zoom Out",kb_Cam_ZoomOut); // zoom out
		// Teclas traslacion - seccion navegacion 
		hashKeysActions.Add("Move Forwards",kb_Up); // move forwards
		hashKeysActions.Add("Move Backwards",kb_Down); // move backwards
		hashKeysActions.Add("Move Left",kb_Left); // move left
		hashKeysActions.Add("Move Right",kb_Right); // move right
		hashKeysActions.Add("Go Up",kb_Y_Up); // go up
		hashKeysActions.Add("Go Down",kb_Y_Down); // go down
		hashKeysActions.Add("Run",kb_Run); // run
		hashKeysActions.Add("Slow",kb_Slow); // slow
		// teclas rotacion player
		hashKeysActions.Add("Pitch",kb_pitch); // pitch
		hashKeysActions.Add("Yaw",kb_yaw); // yaw
		hashKeysActions.Add("Roll",kb_roll); // roll

		// Busca en el arreglo cuantas veces esta repetido una tecla
		foreach (DictionaryEntry d in hashKeysActions) {
			if (!hashResult.Contains(d.Value) && ((KeyCode) d.Value != KeyCode.None)){
				//&& d.Value != KeyCode.None
				hashResult.Add(d.Value, 1);
			} else {
				if((KeyCode) d.Value != KeyCode.None) {
					hashResult[d.Value] = (int) hashResult[d.Value] + 1;
					nameActions.Add(d.Key);
				}
			}
		}

		Hashtable tmpHashTable = (Hashtable) hashResult.Clone();

		// Se deja en el arreglo aquellas teclas cuyo valor sea >= 2
		foreach (DictionaryEntry de in tmpHashTable) {
			if ((int)de.Value == 1){
				hashResult.Remove(de.Key);
				nameActions.Remove(de.Key);
			}
		}
		// Limpiar el arreglo
		tmpHashTable.Clear();

		// Agrupar las teclas repetidas en un string
		foreach (DictionaryEntry de in hashResult) {
			if (hashKeysActions.ContainsValue(de.Key)){
				repeatedActionsValue += de.Key.ToString() + ", ";
			}
		}

		// Agrupar el nombre de las acciones repetidas
		for (int i = 0; i < nameActions.Count; i++){
			repeatedActions += nameActions[i].ToString() + ", ";
			//Debug.Log ("nameActions - tecla: " + nameActions[i].ToString());
		}

		// Mostrar si hay teclas repetidas
		if (repeatedActionsValue != null && repeatedActions != null) 
			Debug.LogWarning( headerMessage + "Repeated key(s): " + repeatedActionsValue + " in action(s): " + repeatedActions);

	}

	/// <summary>
	/// Revisa si uno o mas botones del gamepad se usan para una o mas acciones 
	/// en la camara y en el avatar.
	/// retorna un debug log warning en consola. 
	/// </summary>
	void CheckButtonGamepadActions() {
		const string headerMessage = "Manager Device · Gamepad Settings · Warning\n";
		Hashtable hashKeysActions = new Hashtable();
		Hashtable hashResult = new Hashtable();
		ArrayList nameActions = new ArrayList();
		string repeatedActionsValue = null;
		string repeatedActions = null;


/****
 * 	public bool gp_mixCamRotAxisX;
	public bool gp_mixCamRotAxisY;
	public bool gp_mixCamRotAxisZ;
	// Combine gamepad button for Avatar rotation
	public bool gp_mixAvaRotAxisX;
	public bool gp_mixAvaRotAxisY;
	public bool gp_mixAvaRotAxisZ;

* Nota: Se debe mejorar la validación de los botones
* cuando se han usado en combinación con otros botones.
 * */
		 
		// Botones rotacion camara
		hashKeysActions.Add("Turn Up and Down", gp_Cam_UpDown); 
		if (gp_mixCamRotAxisX) hashKeysActions.Add("Combine with button on X axis", gp_mixButtonCamRotAxisX); 
		hashKeysActions.Add("Turn Left and Right",gp_Cam_LeftRight); 
		if (gp_mixCamRotAxisY) hashKeysActions.Add("Combine with button on Y axis",gp_mixButtonCamRotAxisY); 
		hashKeysActions.Add("Twist Left and Right", gp_Cam_zRotation); 
		if (gp_mixCamRotAxisZ) hashKeysActions.Add("Combine with button on Z axis", gp_mixButtonCamRotAxisZ); 
		
		// Zoom con el gamepad
		hashKeysActions.Add("Zoom In", gp_zoomIn);
		hashKeysActions.Add("Zoom Out", gp_zoomOut);
		
		// Mover con el pad del gamepad
		hashKeysActions.Add("Move Forwards", gp_pad_Up);
		hashKeysActions.Add("Move Backwards", gp_pad_Down);
		hashKeysActions.Add("Move Left", gp_pad_Left);
		hashKeysActions.Add("Move Right", gp_pad_Right);
		hashKeysActions.Add("Go Up", gp_Y_Up);
		hashKeysActions.Add("Go Down", gp_Y_Down);
		
		hashKeysActions.Add("Run", gp_Run);
		hashKeysActions.Add("Slow", gp_Slow);
		
		hashKeysActions.Add("Pitch X axis", gp_pitch);
		if(gp_mixAvaRotAxisX) hashKeysActions.Add("Combine with button on Pitch", gp_mixButtonAvaRotAxisX);
		
		hashKeysActions.Add("Yaw Y axis", gp_yaw);
		if(gp_mixAvaRotAxisY) hashKeysActions.Add("Combine with button on Yaw", gp_mixButtonAvaRotAxisY);
		
		hashKeysActions.Add("Roll Z axis", gp_roll);
		if(gp_mixAvaRotAxisZ) hashKeysActions.Add("Combine with button on Roll", gp_mixButtonAvaRotAxisZ);

		// Busca en el arreglo cuantas veces esta repetida un boton
		foreach (DictionaryEntry d in hashKeysActions) {
			if (!hashResult.Contains(d.Value)){
				hashResult.Add(d.Value, 1);
			} else {
				hashResult[d.Value] = (int) hashResult[d.Value] + 1;
				nameActions.Add(d.Key);
			}
		}

		Hashtable tmpHashTable = (Hashtable) hashResult.Clone();

		// Se deja en el arreglo aquellos botones cuyo valor sea >= 2
		foreach (DictionaryEntry de in tmpHashTable) {
			if ((int)de.Value == 1){
				hashResult.Remove(de.Key);
				nameActions.Remove(de.Key);
			}
		}
		// Limpiar el arreglo
		tmpHashTable.Clear();

		// Agrupar las teclas repetidas en un string
		foreach (DictionaryEntry de in hashResult) {
			if (hashKeysActions.ContainsValue(de.Key)){
				repeatedActionsValue += de.Key.ToString() + ", ";
			}
		}
		
		// Agrupar el nombre de las acciones repetidas
		for (int i = 0; i < nameActions.Count; i++){
			repeatedActions += nameActions[i].ToString() + ", ";
			//Debug.Log ("nameActions - botones: " + nameActions[i].ToString());
		}
		
		// Mostrar si hay teclas repetidas
		if (repeatedActionsValue != null && repeatedActions != null) 
			Debug.LogWarning( headerMessage + "Repeated Button(s): " + repeatedActionsValue + " in action(s): " + repeatedActions);

	}

	/// <summary>
	/// De acuerdo al dispositivo de navegacion seleccionado, valida las teclas o botones
	/// segun sea la seleccion
	/// </summary>
	void CheckNavigationKeysButtons (){
		if (VarNavigationDevices == NavigationDevices.Keyboard) {
			CheckKeyboardActions();
		}

		if (VarNavigationDevices == NavigationDevices.Gamepad) {
			CheckButtonGamepadActions();
		}
	}
	

	void Start(){
		// Prueba de agregar gameobjects 
//		GameObject go_prueba = GameObject.FindGameObjectWithTag("CanMove");
//		if (go_prueba != null) {
//			AddGameObjectToMoveMouse(go_prueba);
//		}
		// Fin prueba

		// Crear los prefabs correspondientes
		SetUpVisualOptions ();
		SetUpManipulationOptions ();
		SetUpNavigationOptions ();
		// Revisar teclas o botones están repetidos
		CheckNavigationKeysButtons();
		// Check for the gameobjects to be moved with mouse
		CheckGameObjectsMoveMouse();
	}

}
