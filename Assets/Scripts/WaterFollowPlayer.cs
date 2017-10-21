using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFollowPlayer : MonoBehaviour {

	private PlayerController player;

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
			transform.position.Set (player.head.position.x, transform.position.y, player.head.position.z);
		}
	}
}
