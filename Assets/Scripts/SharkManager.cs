using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkManager : MonoBehaviour {

    public GameObject player;
    public GameObject sharkPrefab;
    public float corridorWidth = 16f;
    public float zRandom = 1.5f;
    public float startHeight = 30f;
    public float maxSpeed = 10.0f;
	public float speed;
	public float maxFrequency = 5f;  //par secondes
	public float frequency;
	public bool isAttacking = false;
	float time;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
        if (Input.GetKeyUp("s"))
        {
            SpawnShark();
        }

		if (isAttacking) {
			if (time > frequency) {
				time = 0;
				SpawnShark ();
			}
		}		
	}
		
	public void Start(float difficultyLevel)
	{
		frequency = maxFrequency * difficultyLevel;
		speed = maxSpeed * difficultyLevel;
		isAttacking = true;
	}

	public void Stop()
	{
		isAttacking = false;
	}

    public void SpawnShark()
    {
        // Compute distance
        float distToTravelShark = startHeight;
        float timeToTravel = distToTravelShark / speed; // seconds
        float distToTravelPlayer = timeToTravel * player.GetComponent<Rigidbody>().velocity.z;
		// Random Z
		float rand = Random.Range(-zRandom,zRandom);
		float newZ = player.transform.position.z + (rand);
        // Compute shark initial position
		Vector3 sharkPos = new Vector3(Random.Range(-corridorWidth / 2f, corridorWidth / 2f), startHeight, distToTravelPlayer+newZ);
        // Spawn shark
        GameObject shark = GameObject.Instantiate(sharkPrefab, sharkPos, Quaternion.identity);
        shark.GetComponent<Shark>().speed = speed;
    }
}
