/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */

/* s3dCamera.js revised 12/30/12. Usage:
 * Attach to camera. Creates, manages and renders stereoscopic view.
 * NOTE: interaxial is measured in millimeters; zeroPrlxDist is measured in meters 
 * Has companion Editor script to create custom inspector 
 */
 
#pragma strict
// 1. Camera
public var leftCam : GameObject; // left view camera
public var rightCam : GameObject; // right view camera
public var maskCam : GameObject; // black mask for mobile formats
public var guiCam : GameObject; // mask for gui overlay for mobile formats
// Stereo Parameters
public var interaxial : float = 65; // Distance (in millimeters) between cameras
public var zeroPrlxDist : float = 3.0; // Distance (in meters) at which left and right images overlap exactly
// 3D Camera Configuration // 
public var toedIn : boolean = false; // Angle cameras inward to converge. Bad idea!
// Camera Selection // 
//enum cams3D {Left_Right, Left_Only, Right_Only, Right_Left} // declared in s3dEnums.js
public var cameraSelect = cams3D.Left_Right; // View order - swap cameras for cross-eyed free-viewing
public var H_I_T: float = 0;
public var offAxisFrustum : float = 0;
//private var screenSize : Vector2;
public var depthPlane : GameObject;
public var planeNear : GameObject;
public var planeZero : GameObject;
public var planeFar : GameObject;
public	var horizontalFOV : float;
public	var verticalFOV : float;

// Options
public var useCustomStereoLayer : boolean = false; // Set a custom layer to use multiple stereo cameras
public var stereoLayer : int = 0; // Camera will render this layer only
public var useLeftRightOnlyLayers : boolean = true; // Enable layers seen by only one camera
public var leftOnlyLayer : int = 20; // Layer seen by left camera only
public var rightOnlyLayer : int = 21; // Layer seen by right camera only
public var guiOnlyLayer : int = 22; // Layer seen by gui camera only
public var renderOrderDepth : int = 0; // For multiple stereo cameras - higher layers are rendered on top of lower layers

// 2. Render
// enable useStereoShader to use RenderTextures & stereo shader (Unity Pro only) for desktop applications - allows anaglyph format
// turn off for Unity Free, Android and iOS (allows side by side mode only)
public var useStereoShader : boolean = false;
private var useStereoShaderPrev : boolean = false; // track changes to useStereoShader
// Stereo Material + Stereo Shader (uses FOV2GO/Shaders/stereo3DViewMethods)
public var stereoMaterial : Material; // Assign FOV2GO/Materials/stereoMat material in inspector
// Render Textures
private var leftCamRT : RenderTexture;
private var rightCamRT: RenderTexture;

// 3D Display Mode // 
//enum mode3D {SideBySide, Anaglyph, OverUnder, Interlace, Checkerboard}; // declared in s3dEnums.js
public var format3D = mode3D.SideBySide;

// Anaglyph Mode
//enum anaType {Monochrome, HalfColor, FullColor, Optimized, Purple};  // declared in s3dEnums.js
public var anaglyphOptions = anaType.HalfColor;
// Side by Side Mode
public var sideBySideSqueezed : boolean = false;
// Over Under Mode
public var overUnderStretched : boolean = false;
public var usePhoneMask : boolean = true;
public var leftViewRect : Vector4 = Vector4(0,0,0.5,1);
public var rightViewRect : Vector4 = Vector4(0.5,0,1,1);
// Interlace Variables
public var interlaceRows : int = 1080;
public var checkerboardColumns : int = 1920;
public var checkerboardRows : int = 1080;

public var planes : Plane[];

private var initialized : boolean = false;

@script AddComponentMenu ("Stereoskopix/s3d Camera")
@script ExecuteInEditMode()

function Awake () {
	initStereoCamera();
}

