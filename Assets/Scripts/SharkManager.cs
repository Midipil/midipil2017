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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp("s"))
        {
            SpawnShark();
        }
	}

    public void Start(float difficulty)
    {

    }

    public void Stop()
    {

    }

    public void SpawnShark()
    {
        // Compute distance
        float distToTravelShark = startHeight;
        float timeToTravel = distToTravelShark / speed; // seconds
        float distToTravelPlayer = timeToTravel * player.GetComponent<Rigidbody>().velocity.z;
        // Compute shark initial position
        Vector3 sharkPos = new Vector3(Random.Range(-corridorWidth / 2f, corridorWidth / 2f), startHeight, distToTravelPlayer+player.transform.position.z);
        // Spawn shark
        GameObject shark = GameObject.Instantiate(sharkPrefab, sharkPos, Quaternion.identity);
        shark.GetComponent<Shark>().speed = speed;
    }
}
