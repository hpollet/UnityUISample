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
using Iisu;
using IisuUnity;
using System;
using System.Collections.Generic;

/// <summary>
/// Takes care of the communication between iisu and 
/// the Unity application by providing the necessary data from iisu.
/// </summary>
public class IisuInputProvider : MonoBehaviour {
	
	//the IisuUnityBehaviour object handles the iisu device, including its update thread, and disposing.
	private IisuUnityBehaviour _iisuUnity;
	
	//we access iisu data through datahandles
	private IDataHandle<Iisu.Data.IImageData> _depthImage;
	private IDataHandle<Iisu.Data.IImageData> _labelImage;
	private IDataHandle<int> _userImageID;
	private IDataHandle<Iisu.Data.Vector3> _uiPointer;
	private IDataHandle<bool> _uiStatus;
	private IDataHandle<bool> _user1Active;
	
	private Vector3[] _skeletonPositions;
	
	void Awake()
	{
		_iisuUnity = GetComponent<IisuUnityBehaviour>();
		_iisuUnity.Initialize("UnitySampleConfig.xml");
		
		//register a specific data from iisu to the datahandler
		_depthImage = _iisuUnity.Device.RegisterDataHandle<Iisu.Data.IImageData>("SOURCE.CAMERA.DEPTH.Image");
		_labelImage = _iisuUnity.Device.RegisterDataHandle<Iisu.Data.IImageData>("SCENE.LabelImage");
		_userImageID = _iisuUnity.Device.RegisterDataHandle<int>("USER1.SceneObjectID");
		_uiPointer = _iisuUnity.Device.RegisterDataHandle<Iisu.Data.Vector3>("UI.CONTROLLER1.POINTER.NormalizedCoordinates");
		_uiStatus = _iisuUnity.Device.RegisterDataHandle<bool>("UI.CONTROLLER1.IsActive");
		_user1Active = _iisuUnity.Device.RegisterDataHandle<bool>("USER1.IsActive");
		
		//in case UI is disabled, we enable UI to be sure.
		IParameterHandle<bool> _uiEnabled = _iisuUnity.Device.RegisterParameterHandle<bool>("UI.Enabled");
		_uiEnabled.Value = true;
		//enable the circle activation gesture, in case it's disabled
		IParameterHandle<bool> _cirlceEnabled = _iisuUnity.Device.RegisterParameterHandle<bool>("UI.CONTROLLERS.GESTURES.CIRCLE.Enabled");
		_cirlceEnabled.Value = true;
	}
	
	public bool User1Active
	{
		get
		{
			return _user1Active.Value;	
		}
	}
	
	/// <summary>
	/// returns true if the user has activated UI using the activation gesture
	/// </summary>
	public bool UIControllerActive
	{
		get
		{
			return _uiStatus.Value;
		}
	}
	
	/// <summary>
	/// Normalized coordinates of the UI pointer. 
	/// For both X and Y coordinates, everything within the -1 to 1 range corresponds to a position inside the pointing box
	/// </summary>
	public Vector3 PointingCoo
	{
		get
		{
			//keep in mind that the ToUnityVector3() function swaps the Y and Z axes, as iisu and Unity have a different axis system.
			return _uiPointer.Value.ToUnityVector3();	
		}
	}
		
	/// <summary>
	/// The depthmap that is extracted from the source (camera, skv movie)
	/// </summary>	
	public Iisu.Data.IImageData DepthMap
	{
		get
		{
			return _depthImage.Value;
		}
	}
	
	/// <summary>
	/// The label image consists of an array of integer values that correspond to each pixel of the depthmap. 
	/// Each integer value represents an object ID, meaning that every entity in the scene recognized by iisu as a
	/// seperate object has a unique object ID.
	/// In this way, we can extract the user, and show a user mask in the depthmap
	/// </summary>
	public Iisu.Data.IImageData LabelImage
	{
		get
		{
			return _labelImage.Value;
		}
	}
	
	/// <summary>
	/// the object ID from the LabelImage that corresponds to the user
	/// </summary>
	public int UserImageID
	{
		get
		{
			return _userImageID.Value;	
		}
	}
	
	public void LaunchToolbox ()
	{
		_iisuUnity.StartToolBox();
	}
}