function initStereoCamera () {
	SetupCameras();
	SetupShader();
	SetStereoFormat();
	
	var infoScript: s3dDepthInfo;
	infoScript = GetComponent(s3dDepthInfo);
	if (infoScript) { 
		SetupScreenPlanes(); // only create screen planes if necessary
	}
}

function SetupCameras() {
	var lcam = transform.Find("leftCam"); // check if we've already created a leftCam
	if (lcam) {
		leftCam = lcam.gameObject;
		leftCam.GetComponent.<Camera>().CopyFrom (GetComponent.<Camera>());
	} else {
		leftCam = new GameObject ("leftCam", Camera);
		leftCam.AddComponent(GUILayer);
		leftCam.GetComponent.<Camera>().CopyFrom (GetComponent.<Camera>());
		leftCam.transform.parent = transform;
	}

	var rcam = transform.Find("rightCam"); // check if we've already created a rightCam
	if (rcam) {
		rightCam = rcam.gameObject;
		rightCam.GetComponent.<Camera>().CopyFrom (GetComponent.<Camera>());
	} else {
		rightCam = new GameObject("rightCam", Camera);
		rightCam.AddComponent(GUILayer);
		rightCam.GetComponent.<Camera>().CopyFrom (GetComponent.<Camera>());
		rightCam.transform.parent = transform;
	}
	
	var mcam = transform.Find("maskCam"); // check if we've already created a maskCam
	if (mcam) {
		maskCam = mcam.gameObject;
	} else {
		maskCam = new GameObject("maskCam", Camera);
		maskCam.AddComponent(GUILayer);
		maskCam.GetComponent.<Camera>().CopyFrom (GetComponent.<Camera>());
		maskCam.transform.parent = transform;
	}
	
	var gcam = transform.Find("guiCam"); // check if we've already created a maskCam
	if (gcam) {
		guiCam = gcam.gameObject;
	} else {
		guiCam = new GameObject("guiCam", Camera);
		guiCam.AddComponent(GUILayer);
		guiCam.GetComponent.<Camera>().CopyFrom (GetComponent.<Camera>());
		guiCam.transform.parent = transform;
	}

	var guiC : GUILayer = GetComponent(GUILayer);
	guiC.enabled = false;
	
	GetComponent.<Camera>().depth = -2; // rendering order (back to front): centerCam/maskCam/leftCam1/rightCam1/leftCam2/rightCam2/ etc
	
	horizontalFOV = (2 * Mathf.Atan(Mathf.Tan((GetComponent.<Camera>().fieldOfView * Mathf.Deg2Rad) / 2) * GetComponent.<Camera>().aspect))*Mathf.Rad2Deg;
	verticalFOV = GetComponent.<Camera>().fieldOfView;
	
	leftCam.GetComponent.<Camera>().depth = GetComponent.<Camera>().depth + (renderOrderDepth*2) + 2;
	rightCam.GetComponent.<Camera>().depth = GetComponent.<Camera>().depth + ((renderOrderDepth*2)+1) + 3;
	
	if (useLeftRightOnlyLayers) {
		if (useCustomStereoLayer) {
			leftCam.GetComponent.<Camera>().cullingMask = (1 << stereoLayer | 1 << leftOnlyLayer); // show stereo + left only
			rightCam.GetComponent.<Camera>().cullingMask = (1 << stereoLayer | 1 << rightOnlyLayer); // show stereo + right only
		} else {
			leftCam.GetComponent.<Camera>().cullingMask = ~(1 << rightOnlyLayer | 1 << guiOnlyLayer); // show everything but right only layer & mask only layer
			rightCam.GetComponent.<Camera>().cullingMask = ~(1 << leftOnlyLayer | 1 << guiOnlyLayer); // show everything but left only layer & mask only layer
		}
	} else {
		if (useCustomStereoLayer) {
			leftCam.GetComponent.<Camera>().cullingMask = (1 << stereoLayer); // show stereo layer only
			rightCam.GetComponent.<Camera>().cullingMask = (1 << stereoLayer); // show stereo layer only
		}
	}
		
	maskCam.GetComponent.<Camera>().depth = leftCam.GetComponent.<Camera>().depth-1;
	guiCam.GetComponent.<Camera>().depth = rightCam.GetComponent.<Camera>().depth+1;
	
	maskCam.GetComponent.<Camera>().cullingMask = 0;
	guiCam.GetComponent.<Camera>().cullingMask = 1 << guiOnlyLayer; // only show what's in the guiOnly layer
	maskCam.GetComponent.<Camera>().clearFlags = CameraClearFlags.SolidColor;
	guiCam.GetComponent.<Camera>().clearFlags = CameraClearFlags.Depth;
	maskCam.GetComponent.<Camera>().backgroundColor = Color.black;	
	#if !UNITY_EDITOR
		if (!useStereoShader) {
			camera.enabled = false; // speeds up rendering, especially on Android, but doesn't work if useStereoShader is true
		}
	#endif
}	

