using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MirrorCamera : MonoBehaviour {

	public Camera playerCamera;
	public Camera mirroredCamera;
	public GameObject waterPlane;
	public int textureSize = 256;

	// Use this for initialization
	void Start () {
		mirroredCamera.farClipPlane = playerCamera.farClipPlane;
		mirroredCamera.nearClipPlane = playerCamera.nearClipPlane;
		mirroredCamera.orthographic = playerCamera.orthographic;
		mirroredCamera.fieldOfView = playerCamera.fieldOfView;
		mirroredCamera.aspect = playerCamera.aspect;
		mirroredCamera.orthographicSize = playerCamera.orthographicSize;

		mirroredCamera.targetTexture = new RenderTexture (textureSize, textureSize, 16);

		Shader.DisableKeyword ("WATER_SIMPLE");
		Shader.EnableKeyword ("WATER_REFLECTIVE");
		Shader.DisableKeyword ("WATER_REFRACTIVE");
	}
	
	// Update is called once per frame
	void Update () {
		mirroredCamera.transform.position = waterPlane.transform.position - (playerCamera.transform.position - waterPlane.transform.position);
		mirroredCamera.transform.rotation = new Quaternion (
			playerCamera.transform.rotation.x,
			-playerCamera.transform.rotation.y,
			playerCamera.transform.rotation.z,
			-playerCamera.transform.rotation.w
		);

		Material mat = waterPlane.GetComponent<Renderer> ().sharedMaterial;
		Vector4 waveSpeed = mat.GetVector ("WaveSpeed");
		float waveScale = mat.GetFloat ("_WaveScale");
		Vector4 waveScale4 = new Vector4 (waveScale, waveScale, waveScale * 0.4f, waveScale * 0.45f);

		// Time since level load, and do intermediate calculations with doubles
		double t = Time.timeSinceLevelLoad / 20.0;
		Vector4 offsetClamped = new Vector4 (
			                        (float)Math.IEEERemainder (waveSpeed.x * waveScale4.x * t, 1.0),
			                        (float)Math.IEEERemainder (waveSpeed.y * waveScale4.y * t, 1.0),
			                        (float)Math.IEEERemainder (waveSpeed.z * waveScale4.z * t, 1.0),
			                        (float)Math.IEEERemainder (waveSpeed.w * waveScale4.w * t, 1.0)
		                        );

		mat.SetVector ("_WaveOffset", offsetClamped);
		mat.SetVector ("_WaveScale4", waveScale4);

		mirroredCamera.Render ();

		waterPlane.GetComponent<Renderer> ().sharedMaterial.SetTexture ("_ReflectionTex", mirroredCamera.targetTexture);
	}

	void OnPreCull () {
		mirroredCamera.ResetWorldToCameraMatrix ();
		mirroredCamera.ResetProjectionMatrix ();
		Vector3 scale = new Vector3 (1, -1, 1);
		mirroredCamera.projectionMatrix = mirroredCamera.projectionMatrix * Matrix4x4.Scale (scale);
	}

	void OnPreRender () {
		GL.SetRevertBackfacing (true);
	}

	void OnPostRender () {
		GL.SetRevertBackfacing (false);
	}
}
