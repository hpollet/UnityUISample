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

public class SKLogo : MonoBehaviour {
	
	public Texture2D LogoTexture;
	
	public float NormalizedXCoordinate;
	public float NormalizedYCoordinate;
	public float NormalizedWidth;
	
	private float _normalizedHeight;
	
	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width * NormalizedXCoordinate, 
		                   Screen.height * NormalizedYCoordinate, 
		                   Screen.width * NormalizedWidth, 
		                   Screen.width * NormalizedWidth * (float)LogoTexture.height/(float)LogoTexture.width), 
		          LogoTexture);
	}
	
}

