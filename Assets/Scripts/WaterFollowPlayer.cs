﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFollowPlayer : MonoBehaviour {

	private Player player;

	// Use this for initialization
	void Start () {
		GameObject playerGo = GameObject.FindWithTag ("Player");
		if (playerGo == null) {
			Debug.LogError ("Can't find any gameObject with tag \"Player\"");
		}
		player = GameObject.FindWithTag ("Player").GetComponent<Player> ();
		if (player == null) {
			Debug.LogError ("Player GameObject does not have any \"Player\" component");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null) {
			transform.position.x = player.head.position.x;
			transform.position.z = player.head.position.z;
		}
	}
}
