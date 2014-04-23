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
using System;
using System.Runtime.InteropServices;
using Iisu.Data;

/// <summary>
/// Helper class to convert iisu images to Unity images. 
/// </summary>
public class ImageConvertor
{
    private Color[] _colored_image;
	
	private int _width;
	private int _height;
	
	private byte[] imageRaw;
	private byte[] idImageRaw;
    
	private float floatConvertor = 1f / 255f;
	
	//Scheme on how the color of the user mask is determined
	
	//Position of the user: | Camera Position----|-------------Sub optimal range------------|-------------Optimal range----------|----------Sub optimal range-----------|-------
	//                   	|                    |                                          |                                    |                                      |
	//Pre defined ranges:   |               minBadRange                                minGoodRange                         maxGoodRange                            maxBadRange
	//                      |                    |                                          |                                    |                                      |
	//Pixel color:          |                    |<-- red ------------------------ green -->|<-------------- green ------------->|<-- green -------------------- red -->| 
	
	//these values vary between 0 and 255, and are 
	//actually representing normalized distances: 5m (5000mm) is remapped to 0 to 255 values
	private int minBadRange = 50;
	private int maxBadRange = 190;
	private int minGoodRange = 70;
	private int maxGoodRange = 160;
	
	//the minimum red color value used for the user mask
	private int minRed = 170;
    
	private float aaAmount = 0.25f;
	
	public ImageConvertor(int width, int height)
    {
		_width = width;
		_height = height;
    }
	
	public ImageConvertor()
	{
		_width = 160;
		_height = 120;
	}
 
	private void getUVEquivalent(int fromWidth, int fromHeight, int fromU, int fromV, int toWidth, int toHeight, out int toU, out int toV, out int toIndex)
	{
		float uNorm = (float)fromU/(float)fromWidth;
		float vNorm = (float)fromV/(float)fromHeight;
		
		toU = (int)(uNorm * toWidth);
		toV = (int)(vNorm * toHeight);
		
		toIndex = toU + toV * toWidth;
	}
	
	private void getUV(int index, int width, int height, out int u, out int v)
	{
		u = index % width;
		v = index / width;
	}
	
	/// <summary>
	///	A somewath simple and naive but fast real time anti aliasing to improve the shape of the user mask a bit.
	/// It checks the surroundings of the current pixel, and makes it more and more transparent for each non-user-mask
	/// pixel found in its neighbours.
	/// </summary>
	private float getAlphaValue(int index, int u, int v, int width, int height, int userID, float initAlpha)
	{
		float alpha = initAlpha;
		
		if(u-1 >= 0)
		{
			if(idImageRaw[index-1] != userID)
				alpha -= aaAmount;
			
			if(v-1 >= 0)
			{
				if(idImageRaw[index - width - 1] != userID)
					alpha -= aaAmount;	
			}
			if(v+1 < height)
			{
				if(idImageRaw[index + width - 1] != userID)
				alpha -= aaAmount;
			}
		}
		if(u+1 < width)
		{
			if(idImageRaw[index+1] != userID)
				alpha -= aaAmount;
			
			if(v-1 >= 0)
			{
				if(idImageRaw[index - width + 1] != userID)
				alpha -= aaAmount;
			}
			if(v+1 < height)
			{
				if(idImageRaw[index + width + 1] != userID)
				alpha -=aaAmount;
			}
		}
		if(v-1 >= 0)
		{
			if(idImageRaw[index-width] != userID)
				alpha -= aaAmount;
		}
		if(v+1 < height)
		{
			if(idImageRaw[index+width] != userID)
				alpha -= aaAmount;
		}
		
		if(alpha < 0)
			alpha = 0;
		
		return alpha;
	}
	
