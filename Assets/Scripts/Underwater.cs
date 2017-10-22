using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Underwater : MonoBehaviour {

	public Color fogColor = new Color (0, 0.4f, 0.7f, 1);

	void Start () {
		//Set the background color
		GetComponent<Camera> ().backgroundColor = fogColor;
		//GetComponent<Camera> ().clearFlags = CameraClearFlags.Color;

		RenderSettings.fog = true;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = 0.04f;
		RenderSettings.skybox = null;
	}

}
	