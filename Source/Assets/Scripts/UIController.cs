/***************************************************************************************/
//
//  SoftKinetic iisu SDK code samples 
//
//  Project Name      : UISample
//  Revision          : 1.0
//  Description       : Tutorial on how to use the UI layer of iisu. 
//						The UI layer is ideal for developing gesture based 2D interfaces.
//
// DISCLAIMER
// All rights reserved to SOFTKINETIC INTERNATIONAL SA/NV (a company incorporated
// and existing under the laws of Belgium, with its principal place of business at
// Boulevard de la Plainelaan 15, 1050 Brussels (Belgium), registered with the Crossroads
// bank for enterprises under company number 0811 784 189 - “Softkinetic”)
//
// For any question about terms and conditions, please contact: info@softkinetic.com
// Copyright (c) 2007-2011 SoftKinetic SA/NV
//
/****************************************************************************************/

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class responsible of the control of both the master (first hand) and slave (second hand) UI pointer.
/// </summary>
public class UIController : MonoBehaviour {
	
	public GUIStyle textStyle;
	public Transform Pointer;
	public IisuInputProvider InputProvider;
	
	private float _xNorm;
	private float _yNorm;
	
	private float _width;
	private float _height;
	
	public Texture2D CircleGesture;
	
	private const float DEPTH = 5;
	
	private bool _valid;
	
	void Start()
	{
		_height = Camera.main.orthographicSize * 2;
		_width = Camera.main.aspect * _height;
		_valid = false;
	}
	
	void Update()
	{
		//Fetch the normalized UI coordinates for both pointers. 
		//We only use the values between -1 and 1 (by clamping), 
		//as we are only interested in the coordinates within the 
		//pointing box.
		_xNorm = (Mathf.Clamp(InputProvider.PointingCoo.x, -1, 1) + 1)/2;
		_yNorm = (Mathf.Clamp(InputProvider.PointingCoo.y, -1, 1) + 1)/2;
		
		Pointer.position = new Vector3(-_width/2 + _xNorm * _width, -_height/2 + _yNorm * _height, DEPTH);
		
		_valid = InputProvider.UIControllerActive;
	}
	
	void OnGUI()
	{
		//if UI is not active, we provide the user with a feedback message that a circle gesture has to be
		//made to activate UI
		if(!_valid)
		{
			GUI.DrawTexture(new Rect(Screen.width/2 - 200, Screen.height/2 - 200, 400, 400), CircleGesture);	
			GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 + 200, 500, 100), "Please make a circle gesture to continue", textStyle);
		}
	}
}