	/// <summary>
	/// Generates a grayscale depthmap with a given alpha, by converting the iisu image to a Unity Texture2D.
	/// </summary>
	public bool generateDepthMap(IImageData image, ref Texture2D destinationImage,float alpha)
	{
		if(image == null)
			return false;
		
		if (image.Raw == IntPtr.Zero)
			return false;

		if (_colored_image == null || _colored_image.Length != image.ImageInfos.BytesRaw / 2)
        {
            _colored_image = new Color[_width*_height];
            imageRaw = new byte[image.ImageInfos.BytesRaw];
        }
		
		uint byte_size = (uint)image.ImageInfos.BytesRaw;
				
		// copy image content into managed array
        Marshal.Copy(image.Raw, imageRaw, 0, (int)byte_size);
		
		int destinationU, destinationV;
		int sourceU, sourceV;
		int sourceIndex;
		
		int imageWidth = (int)image.ImageInfos.Width;
		int imageHeight = (int)image.ImageInfos.Height;
		
		//build op the Unity Texture2D
		for(int destinationIndex = 0; destinationIndex<_colored_image.Length; ++destinationIndex)
		{
			//get the UV coordinates of the destination texture
			getUV(destinationIndex, _width, _height, out destinationU, out destinationV);
			
			//get the UV coordinates from the original iisu image, remapped to the destination textures widh and height
			getUVEquivalent(_width, _height, destinationU, destinationV, imageWidth, imageHeight, out sourceU, out sourceV, out sourceIndex);
			
			// reconstruct ushort value from 2 bytes (low indian)
			ushort value = (ushort)(imageRaw[sourceIndex * 2] + (imageRaw[sourceIndex * 2 + 1] << 8));
			
            //normalize depth value in millimeter so that 5m <-> color 255
			value = (ushort)(value * 255/(5000));
			if (value > 255) value = 255;
			
			//apply a grayscale color indicating the depth to the current pixel
			_colored_image[destinationIndex].r = value * floatConvertor;
			_colored_image[destinationIndex].g = value * floatConvertor;
			_colored_image[destinationIndex].b = value * floatConvertor;
			_colored_image[destinationIndex].a = alpha;
        }
		
		destinationImage.SetPixels(_colored_image);
		destinationImage.Apply();
		
		return true;

	}
	
