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

public class DepthImage : MonoBehaviour
{
    public IisuInputProvider IisuInput;

    private ImageConvertor _imageConvertor;

    public Texture2D DepthMap;
	
	public float NormalizedXCoordinate;
	public float NormalizedYCoordinate;
	public float NormalizedWidth;
	public float Alpha;
	
	private float _heightWidthRatio;
	
	private float _timer;
	
	public bool AntiAliasing;
	
    void Awake()
    {
        _imageConvertor = new ImageConvertor(160, 120);
		_timer = 0;
		_heightWidthRatio = 120f/160f;
    }
	
	/// <summary>
	/// We get the depth image from iisu, which is in a 16bit grey image format 
	/// The image is converted by the ImageConvertor class to a Unity image, and then applied to the 2D GUI texture
	/// </summary>	
    void Update()
    {
		if(_timer >= 0.0333f)
		{
			_timer = 0;
			
			if (DepthMap == null)
	        {
	            DepthMap = new Texture2D(160, 120, TextureFormat.ARGB32, false);
	       	}
			
			if(IisuInput.User1Active)
			{
				_imageConvertor.generateUserMask(IisuInput.DepthMap, IisuInput.LabelImage, ref DepthMap, IisuInput.UserImageID, Alpha, AntiAliasing);	
			}
			else
			{
				_imageConvertor.generateDepthMap(IisuInput.DepthMap, ref DepthMap, Alpha);
			}
		}
		else
		{
			_timer += Time.deltaTime;
		}
    }
	
	void OnGUI()
	{
		if(DepthMap != null)
		{
			GUI.DrawTexture(new Rect(Screen.width * NormalizedXCoordinate, 
			                      	 Screen.height * NormalizedYCoordinate + Screen.width * NormalizedWidth * _heightWidthRatio, 
			                         Screen.width * NormalizedWidth,
			                         -Screen.width * NormalizedWidth * _heightWidthRatio), DepthMap);
		}
	}
}