function SetupShader() {
	if (useStereoShader) {		
		if (!leftCamRT || !rightCamRT) {
			leftCamRT = new RenderTexture (Screen.width, Screen.height, 24);
			rightCamRT = new RenderTexture (Screen.width, Screen.height, 24);
		}
		stereoMaterial.SetTexture ("_LeftTex", leftCamRT);
		stereoMaterial.SetTexture ("_RightTex", rightCamRT);
	
		leftCam.GetComponent.<Camera>().targetTexture = leftCamRT;
		rightCam.GetComponent.<Camera>().targetTexture = rightCamRT;
	} else {
		if (format3D == mode3D.SideBySide) {
			if (!usePhoneMask) {
				leftCam.GetComponent.<Camera>().rect = Rect(0,0,0.5,1);
				rightCam.GetComponent.<Camera>().rect = Rect(0.5,0,0.5,1);
			} else {
				leftCam.GetComponent.<Camera>().rect = Vector4toRect(leftViewRect);
				rightCam.GetComponent.<Camera>().rect = Vector4toRect(rightViewRect);
			}
			leftViewRect = RectToVector4(leftCam.GetComponent.<Camera>().rect);
			rightViewRect = RectToVector4(rightCam.GetComponent.<Camera>().rect);
		} else if (format3D == mode3D.OverUnder) {
			if (!usePhoneMask) {
				leftCam.GetComponent.<Camera>().rect = Rect(0,0.5,1,0.5);
				rightCam.GetComponent.<Camera>().rect = Rect(0,0,1,0.5);
			} else {
				leftCam.GetComponent.<Camera>().rect = Vector4toRect(leftViewRect);
				rightCam.GetComponent.<Camera>().rect = Vector4toRect(rightViewRect);
			}
			leftViewRect = RectToVector4(leftCam.GetComponent.<Camera>().rect);
			rightViewRect = RectToVector4(rightCam.GetComponent.<Camera>().rect);
		} else {
			print("Unity Free only supports Side-by-Side and Over-Under modes!");
		}
		fixCameraAspect();
	}
}

function SetStereoFormat() {	
	switch (format3D) {
		case (mode3D.SideBySide):
			if (useStereoShader) {
				maskCam.GetComponent.<Camera>().enabled = false;
			} else {
				maskCam.GetComponent.<Camera>().enabled = usePhoneMask;
			}
			break;
		case (mode3D.Anaglyph):
			maskCam.GetComponent.<Camera>().enabled = false;
			SetAnaType();
			break;
		case (mode3D.OverUnder):
			if (useStereoShader) {
				maskCam.GetComponent.<Camera>().enabled = false;
			} else {
				maskCam.GetComponent.<Camera>().enabled = usePhoneMask;
			}
			break;
		case (mode3D.Interlace):
			maskCam.GetComponent.<Camera>().enabled = false;
			SetWeave(false);
			break;
		case (mode3D.Checkerboard):
			maskCam.GetComponent.<Camera>().enabled = false;
			SetWeave(true);
			break;
	}
}


