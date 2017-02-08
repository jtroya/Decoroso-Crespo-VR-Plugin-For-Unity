/*******************************************************************\
* Laboratorio Decoroso Crespo           	                       
* Universidad Politecnica de Madrid 
* 
* Autor: Jorge Troya Moreno										
* Descripcion: Clase en la que se almacenan las teclas que se usaran
* en el teclado y en el gamepad para caminar, volar, correr y las
* de desplazamiento.
\*******************************************************************/

using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class KeyNavigationCodes : MonoBehaviour {
	// Opciones para el teclado
	public KeyCode kb_Walk;
	public KeyCode kb_Run;
	public KeyCode kb_Fly;
	public KeyCode kb_Jump;
	public KeyCode kb_Up;
	public KeyCode kb_Down;
	public KeyCode kb_Left;
	public KeyCode kb_Right;
	// Opciones para el gamepdad
	private XboxButton gp_Walk;
	private XboxButton gp_Run;
	private XboxButton gp_Fly;
	private XboxButton gp_Jump;
	// Mover con los axis del gamepad
	private XboxAxis gp_axis_UpDown;
	private XboxAxis gp_axis_LeftRight;
	// Mover con el pad del gamepad
	private XboxDPad gp_pad_Up;
	private XboxDPad gp_pad_Down;
	private XboxDPad gp_pad_Left;
	private XboxDPad gp_pad_Right;


}
