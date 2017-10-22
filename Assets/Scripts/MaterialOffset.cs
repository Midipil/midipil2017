using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOffset : MonoBehaviour {

	private PlayerController player;

	public float wavesSpeed = 1;
	public float wavesAmplitude = 0.02f;

	// Use this for initialization
	void Start () {
		GameObject playerGo = GameObject.FindWithTag ("Player");
		if (playerGo == null) {
			Debug.LogError ("Can't find any gameObject with tag \"Player\"");
			return;
		}
		player = GameObject.FindWithTag ("Player").GetComponent<PlayerController> ();
		if (player == null) {
			Debug.LogError ("Player GameObject does not have any \"Player\" component");
		}
	}

	// Update is called once per frame
	void Update () {
		if (player != null) {
			Material mat = GetComponent<Renderer> ().material;
			mat.mainTextureOffset = new Vector2 (wavesAmplitude * Mathf.Sin (Time.time * wavesSpeed) - player.transform.position.x / mat.mainTextureScale.x * transform.localScale.x, wavesAmplitude * Mathf.Sin (Time.time * wavesSpeed) + player.transform.position.z / mat.mainTextureScale.y * transform.localScale.z);
		}
	}
}
