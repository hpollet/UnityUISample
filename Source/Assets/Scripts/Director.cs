using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {

	// ceci est juste pour verifier que Github va voir la difference 
	// rien n'a ete visible
	public IisuInputProvider InputProvider;
	
	void Update()
	{
		//if the user presses CTRL + T, the Toolbox is launched from the application
		if( (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.T))
		{
			InputProvider.LaunchToolbox();
		}	
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(30, Screen.height - 50, 600, 50), "Press CTRL + T to launch the Toolbox");
	}
}