function SetupScreenPlanes() {
	var screenTest = transform.Find("depthPlanes");
	if (depthPlane) { // first make sure that user has assigned a depthPlane prefab
		if (!screenTest) {
			planeZero = Instantiate (depthPlane, transform.position, transform.rotation);
			var depthPlanes = new GameObject ("depthPlanes");
			depthPlanes.transform.parent = transform;
			depthPlanes.transform.localPosition = Vector3.zero;
	 		planeZero.transform.parent = depthPlanes.transform;
	     	planeZero.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	     	planeZero.name = "screenDistPlane";
	     	
			planeNear = Instantiate (depthPlane, transform.position, transform.rotation);
			planeNear.transform.parent = depthPlanes.transform;
	     	planeNear.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	     	planeNear.name = "nearDistPlane";
	     	var shader1 : Shader = Shader.Find("Particles/Additive");
	     	planeNear.GetComponent.<Renderer>().sharedMaterial = new Material(shader1);
	     	planeNear.GetComponent.<Renderer>().sharedMaterial.mainTexture = depthPlane.GetComponent.<Renderer>().sharedMaterial.mainTexture;
	     	planeNear.GetComponent.<Renderer>().sharedMaterial.SetColor("_TintColor",Color.yellow);
	     	
			planeFar = Instantiate (depthPlane, transform.position, transform.rotation);
			planeFar.transform.parent = depthPlanes.transform;
	     	planeFar.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	     	planeFar.name = "farDistPlane";
	     	var shader2 : Shader = Shader.Find("Particles/Additive");
	     	planeFar.GetComponent.<Renderer>().sharedMaterial = new Material(shader2);
	     	planeFar.GetComponent.<Renderer>().sharedMaterial.mainTexture = depthPlane.GetComponent.<Renderer>().sharedMaterial.mainTexture;
	     	planeFar.GetComponent.<Renderer>().sharedMaterial.SetColor("_TintColor",Color.green);
		} else {
			planeZero = GameObject.Find("screenDistPlane");
			planeNear = GameObject.Find("nearDistPlane");
			planeFar = GameObject.Find("farDistPlane");
		}
		planeZero.GetComponent.<Renderer>().enabled = false;
		planeNear.GetComponent.<Renderer>().enabled = false;
		planeFar.GetComponent.<Renderer>().enabled = false;
	}
}  	

// called from initStereoCamera (above), and from s3dGyroCam.js (when phone orientation is changed due to AutoRotation)
function fixCameraAspect() {
	GetComponent.<Camera>().ResetAspect();
	//yield WaitForSeconds(0.25);
	GetComponent.<Camera>().aspect *= leftCam.GetComponent.<Camera>().rect.width*2/leftCam.GetComponent.<Camera>().rect.height;
	leftCam.GetComponent.<Camera>().aspect = GetComponent.<Camera>().aspect;
	rightCam.GetComponent.<Camera>().aspect = GetComponent.<Camera>().aspect;
}

function Vector4toRect(v : Vector4) {
	var r : Rect =  Rect(v.x,v.y,v.z,v.w);
	return r;
}
	
function RectToVector4(r : Rect) {
	var v : Vector4 = Vector4(r.x,r.y,r.width,r.height);
	return v;
}
	
function Update() {
	#if UNITY_EDITOR
	if (!useStereoShader) {
		if (EditorApplication.isPlaying) {  // speeds up rendering while in play mode, but doesn't work if useStereoShader is true
			GetComponent.<Camera>().enabled = false;
		} else {
			GetComponent.<Camera>().enabled = true; // need camera enabled when in edit mode
		}
	}
	if (useStereoShader) {
		if (useStereoShaderPrev == false) {
			initStereoCamera();
		}
	} else {
		if (useStereoShaderPrev == true) {
			releaseRenderTextures();
			SetupShader();
			SetStereoFormat();
		}
	}
	useStereoShaderPrev = useStereoShader;
	#endif
	planes = GeometryUtility.CalculateFrustumPlanes(GetComponent.<Camera>());
	
	if (Application.isPlaying) {
		if (!initialized) {
			initialized = true;
		}
	} else {
		initialized = false;
		SetupShader();
		SetStereoFormat();
	}
	UpdateView();
}