	/// <summary>
	/// If the user is detected, a user mask can be generated using this method. It leaves out the environment, 
	/// and colors the user mask depending on the distance the user is in respect to the camera. If the user
	/// is located at the optimal distance to the camera, the user mask is colored green. When the user moves to 
	/// far from or to close to the camera, the user masks color turns to red.
	/// </summary>
    public bool generateUserMask(IImageData image, IImageData idImage, ref Texture2D destinationImage, int userID, float alpha, bool antiAliased)
    {
		if(image == null || idImage == null)
			return false;
		
		if (image.Raw == IntPtr.Zero || idImage.Raw == IntPtr.Zero)
			return false;

		if (_colored_image == null || _colored_image.Length != image.ImageInfos.BytesRaw / 2)
        {
            _colored_image = new Color[_width*_height];
            imageRaw = new byte[image.ImageInfos.BytesRaw];
        }
		
		if(idImageRaw == null || idImageRaw.Length != idImage.ImageInfos.BytesRaw)
		{
			idImageRaw = new byte[idImage.ImageInfos.BytesRaw];	
		}
		
		uint byte_size = (uint)image.ImageInfos.BytesRaw;
		uint labelImageSize = (uint)idImage.ImageInfos.BytesRaw;
				
		// copy image content into managed arrays
        Marshal.Copy(image.Raw, imageRaw, 0, (int)byte_size);
		Marshal.Copy(idImage.Raw, idImageRaw, 0, (int)labelImageSize);
		
		int destinationU, destinationV;
		int sourceU, sourceV;
		int sourceIndex;
		int labelU, labelV;
		int labelIndex;
		
		int colorUnits;
		int red, green;
		
		int imageWidth = (int)image.ImageInfos.Width;
		int imageHeight = (int)image.ImageInfos.Height;
		int idImageWidth = (int)idImage.ImageInfos.Width;
		int idImageHeight = (int)idImage.ImageInfos.Height;
		
		//build up the user mask
		for(int destinationIndex = 0; destinationIndex<_colored_image.Length; ++destinationIndex)
		{
			//get the UV coordinates from the final texture that will be displayed
			getUV(destinationIndex, _width, _height, out destinationU, out destinationV);
			
			//the resolutions of the depth and label image can differ from the final texture, 
			//so we have to apply some remapping to get the equivalent UV coordinates in the depth and label image.
			getUVEquivalent(_width, _height, destinationU, destinationV, imageWidth, imageHeight, out sourceU, out sourceV, out sourceIndex);
			getUVEquivalent(_width, _height, destinationU, destinationV, idImageWidth, idImageHeight, out labelU, out labelV, out labelIndex);
			
			// reconstruct ushort value from 2 bytes (low indian)
			ushort value = (ushort)(imageRaw[sourceIndex * 2] + (imageRaw[sourceIndex * 2 + 1] << 8));
						
            //normalize depth value in millimeter so that 5m <-> color 255
			value = (ushort)(value * 255/(5000));
			if (value > 255) value = 255;
		
			//If the current pixel is part of the user mask, we draw it in a color that varies from green to red,
			//depending on the distance the user is located from the camera.
			//The way how the color is determined can be seen at the top of this file.
			if(labelIndex < idImageRaw.Length && idImageRaw[labelIndex] == userID)
			{
				if(value < minGoodRange)
				{
					colorUnits = (int)(((float)(Mathf.Clamp(value, minBadRange, minGoodRange) - minBadRange) / (float)(minGoodRange - minBadRange)) * (255 - minRed + 255));
					
					red = Mathf.Clamp(minRed + colorUnits, minRed, 255);
					colorUnits -= (red - minRed);
					green = Mathf.Clamp(colorUnits, 0, int.MaxValue);
					
					_colored_image[destinationIndex].r = red * floatConvertor;
					_colored_image[destinationIndex].g = green * floatConvertor;
					_colored_image[destinationIndex].b = 0;
					if(antiAliased)
						_colored_image[destinationIndex].a = getAlphaValue(labelIndex, labelU, labelV, idImageWidth, idImageHeight, userID, alpha);
					else
						_colored_image[destinationIndex].a = alpha;
				}
				else if(value >= minGoodRange && value <= maxGoodRange)
				{
					colorUnits = (int)(((float)(value - minGoodRange) / (float)(maxGoodRange - minGoodRange)) * 500);
					
					red = Mathf.Clamp(255 - colorUnits, 0, 255);
					colorUnits -= (255 - red);
					red += Mathf.Clamp(colorUnits, 0, int.MaxValue);
					
					_colored_image[destinationIndex].r = red * floatConvertor;
					_colored_image[destinationIndex].g = 1;
					_colored_image[destinationIndex].b = 0;
					if(antiAliased)
						_colored_image[destinationIndex].a = getAlphaValue(labelIndex, labelU, labelV, idImageWidth, idImageHeight, userID, alpha);
					else
						_colored_image[destinationIndex].a = alpha;
				}
				else
				{
					colorUnits = (int)(((float)(Mathf.Clamp(value, maxGoodRange, maxBadRange) - maxGoodRange) / (float)(maxBadRange - maxGoodRange)) * (255 - minRed + 255));
					
					green = Mathf.Clamp(255 - colorUnits, 0, int.MaxValue);
					colorUnits -= (255 - green);
					red = Mathf.Clamp(255 - colorUnits, minRed, 255);
					
					_colored_image[destinationIndex].r = red * floatConvertor;
					_colored_image[destinationIndex].g = green * floatConvertor;
					_colored_image[destinationIndex].b = 0;
					if(antiAliased)
						_colored_image[destinationIndex].a = getAlphaValue(labelIndex, labelU, labelV, idImageWidth, idImageHeight, userID, alpha);
					else
						_colored_image[destinationIndex].a = alpha;
				}
				
			}
			//if the pixel is outside of the user mask, it is made completely transparent.
			else
			{
				_colored_image[destinationIndex].r = value * floatConvertor;
				_colored_image[destinationIndex].g = value * floatConvertor;
				_colored_image[destinationIndex].b = value * floatConvertor;
				_colored_image[destinationIndex].a = 0;
			}	
        }
		
		destinationImage.SetPixels(_colored_image);
		destinationImage.Apply();
		
		return true;

    }
}