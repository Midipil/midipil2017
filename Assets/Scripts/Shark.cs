using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour {

	// Speed in m/s
	public float speed = 10.0f;

	public ParticleSystem bubbles;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (this.transform.position.y > 0) {
			this.transform.Translate (new Vector3 (0f, -speed * Time.deltaTime, 0f));
		} else {
			// Shark is in the ground
			if (bubbles.isEmitting) {
				bubbles.Stop ();
			}
		}
	}
}