function UpdateView() {
	switch (cameraSelect) {
		case cams3D.Left_Right:
			leftCam.transform.position = transform.position + transform.TransformDirection(-interaxial/2000.0, 0, 0);
			rightCam.transform.position = transform.position + transform.TransformDirection(interaxial/2000.0, 0, 0);
			break;
		case cams3D.Left_Only:
			leftCam.transform.position = transform.position + transform.TransformDirection(-interaxial/2000.0, 0, 0);
			rightCam.transform.position = transform.position + transform.TransformDirection(-interaxial/2000.0, 0, 0);
			break;
		case cams3D.Right_Only:
			leftCam.transform.position = transform.position + transform.TransformDirection(interaxial/2000.0, 0, 0);
			rightCam.transform.position = transform.position + transform.TransformDirection(interaxial/2000.0, 0, 0);
			break;
		case cams3D.Right_Left:
			leftCam.transform.position = transform.position + transform.TransformDirection(interaxial/2000.0, 0, 0);
			rightCam.transform.position = transform.position + transform.TransformDirection(-interaxial/2000.0, 0, 0);
			break;
	}
	if (toedIn) {
		leftCam.GetComponent.<Camera>().projectionMatrix = GetComponent.<Camera>().projectionMatrix;
		rightCam.GetComponent.<Camera>().projectionMatrix = GetComponent.<Camera>().projectionMatrix;
		leftCam.transform.LookAt (transform.position + (transform.TransformDirection (Vector3.forward) * zeroPrlxDist));
		rightCam.transform.LookAt (transform.position + (transform.TransformDirection (Vector3.forward) * zeroPrlxDist));
	} else {
		leftCam.transform.rotation = transform.rotation; 
		rightCam.transform.rotation = transform.rotation;
		switch (cameraSelect) {
			case cams3D.Left_Right:
				leftCam.GetComponent.<Camera>().projectionMatrix = setProjectionMatrix(true);
				rightCam.GetComponent.<Camera>().projectionMatrix = setProjectionMatrix(false);
				break;
			case cams3D.Left_Only:
				leftCam.GetComponent.<Camera>().projectionMatrix = setProjectionMatrix(true);
				rightCam.GetComponent.<Camera>().projectionMatrix = setProjectionMatrix(true);
				break;
			case cams3D.Right_Only:
				leftCam.GetComponent.<Camera>().projectionMatrix = setProjectionMatrix(false);
				rightCam.GetComponent.<Camera>().projectionMatrix = setProjectionMatrix(false);
				break;
			case cams3D.Right_Left:
				leftCam.GetComponent.<Camera>().projectionMatrix = setProjectionMatrix(false);
				rightCam.GetComponent.<Camera>().projectionMatrix = setProjectionMatrix(true);
				break;
		}
	}
}

