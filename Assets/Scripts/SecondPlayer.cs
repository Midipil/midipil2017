using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPlayer : MonoBehaviour {

	public GameObject sharkPrefab;
	public float spawnHight;
	public Camera cam;
	Vector3 pos = new Vector3();
	public float frequency;
	float time;
	public GameObject player;
	private Vector3 offset;  

	void Start () 
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		offset = transform.position - player.transform.position;
	}

	void Update () 
	{
		time += Time.deltaTime;

		if(Input.GetMouseButtonDown(0) && time > frequency)   
		{
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {

				GameObject obj = Instantiate (sharkPrefab);
				obj.transform.position = new Vector3 (hit.point.x, hit.point.y + spawnHight, hit.point.z);
				time = 0;
			}
		}
	}

	void LateUpdate () 
	{
		transform.position = player.transform.position + offset;
	}
}
