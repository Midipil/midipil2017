using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkManager : MonoBehaviour {

	GameObject player;
	public GameObject sharkPrefab;
	public float corridorWidth = 16f;
	public float zRandom = 1.5f;
	public float startHeight = 30f;
	public float speed = 1f;
	//par secondes
	public float frequency = 6f;
	public bool isAttacking = false;
	public float easySpeed = 7f;
	public float hardSpeed = 18f;
	public float easyFreq = 6f;
	public float hardFreq = 3f;
	float time;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (Input.GetKeyUp ("s")) {
			SpawnShark ();
		}

		if (isAttacking) {
			if (time > frequency) {
				time = 0;
				SpawnShark ();
			}
		}		
	}

	public void Start (float difficultyLevel) {
		speed = Mathf.Lerp (easySpeed, hardSpeed, difficultyLevel);
		frequency = Mathf.Lerp (easyFreq, hardFreq, difficultyLevel);
		isAttacking = true;
	}

	public void Stop () {
		isAttacking = false;
	}

	[ContextMenu("SpawnShark")]
	public void SpawnShark () {
		// Compute distance
		float distToTravelShark = startHeight;
		float timeToTravel = distToTravelShark / speed; // seconds
		float distToTravelPlayer = timeToTravel * player.GetComponent<Rigidbody> ().velocity.z;
		// Random Z
		float rand = Random.Range (0, zRandom);
		float newZ = player.transform.position.z + (rand);
		// Compute shark initial position
		Vector3 sharkPos = new Vector3 (Random.Range (-corridorWidth / 3f, corridorWidth / 3f), startHeight, distToTravelPlayer + newZ);
		// Spawn shark
		GameObject shark = GameObject.Instantiate (sharkPrefab, sharkPos, Quaternion.identity);
		shark.GetComponent<Shark> ().speed = speed;
	}
}