// Calculate Stereo Projection Matrix
function setProjectionMatrix(isLeftCam : boolean) : Matrix4x4 {
	var left : float;
	var right : float;
	var a : float;
	var b : float;
	var FOVrad : float;
	var tempAspect: float = GetComponent.<Camera>().aspect;
	FOVrad = GetComponent.<Camera>().fieldOfView / 180.0 * Mathf.PI;
	if (format3D == mode3D.SideBySide) {
		if (!sideBySideSqueezed) {
			tempAspect /= 2;	// if side by side unsqueezed, double width
		}
	} else if (format3D == mode3D.OverUnder) {
		if (overUnderStretched) {
			tempAspect /= 4;
		} else {
			tempAspect /= 2;
		}
	}
	a = GetComponent.<Camera>().nearClipPlane * Mathf.Tan(FOVrad * 0.5);
	b = GetComponent.<Camera>().nearClipPlane / zeroPrlxDist;
	if (isLeftCam) {
		left  = (-tempAspect * a) + (interaxial/2000.0 * b) + (H_I_T/100) + (offAxisFrustum/100);
		right =	(tempAspect * a) + (interaxial/2000.0 * b) + (H_I_T/100) + (offAxisFrustum/100);
	} else {
		left  = (-tempAspect * a) - (interaxial/2000.0 * b) - (H_I_T/100) + (offAxisFrustum/100);
		right =	(tempAspect * a) - (interaxial/2000.0 * b) - (H_I_T/100) + (offAxisFrustum/100);
	}
	return PerspectiveOffCenter(left, right, -a, a, GetComponent.<Camera>().nearClipPlane, GetComponent.<Camera>().farClipPlane);
} 

function PerspectiveOffCenter(
	left : float, right : float,
	bottom : float, top : float,
	near : float, far : float ) : Matrix4x4 {
	var x =  (2.0 * near) / (right - left);
	var y =  (2.0 * near) / (top - bottom);
	var a =  (right + left) / (right - left);
	var b =  (top + bottom) / (top - bottom);
	var c = -(far + near) / (far - near);
	var d = -(2.0 * far * near) / (far - near);
	var e = -1.0;

	var m : Matrix4x4;
	m[0,0] = x;  m[0,1] = 0;  m[0,2] = a;  m[0,3] = 0;
	m[1,0] = 0;  m[1,1] = y;  m[1,2] = b;  m[1,3] = 0;
	m[2,0] = 0;  m[2,1] = 0;  m[2,2] = c;  m[2,3] = d;
	m[3,0] = 0;  m[3,1] = 0;  m[3,2] = e;  m[3,3] = 0;
	return m;
}

function releaseRenderTextures() {
	leftCam.GetComponent.<Camera>().targetTexture = null;
	rightCam.GetComponent.<Camera>().targetTexture = null;
	leftCamRT.Release();
	rightCamRT.Release();
}	

// Draw Scene Gizmos
function OnDrawGizmos () {
	var gizmoLeft : Vector3 = transform.position + transform.TransformDirection(-interaxial/2000.0, 0, 0); // interaxial/2/1000mm
	var gizmoRight : Vector3 = transform.position + transform.TransformDirection(interaxial/2000.0, 0, 0);
	var gizmoTarget : Vector3 = transform.position + transform.TransformDirection (Vector3.forward) * zeroPrlxDist;
	Gizmos.color = Color (1,1,1,1);
	Gizmos.DrawLine (gizmoLeft, gizmoTarget);
	Gizmos.DrawLine (gizmoRight, gizmoTarget);
	Gizmos.DrawLine (gizmoLeft, gizmoRight);
	Gizmos.DrawSphere (gizmoLeft, 0.02);
	Gizmos.DrawSphere (gizmoRight, 0.02);
	Gizmos.DrawSphere (gizmoTarget, 0.02);
	//Gizmos.DrawWireCube (gizmoTarget, Vector3(screenSize.x,screenSize.y,0.01));
}

function OnRenderImage (source:RenderTexture, destination:RenderTexture) {
	if (useStereoShader) {
	   RenderTexture.active = destination;
	   GL.PushMatrix();
	   GL.LoadOrtho();
	   switch (format3D) {
	   	case mode3D.Anaglyph:
	   	    stereoMaterial.SetPass(0);
	      	DrawQuad(0);
	   		break;
	   	case mode3D.SideBySide:
	   	case mode3D.OverUnder:
			for(var i:int = 1; i <= 2; i++) {
				stereoMaterial.SetPass(i);
				DrawQuad(i);
			}
			break;
		case mode3D.Interlace:
		case mode3D.Checkerboard:
			stereoMaterial.SetPass(3);
			DrawQuad(3);
	   		break;
	   	default:
	   		break;
	   }
	   GL.PopMatrix();
	}
}

