using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Underwater : MonoBehaviour {

	void Start () {
		//Set the background color
		GetComponent<Camera> ().backgroundColor = new Color (0, 0.4f, 0.7f, 1);

		RenderSettings.fog = true;
		RenderSettings.fogColor = new Color (0, 0.4f, 0.7f, 0.6f);
		RenderSettings.fogDensity = 0.04f;
		RenderSettings.skybox = null;
	}

}
	