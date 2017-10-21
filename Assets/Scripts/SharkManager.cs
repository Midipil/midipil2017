using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkManager : MonoBehaviour {

    public GameObject player;
    public GameObject sharkPrefab;
    public float corridorWidth = 16f;
    public float zRandom = 1.5f;
    public float startHeight = 30f;
    public float speed = 10.0f;
	public float frequency = 5f;  //par secondes
	public bool isAttacking = false;
	public float difficulty = 0;
	float time;
	// Use this for initialization
	void Start () {
		
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

	public void StartAttack(float difficultyLevel)
	{
		difficulty = difficultyLevel;
		isAttacking = true;
	}

	public void StopAttack()
	{
		
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