// Interlace & Checkerboard Modes
function SetWeave(xy) {
	if (xy) {
		stereoMaterial.SetFloat("_Weave_X", checkerboardColumns);
		stereoMaterial.SetFloat("_Weave_Y", checkerboardRows);
	} else {
		stereoMaterial.SetFloat("_Weave_X", 1);
		stereoMaterial.SetFloat("_Weave_Y", interlaceRows);
	}
}

// Anaglyph Mode
function SetAnaType() {
   switch (anaglyphOptions) {
   		case anaType.Monochrome:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(0.299,0.587,0.114,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0.299,0.587,0.114,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0.299,0.587,0.114,0));
	   	break;
   		case anaType.HalfColor:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(0.299,0.587,0.114,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0,1,0,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0,0,1,0));
	   	break;
   		case anaType.FullColor:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(1,0,0,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0,1,0,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0,0,1,0));
	   	break;
   		case anaType.Optimized:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(0,0.7,0.3,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0,1,0,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0,0,1,0));
	   	break;
   		case anaType.Purple:
   			stereoMaterial.SetVector("_Balance_Left_R", Vector4(0.299,0.587,0.114,0));
   			stereoMaterial.SetVector("_Balance_Left_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Left_B", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_R", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_G", Vector4(0,0,0,0));
  			stereoMaterial.SetVector("_Balance_Right_B", Vector4(0.299,0.587,0.114,0));
	   	break;
   }
}

// Draw Render Textures Quads
function DrawQuad(cam) {
	if (format3D == mode3D.Anaglyph) {
   		GL.Begin (GL.QUADS);      
      	GL.TexCoord3( 0.0, 0.0, 0.0 ); GL.Vertex3( 0.0, 0.0, 0.0 );
      	GL.TexCoord3( 1.0, 0.0, 0.0 ); GL.Vertex3( 1.0, 0.0, 0.0 );
      	GL.TexCoord3( 1.0, 1.0, 0.0 ); GL.Vertex3( 1.0, 1.0, 0.0 );
      	GL.TexCoord3( 0.0, 1.0, 0.0 ); GL.Vertex3( 0.0, 1.0, 0.0 );
   		GL.End();
	} else {
		if (format3D==mode3D.SideBySide) {
			if (cam==1) {
		   		GL.Begin (GL.QUADS);      
		      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 0.5, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 0.5, 1.0, 0.1 );
		      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 1.0, 0.1 );
		   		GL.End();
			} else {
		   		GL.Begin (GL.QUADS);      
		      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.5, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1.0, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1.0, 1.0, 0.1 );
		      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.5, 1.0, 0.1 );
		   		GL.End();
			}
		} else if (format3D == mode3D.OverUnder) {
			if (cam==1) {
		   		GL.Begin (GL.QUADS);      
		      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.5, 0.1 );
		      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1.0, 0.5, 0.1 );
		      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1.0, 1.0, 0.1 );
		      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 1.0, 0.1 );
		   		GL.End();
			} else {
		   		GL.Begin (GL.QUADS);      
		      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1.0, 0.0, 0.1 );
		      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1.0, 0.5, 0.1 );
		      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 0.5, 0.1 );
		   		GL.End();
			} 
		} else if (format3D == mode3D.Interlace || format3D == mode3D.Checkerboard) {
	   		GL.Begin (GL.QUADS);      
	      	GL.TexCoord2( 0.0, 0.0 ); GL.Vertex3( 0.0, 0.0, 0.1 );
	      	GL.TexCoord2( 1.0, 0.0 ); GL.Vertex3( 1, 0.0, 0.1 );
	      	GL.TexCoord2( 1.0, 1.0 ); GL.Vertex3( 1, 1.0, 0.1 );
	      	GL.TexCoord2( 0.0, 1.0 ); GL.Vertex3( 0.0, 1.0, 0.1 );
	   		GL.End();
		}
	}
} 
